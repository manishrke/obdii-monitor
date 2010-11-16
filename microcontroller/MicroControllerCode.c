#define FCY 4000000UL
#define COMBUFF 32
#define SENSORS 65

#include "FSIO.h"
#include "uart.h"
#include "string.h"
#include "spi.h"

char   ELM_Prompt = 0;
char   UART1SaveString[COMBUFF+2];
char   UART1Accept = 1;
char * UART1RecvBuffer;
char * UART1RecvPtr;
char   UART1RecvBytes = 0;
char   UART1CRCount = 0;

FSFILE *logFile, *sensors;

_CONFIG2(IESO_OFF & FNOSC_PRIPLL & FCKSM_CSDCMD & OSCIOFNC_OFF & POSCMOD_HS)
_CONFIG1(JTAGEN_OFF & GCP_OFF & GWRP_OFF & COE_OFF & ICS_PGx2 & FWDTEN_OFF & BKBUG_ON)

///////////////////////////////////////////////////////////////////
// Timer Code
///////////////////////////////////////////////////////////////////
const short int timerTicksPerMS = 2000;
long int timerTicksRaw = 0;
long int timerMS = 0;

void __attribute__((__interrupt__, __shadow__)) _T3Interrupt(void){
	IFS0bits.T3IF = 0;
    timerMS += 60000; //real MS is timerMS + timerTicksRaw/timerTicksPerMS
}

void Timer_Init(void){
	T3CON  = 0x0000;
	T2CON  = 0x0018;
	
	TMR3   = 0;
	TMR2   = 0;
	
	PR3    = 0x0727;  
	PR2    = 0x0E00;
	
	IPC2bits.T3IP = 0x07; // T3IP<2:0> bits (IPC2<2:0>) are priority
	IFS0bits.T3IF = 0;  //T3IF   (EFS0<8>) is used as status flag for inter
	IEC0bits.T3IE = 1;  //T3IE   (IEC0<8>) is used to enable 32 bit timer int
	T2CONbits.TON = 1;
}

long int Timer_GetTimeMS(void){
	return timerMS + ((((((long int)TMR3) << 16)|TMR2) / timerTicksPerMS));
}

void Timer_WaitMS(long int ms){
	long int old_ms = Timer_GetTimeMS();
	while((Timer_GetTimeMS() - old_ms) < ms);
}
///////////////////////////////////////////////////////////////////

	//IPC13bits.INT4IP = 0x03;
	//IFS3bits.INT4IF = 0;
	//IEC3bits.INT4IE = 1;  

///////////////////////////////////////////////////////////////////
// Accelerometer Code
///////////////////////////////////////////////////////////////////
#define ACCEL_PACKET_LENGTH 11

char accelAxisReady = 0;
char accelAxisPoll = 0;
char accelSaveString[ACCEL_PACKET_LENGTH];

void __attribute__((__interrupt__)) _INT4Interrupt(void){  
    IFS3bits.INT4IF = 0;
} 

void __attribute__((__interrupt__)) _SPI2Interrupt(void){  
	int data, i;
	for(i=0; i<5; i+=1);
	
	PORTG = 0x0200;
    IFS2bits.SPI2IF = 0;
	PORTA = PORTA ^ 0x0080;

	data = SPI2BUF;

	if(accelAxisPoll > 0){
		*(accelSaveString + 7 + accelAxisPoll) = (char)(data & 0x00FF);
		if(accelAxisPoll >= 3){
			accelAxisPoll = 0;
			accelAxisReady = 1;
		}else{
			for(i=0; i<4; i+=1);
			accelAxisPoll += 1;
			PORTG   = 0x0000;
			for(i=0; i<1; i+=1);
			SPI2BUF = (0x03 + accelAxisPoll*2) << 10;
		}
	}
} 

void SPI_Init(void){
	ConfigIntSPI2(SPI_INT_EN & SPI_INT_PRI_5); 
	SPI2CON1 = 0x0536;   
	SPI2STATbits.SPIROV = 0;
	SPI2STATbits.SPIEN  = 1;
}

void Accel_Init(void){
	accelSaveString[0] = 0xEE;
	accelSaveString[1] = 0x09;
	accelSaveString[6] = 'A';
	accelSaveString[7] = 'C';
	SPI_Init();
}

void Accel_BeginUpdateSaveString(void){
	int i;
	accelAxisReady = 0;
	accelAxisPoll = 1;
	PORTG   = 0x0000;
	for(i=0; i<1; i+=1);
	SPI2BUF = (0x03 + accelAxisPoll*2) << 10;
}

void Accel_WaitUpdated(void){
	while(!accelAxisReady);
}
///////////////////////////////////////////////////////////////////



void __attribute__((__interrupt__)) _U1TXInterrupt(void){  
    IFS0bits.U1TXIF = 0;
} 

void __attribute__((__interrupt__)) _U1RXInterrupt(void){
    char ch;
    IFS0bits.U1RXIF = 0;
    while( DataRdyUART1()){
        if(!UART1Accept) continue;
		if(UART1RecvBytes-1 > COMBUFF) continue; //IMPL - raise error flag or something like that - overflow
        ch = ReadUART1();
        WriteUART2(ch);
		 
        if(ch=='>') 
			ELM_Prompt = 1;	
		else if(ch==0x0D){
			UART1CRCount += 1;
			continue;
		}else{
	        ( *(UART1RecvPtr)++) = ch;
			UART1RecvBytes += 1;
		}		
    } 
} 
 
void __attribute__((__interrupt__)) _U2TXInterrupt(void){  
    IFS1bits.U2TXIF = 0;
} 

void __attribute__((__interrupt__)) _U2RXInterrupt(void){
    char ch;
    IFS1bits.U2RXIF = 0;
    while( DataRdyUART2()){
        ch = ReadUART2();
        //PORTA = ch;
        WriteUART1(ch);
    } 
}  

void ELM_UART_Write(unsigned int *string){
	UART1RecvBytes = 0;
	UART1CRCount   = 0;
	UART1RecvPtr   = UART1RecvBuffer;
	putsUART1(string);
}


int ELM_Wait(short int mstimeout){
	long int time_start = Timer_GetTimeMS();
    while(!ELM_Prompt){
		if((short int)(Timer_GetTimeMS() - time_start) > mstimeout) return 0;
    }
    ELM_Prompt = 0;
	UART1RecvBuffer[(int)UART1RecvBytes] = 0;  //Make a terminator for the string
	return 1;  
};

const char ELM_RESET_STR[] = "ATZ\r\0";

int ELM_Reset(void){
	ELM_UART_Write((unsigned int *)ELM_RESET_STR); 
	if(!ELM_Wait(1000)) return 0;
	if(!strstr(UART1RecvBuffer, "ELM327")) return 0;
	return 1;
}


const char ELM_ECHO_OFF[] = "ATE0\r\0";

int ELM_EchoOff(void){
	ELM_UART_Write((unsigned int *)ELM_ECHO_OFF); 
	if(!ELM_Wait(1000)) return 0;
	if(!strstr(UART1RecvBuffer, "OK")) return 0;
	return 1;
}

const char ELM_SPACES_OFF[] = "ATS0\r\0";

int ELM_SpacesOff(void){
	ELM_UART_Write((unsigned int *)ELM_SPACES_OFF);
	if(!ELM_Wait(1000)) return 0;
	if(!strstr(UART1RecvBuffer, "OK")) return 0;
	return 1;
}

const char ELM_RETURN_OFF[] = "ATL0\r\0";

int ELM_ReturnOff(void){
	ELM_UART_Write((unsigned int *)ELM_RETURN_OFF); 
	if(!ELM_Wait(1000)) return 0;
	if(!strstr(UART1RecvBuffer, "OK")) return 0;
	return 1;
}

char charToASCIIHex(char val){
    val = 0x0F & val;
    if(val < 10) return val + 0x30;
	return val + 0x41 - 10;
}

char charToASCIIHexMS(char val){
	return charToASCIIHex(val >> 4);
}

char ELM_SENSOR_READ[] = "01xx\r\0";

int ELM_SensorRead(char pid){
    ELM_SENSOR_READ[2] = charToASCIIHexMS(pid);
    ELM_SENSOR_READ[3] = charToASCIIHex(pid);
	ELM_UART_Write((unsigned int *)ELM_SENSOR_READ);    
	//check for happy reply, 01 0C should be 01 4C
	return 1;  
}

char ELM_SENSOR_READ_FAST[] = "01xxx\r\0";

int ELM_SensorReadFast(char pid, char replies){
    ELM_SENSOR_READ_FAST[2] = charToASCIIHexMS(pid);
    ELM_SENSOR_READ_FAST[3] = charToASCIIHex(pid);
    ELM_SENSOR_READ_FAST[4] = charToASCIIHex(replies);
	ELM_UART_Write((unsigned int *)ELM_SENSOR_READ_FAST);    
	//check for happy reply, 01 0C should be 01 4C
	return 1;  
}

int ELM_DetectOBD(void){
	ELM_SensorRead(0x0C);  //For now we just force ELM->OBD negotiation by reading the RPM sensor
	if(!ELM_Wait(6000)) return 0;
	return 1;
}

char ELM_Sensors[SENSORS];
int ELM_Enumerate(void){
	int has_data = 0;
	int pid = 0;
	while(pid < SENSORS){
		PORTA = pid;
		has_data = 1;
		ELM_SensorRead(pid);
		if(!ELM_Wait(1000)) continue;

	    if(UART1RecvBuffer[0] != '4' || strstr(UART1RecvBuffer, "NO DATA")) has_data = 0;
		UART1CRCount = UART1CRCount >> 1;
		if(!has_data) UART1CRCount = 0;

		ELM_Sensors[pid] = has_data << 7 | (0x04 * has_data) | UART1CRCount;
		pid += 1;
	}
	return 1;
}



void ELM_Init(void){
	int connected = 0;
	while(!connected){
		PORTA = 0x0001;
		while(!ELM_Reset());
		PORTA = 0x0003;
		if(!ELM_EchoOff()) continue;
		PORTA|= 0x0007;
	    if(!ELM_SpacesOff()) continue;
		PORTA|= 0x000F;
		if(!ELM_ReturnOff()) continue;
		PORTA|= 0x001F;
		if(!ELM_DetectOBD()) continue;
		PORTA|= 0x003F;
		connected = 1;
	}   
}



void UART_Init(void){
	UART1RecvBuffer = UART1SaveString + 2;
	UART1RecvPtr = UART1RecvBuffer;

	ConfigIntUART1(UART_RX_INT_EN & UART_RX_INT_PR6 & 
                   UART_TX_INT_DIS & UART_TX_INT_PR2);
	OpenUART1(0x8008, 0x8400, 104);     //Without loopback


	ConfigIntUART2(UART_RX_INT_EN & UART_RX_INT_PR6 & 
                   UART_TX_INT_DIS & UART_TX_INT_PR2);
	OpenUART2(0x8008, 0x8400, 104);     //Without loopback
	
}



void IO_Init(void){
    TRISA = 0x0000;
	PORTA = 0x0000;

	TRISD = 0xFFFF;
	
	TRISG = 0xF1CF;
	PORTG = 0x0200;
}


int main(void){
	long int time_ms = 0;

	IO_Init();
	UART_Init();
	Timer_Init(); 
	Accel_Init();
	ELM_Init();


	PORTA = 0x00FF;
	while (!MDD_MediaDetect());  //Wait for SD
	while (!FSInit());		     //Init file system
	PORTA = 0x0000;

	sensors = FSfopen("Sensors.ini", "r");
    if(sensors == NULL){
		ELM_Enumerate();
		sensors = FSfopen("Sensors.ini", "w");
		if(sensors == NULL) while(1);
		if(FSfwrite(ELM_Sensors, 1, SENSORS, sensors) != SENSORS) while(1);	
	}else{
		if(FSfread(ELM_Sensors, 1, SENSORS, sensors) != SENSORS) while(1);	
	}
    if(FSfclose(sensors)) while(1);	
	

	logFile = FSfopen ("Sensors.log", "a");
	if (logFile == NULL) while(1);
    if (FSfwrite(&time_ms, 4, 1, logFile) != 1) while(1);	
    if (FSfwrite("NEW_SESSION\0", 1, 12, logFile) != 12) while(1);	

	UART1SaveString[0] = 'O';
	UART1SaveString[1] = 'B';
	int pid = 1;
	int doScanning = 1;
	unsigned int pass = 0;
	while(doScanning){
		if(pid >= SENSORS){
		    pid = 1;
            pass += 1;
			PORTA = PORTA ^ 0x0001;
 		}      
		if(!(ELM_Sensors[pid] & 0x80) || (ELM_Sensors[pid] & 0x7C) == 0 || pass%((ELM_Sensors[pid] & 0x7C)>>2) != 0 ){
			pid += 1;
			continue;
		}
		if((ELM_Sensors[pid]&0x03) > 0){
			ELM_SensorReadFast(pid, ELM_Sensors[pid]&0x03);
		}else{
			ELM_SensorRead(pid);
		}
		if(!ELM_Wait(2000)) break;
		
		if(!(PORTD & 0x0040)) doScanning = 0;

		Accel_BeginUpdateSaveString();        
		time_ms = Timer_GetTimeMS();
		*((long int *)(accelSaveString+2)) = time_ms;
		Accel_WaitUpdated();

		if (FSfwrite (accelSaveString, 1, ACCEL_PACKET_LENGTH, logFile) != ACCEL_PACKET_LENGTH) while(1);			

		if (FSfwrite (&time_ms, 4, 1, logFile) != 1) while(1);	
		if (FSfwrite (UART1SaveString, 1, UART1RecvBytes+3, logFile) != UART1RecvBytes+3) while(1);	
	

		pid += 1;
	}
    
	PORTA = 0x00FF;
	if (FSfclose (logFile)) while(1);
	PORTA = 0x00AA;
	while(1);

	CloseUART1();   
	CloseUART2();
}








