#define FCY 4000000UL
#define COMBUFF 32

#define SENSORS 65

#include "FSIO.h"
#include "uart.h"
#include "string.h"
#include "spi.h"

char   ELM_Prompt = 0;
char __attribute__((aligned(16))) UART1SaveString[COMBUFF+9];
char   UART1Accept = 1;
char * UART1RecvBuffer;
char * UART1RecvPtr;
char   UART1RecvBytes = 0;
char   UART1CRCount = 0;


FSFILE *logFile, *sensors;

_CONFIG2(IESO_OFF & FNOSC_PRIPLL & FCKSM_CSDCMD & OSCIOFNC_OFF & POSCMOD_HS)
_CONFIG1(JTAGEN_OFF & GCP_OFF & GWRP_OFF & COE_OFF & ICS_PGx2 & FWDTEN_OFF & BKBUG_ON)


char charToASCIIHex(char val){
    val = 0x0F & val;
    if(val < 10) return val + 0x30;
	return val + 0x41 - 10;
}

char charToASCIIHexMS(char val){
	return charToASCIIHex(val >> 4);
}

char ASCIIHexToChar(char hex){
	if(hex >= 0x41) return hex - 55;
	return hex - 0x30;
}



///////////////////////////////////////////////////////////////////
// Circular Buffer Code
///////////////////////////////////////////////////////////////////

#define CIRCULAR_BUFF_SIZE 20

typedef struct {
	unsigned char buffer[CIRCULAR_BUFF_SIZE];
	char wpos;
	char rpos;
	char full;
} CircularBuffer;

void initCircularBuffer(CircularBuffer *buf){
	buf->wpos = 0;
	buf->rpos = 0;
	buf->full = 0;
}
unsigned char hasChar(CircularBuffer *buf){
	if(buf->full) return 1;
	if(buf->rpos == buf->wpos) return 0;
	return 1;
}
unsigned char getChar(CircularBuffer *buf){
	unsigned char ch = buf->buffer[buf->rpos];
	buf->rpos += 1;
	if(buf->rpos == CIRCULAR_BUFF_SIZE) buf->rpos = 0;
	if(buf->full) buf->full = 0;
	return ch;
}
unsigned char putChar(CircularBuffer *buf, unsigned char ch){
	if(buf->full) return 0;
	buf->buffer[buf->wpos] = ch;
	buf->wpos += 1;
	if(buf->wpos == CIRCULAR_BUFF_SIZE) buf->wpos = 0;
	if(buf->wpos == buf->rpos) buf->full = 1;
	return 1;
}


///////////////////////////////////////////////////////////////////
// Timer Code
///////////////////////////////////////////////////////////////////
const short int timerTicksPerMS = 2000;
long int timerTicksRaw = 0;
long int timerMS = 0;

void __attribute__((__interrupt__, __shadow__)) _T3Interrupt(void){
    timerMS += 60000; //real MS is timerMS + timerTicksRaw/timerTicksPerMS
	IFS0bits.T3IF = 0;
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


///////////////////////////////////////////////////////////////////
// Accelerometer / SPI UART Main Code
///////////////////////////////////////////////////////////////////
#define ACCEL_PACKET_LENGTH 11
#define SPI_CS_ACCEL 0x0002
#define SPI_CS_UART  0x0200
#define SPI_CS_OFF   0x0202

CircularBuffer SPIUART_BufIn;
CircularBuffer SPIUART_BufOut;

short unsigned int SPI2_CS = SPI_CS_OFF;  
char SPIUART_WantsAttention = 0;
char SPIUART_LastWasWrite   = 0;
char SPIUART_HasAttention   = 0;
char accelAxisReady = 0;
char accelAxisPoll = 0;
char __attribute__((aligned(16))) accelSaveString[ACCEL_PACKET_LENGTH];

void __attribute__((__interrupt__)) _INT4Interrupt(void){  

	PORTA = PORTA ^ 0x0040;
	if(!SPIUART_HasAttention) SPIUART_Attention();
    IFS3bits.INT4IF = 0;
	
	//This interrupt happens whenever we have data from the UART3, we should switch the spi2cs to 
    //UART and om nom nom blargle blargle from the UART3 before switching back to accelerometer
	// maybe use 0x0002;  (rg1) for the UART chip?  rg9 is being used for accelo.
	// trnamission works in this way: write whilenotready; write whilenotready;
	//PORTG   = 0x0200;
	//for(i=0; i<1; i+=1);
	//SPI2BUF = 0x0000;

	//PORTG   = 0x0200;
	//for(i=0; i<1; i+=1);
	//SPI2BUF = 0x1000 | databyte;

	// R is data availablt
    // T is transmit buffer empty   R and T are the first 2 bits of any operation wewt wewt

} 

void __attribute__((__interrupt__)) _SPI2Interrupt(void){  
	int data, i;
	for(i=0; i<5; i+=1);
	
	PORTG = SPI_CS_OFF;
	PORTA = PORTA ^ 0x0080;

	data = SPI2BUF;

	if(SPI2_CS == SPI_CS_ACCEL){
		if(accelAxisPoll > 0){
			*(accelSaveString + 7 + accelAxisPoll) = (char)(data & 0x00FF);
			if(accelAxisPoll >= 3){
				accelAxisPoll = 0;
				accelAxisReady = 1;
    			SPI2_CS = SPI_CS_OFF; //Release the SPI
			}else{
				for(i=0; i<4; i+=1);
				accelAxisPoll += 1;
				PORTG   = SPI2_CS;
				for(i=0; i<1; i+=1);
				SPI2BUF = (0x03 + accelAxisPoll*2) << 10;
    			IFS2bits.SPI2IF = 0;
				return;
			}
		}
	}else if(SPI2_CS == SPI_CS_UART){
		for(i=0; i<2; i+=1); 
		if(data & 0x8000) putChar(&SPIUART_BufIn, (unsigned char)(data & 0x00FF));
		if(data & 0x4000 && hasChar(&SPIUART_BufOut)){
			PORTG   = SPI2_CS;
			if(SPIUART_LastWasWrite){
				SPIUART_LastWasWrite = 0;
				SPI2BUF = 0x0000;  
			}else{
				SPIUART_LastWasWrite = 1;
				SPI2BUF = 0x8000 | (getChar(&SPIUART_BufOut) & 0x00FF);
			}	
		}else if(data & 0x8000){
			PORTG   = SPI2_CS;
			SPI2BUF = 0x0000;  
		}else{
			SPI2_CS = SPI_CS_OFF;
		}
	}

	/*
	if(SPIUART_WantsAttention){
		SPIUART_WantsAttention = 0;
		if(SPI2_CS != SPI_CS_UART){
			SPI2_CS = SPI_CS_UART;
			PORTG   = SPI2_CS;
			SPI2BUF = 0x0000; 		
		}
	}
	*/

    IFS2bits.SPI2IF = 0;

} 

void SPI_Init(void){
	ConfigIntSPI2(SPI_INT_EN & SPI_INT_PRI_2); 
	SPI2CON1 = 0x0536;    // 16bit, master SPI, 1.333MHz clocking
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
	while(SPI2_CS != SPI_CS_OFF); // Wait until SPI UART has released the interface, if it is being held
	accelAxisReady = 0;
	accelAxisPoll = 1;
	SPI2_CS = SPI_CS_ACCEL;
	PORTG   = SPI2_CS;
	for(i=0; i<1; i+=1);
	SPI2BUF = (0x03 + accelAxisPoll*2) << 10;
}

void Accel_WaitUpdated(void){
	while(!accelAxisReady);
}
///////////////////////////////////////////////////////////////////

void SPIUART_Init(void){

	initCircularBuffer(&SPIUART_BufIn);
	initCircularBuffer(&SPIUART_BufOut);

	PORTG   = SPI_CS_UART;
	SPI2BUF = 0xCC09;     //1100 1100 0000 1001  with both transmit and receive interrups

	INTCON2bits.INT4EP = 1;
	IPC13bits.INT4IP = 0x03;
	IFS3bits.INT4IF = 0;
	IEC3bits.INT4IE = 1;  

}

void SPIUART_Send(const unsigned char *buffer, char bytes){
	char i=0;
	SPIUART_HasAttention = 1;

	while(i<bytes){
		if(putChar(&SPIUART_BufOut, buffer[i])) i += 1;
        SPIUART_Attention();
	}

	SPIUART_HasAttention = 0;
}

void SPIUART_Attention(void){
	if(SPI2_CS == SPI_CS_OFF){
		SPIUART_LastWasWrite = 0;
		SPI2_CS = SPI_CS_UART;
		PORTG   = SPI2_CS;
		SPI2BUF = 0x0000;  	//We don't know if we are clear to send data
	}else if(SPI2_CS == SPI_CS_ACCEL){
		SPIUART_WantsAttention = 1;
	}
}

///////////////////////////////////////////////////////////////////
// SPI UART Code
///////////////////////////////////////////////////////////////////
// when we send some of them there datamons over the UART we have to switch to 
// 16bit mode over summa them thar SPIs dudebrah
//
//  configuratoin to write: 0b1100 0100 0000 1001
//                            0101 0100 0000 1001
//                             C409
//  write data 0b1000 0000 dddd dddd
//  read  data 0b0000 0000 0000 0000
//
//	
//  on interrupt, set flag that we have stuff from SPI UART, 
//  if not in use by accelerometer polling -> immediately write 0x0000 to read the incoming byte
//	
//  
//
///////////////////////////////////////////////////////////////////



///////////////////////////////////////////////////////////////////
// GPS Code
///////////////////////////////////////////////////////////////////
char GPSRecvBytes;
char __attribute__((aligned(16))) GPSSaveString[50];
char *GPSRecvBuffer;
char *GPSRecvPtr;
char GPSHasSatLock;
char GPSLoggedFirstDatetime;
char GPSCare = 0;
char GPSCommaCount = 0;
char GPSPeriod = 0;
char GPSChecksum = 0;

void __attribute__((__interrupt__)) _U2TXInterrupt(void){  
    IFS1bits.U2TXIF = 0;
	
} 

//IMPL Implement protection against duplicates  by using timestamps

void __attribute__((__interrupt__)) _U2RXInterrupt(void){
    char ch;
    
	while( DataRdyUART2()){
        ch = ReadUART2();
		if(!GPSCare) continue;

		if(GPSRecvBytes >= 42) continue;
		if(GPSCare == 1){
			if(ch=='$') GPSCare = 2;
			continue;			
		}
		if(GPSCare == 2){
			if(ch=='*'){
				GPSCare = 3;
				continue;
			}
			GPSChecksum ^= ch;
		}
		if(GPSCare == 3){
			GPSChecksum ^= ASCIIHexToChar(ch) << 4;
			GPSCare = 4;
			continue;
		}
		if(GPSCare == 4){
			GPSChecksum ^= ASCIIHexToChar(ch);
			if(GPSChecksum == 0){
				GPSCare = 5;
			}else{
				GPSCare = 0;
			}
			continue;
		}

		if(ch==','){
			GPSCommaCount += 1;
			GPSPeriod = 0;
			continue;
		}
		if(ch=='*'){
			GPSCare = 3;
		}
		if(ch=='.') GPSPeriod = 1;

		if(GPSCommaCount == 2){
			if(ch == 'A') GPSHasSatLock = 1;
			if(ch == 'V'){
				GPSHasSatLock = 0;
				GPSCare = 0;  //IMPL but the data will still be coming for the rest of line, handle this
				continue;
			}
		}

		if(GPSLoggedFirstDatetime==0){
			if((GPSCommaCount == 1 && !GPSPeriod) || GPSCommaCount == 9){
				( *(GPSRecvPtr)++) = ch;
				GPSRecvBytes += 1;
			}
		}else{
			if(GPSCommaCount >= 3 && GPSCommaCount <=7){
				( *(GPSRecvPtr)++) = ch;
				GPSRecvBytes += 1;
			}
		}
			
			
	}

    IFS1bits.U2RXIF = 0;
		
}   


void GPS_Init(void){
	GPSRecvBuffer = GPSSaveString + 8;
	GPSRecvPtr = GPSRecvBuffer;
	GPSCare = 0;
	GPSRecvBytes = 0;
	GPSHasSatLock = 0;
	GPSLoggedFirstDatetime = 0;

	GPSSaveString[0] = 0xEE;
	GPSSaveString[6] = 'G';
	GPSSaveString[7] = 'T';

	ConfigIntUART2(UART_RX_INT_EN & UART_RX_INT_PR6 & 
                   UART_TX_INT_DIS & UART_TX_INT_PR2);
	OpenUART2(0x8008, 0x8400, 833);  

	putsUART2((unsigned int *)"$PSRF100,1,38400,8,1,0*3D\r\n\0");  // GPS Defaults to 4800 baud, increase it
	Timer_WaitMS(200);
	CloseUART2();
	
	ConfigIntUART2(UART_RX_INT_EN & UART_RX_INT_PR6 & 
                   UART_TX_INT_DIS & UART_TX_INT_PR2);
	OpenUART2(0x8008, 0x8400, 104);     //Without loopback
	
	putsUART2((unsigned int *)"$PSRF103,00,00,00,01*24\r\n\0");	Timer_WaitMS(50);
	putsUART2((unsigned int *)"$PSRF103,01,00,00,01*25\r\n\0");	Timer_WaitMS(50);
	putsUART2((unsigned int *)"$PSRF103,02,00,00,01*26\r\n\0");	Timer_WaitMS(50);
	putsUART2((unsigned int *)"$PSRF103,03,00,00,01*27\r\n\0");	Timer_WaitMS(50);
	putsUART2((unsigned int *)"$PSRF103,04,00,00,01*20\r\n\0");	Timer_WaitMS(50);
	putsUART2((unsigned int *)"$PSRF103,05,00,00,01*21\r\n\0");	Timer_WaitMS(50);
	//putsUART2((unsigned int *)"$PTNLSRT,C*3C\r\n\0"); Timer_WaitMS(100);	
	putsUART2((unsigned int *)"$PTNLSRT,W*28\r\n\0"); Timer_WaitMS(100);
	//putsUART2((unsigned int *)"$PTNLSRT,H*37\r\n\0"); Timer_WaitMS(100);
	
}

void GPSRequest(void){
	if(GPSCare != 5 && GPSCare != 0) return;
	GPSCare = 1;
	GPSCommaCount = 0;	
	GPSPeriod = 0;
	GPSChecksum = 0;
	GPSRecvBytes = 0;
	GPSRecvPtr = GPSRecvBuffer;
	putsUART2((unsigned int *)"$PSRF103,04,01,00,01*21\r\n\0");
}


///////////////////////////////////////////////////////////////////



void __attribute__((__interrupt__)) _U1TXInterrupt(void){  
    IFS0bits.U1TXIF = 0;
} 

void __attribute__((__interrupt__)) _U1RXInterrupt(void){
    char ch;
    while( DataRdyUART1()){
        if(!UART1Accept) continue;
		if(UART1RecvBytes-1 > COMBUFF) continue; //IMPL - raise error flag or something like that - overflow
        ch = ReadUART1();
        //WriteUART2(ch);
		 
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

    IFS0bits.U1RXIF = 0;

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
	UART1SaveString[0] = 0xEE;
	UART1SaveString[6] = 'O';
	UART1SaveString[7] = 'B';

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
	UART1RecvBuffer = UART1SaveString + 8;
	UART1RecvPtr = UART1RecvBuffer;

	ConfigIntUART1(UART_RX_INT_EN & UART_RX_INT_PR6 & 
                   UART_TX_INT_DIS & UART_TX_INT_PR2);
	OpenUART1(0x8008, 0x8400, 104);     //Without loopback

	/*
	ConfigIntUART2(UART_RX_INT_EN & UART_RX_INT_PR6 & 
                   UART_TX_INT_DIS & UART_TX_INT_PR2);
	OpenUART2(0x8008, 0x8400, 104);     //Without loopback
	*/
}



void IO_Init(void){
    TRISA = 0x8000;
	PORTA = 0x0000;

	TRISD = 0xFFFF;

	// There will be another chip select for the UART3 we will put on SPI2 bus -> this is RG1 (0x0002)	
	TRISG = 0xF1CD;  // This is the chip select for the accelerometer on SPI2 bus and chip select for UART3
	PORTG = 0x0202;  //
}


int main(void){
	long int time_ms = 0;

	IO_Init();
	UART_Init();
	Timer_Init(); 
	Timer_WaitMS(100);
	Accel_Init();
	SPIUART_Init();
	ELM_Init();
	GPS_Init();
	
	/*
	PORTA = 0x0000;
	Timer_WaitMS(100);
	while(1){
		GPSCare = 0;
		GPSRequest();
		Timer_WaitMS(1000);		
		if(GPSCare == 5){
			PORTA = PORTA ^ 0x0010; 
		}
	}*/

	/*
	unsigned char cur;
	while(1){
		//Timer_WaitMS(500);
		SPIUART_Send("Why Hallo There!\r\n", 16);
		if(hasChar(&SPIUART_BufIn)){
			cur = getChar(&SPIUART_BufIn);
			SPIUART_Send(&cur, 1);
		}
		//PORTG   = 0x0200;
		//SPI2BUF = 0x8058;   //  
		//SPI2BUF = 0x4000;
		//PORTA = (PORTA & 0xFF00) | (PORTA >> 15);
	}
	*/
	char has_media = 0x00;

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
    if (FSfwrite("\xEE\x06\0\0\0\0NS", 1, 8, logFile) != 8) while(1);	
	SPIUART_Send("\xEE\x06\0\0\0\0NS", 8);


	int  pid_write = 0;
	char pid_dowrite = 0;
	int pid = 1;
	int doScanning = 1;
	unsigned int pass = 0;
	unsigned char ch;
	long int lastGPSRequestTime = 0;
	while(doScanning){

		if(!has_media && MDD_MediaDetect()){
			
		}		

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
		if(!ELM_Wait(2000)){
		
		}else{
			Accel_BeginUpdateSaveString();  
			time_ms = Timer_GetTimeMS();
			*((long int *)(accelSaveString+2)) = time_ms;
			Accel_WaitUpdated(); //IMPL if MSB start with 011 , data overflow in +
								 //IMPL if MSB start with 100 , data overflow in -
			if (FSfwrite (accelSaveString, 1, ACCEL_PACKET_LENGTH, logFile) != ACCEL_PACKET_LENGTH) while(1);			
			SPIUART_Send(accelSaveString, ACCEL_PACKET_LENGTH);
		}
		
		if(!(PORTD & 0x0040)) has_media |= 0x02;

		
		

		if((time_ms - lastGPSRequestTime) > 1000){
			lastGPSRequestTime = time_ms;
			GPSCare = 0;
			GPSRequest();
		}
		if(GPSCare == 5){
			PORTA = PORTA ^ 0x0008;
			GPSSaveString[1] = GPSRecvBytes+6;
			*((long int *)(GPSSaveString+2)) = time_ms;
			if (FSfwrite (GPSSaveString, 1, GPSRecvBytes+8, logFile) != GPSRecvBytes+8) while(1);	
			SPIUART_Send(GPSSaveString, GPSRecvBytes+8);
			if(!GPSLoggedFirstDatetime){
				GPSLoggedFirstDatetime = 1;
				GPSSaveString[7] = 'P';
			}
			GPSCare = 0;
		}
		
		/*
		UART1SaveString[1] = UART1RecvBytes+6;
		*((long int *)(UART1SaveString+2)) = time_ms;
		if (FSfwrite (UART1SaveString, 1, UART1RecvBytes+8, logFile) != UART1RecvBytes+8) while(1);	
		SPIUART_Send(UART1SaveString, UART1RecvBytes+8);
		*/

		UART1SaveString[1] = UART1RecvBytes+4;
		*((long int *)(UART1SaveString+2)) = time_ms;
		if (FSfwrite (UART1SaveString, 1, 8, logFile) != 8) while(1);	
		if (FSfwrite ((UART1SaveString+10), 1, UART1RecvBytes-2, logFile) != UART1RecvBytes-2) while(1);	
		SPIUART_Send(UART1SaveString, 8);
		SPIUART_Send(UART1SaveString+10, UART1RecvBytes-2);

		while(hasChar(&SPIUART_BufIn)){
			PORTA = PORTA ^ 0x0002;
			ch = getChar(&SPIUART_BufIn);
			if(pid_dowrite){
				ELM_Sensors[pid_write] = ch;
				pid_write += 1;
				if(pid_write == SENSORS){
					pid_dowrite = 0;
				}
			}else if(ch == 'r'){
				SPIUART_Send("\xEE\x47\0\0\0\0CF", 8);
				SPIUART_Send(ELM_Sensors, SENSORS);
			}else if(ch == 'w' && pid_dowrite == 0){
				pid_dowrite = 1;
				pid_write   = 0;
			}
		}

		pid += 1;
	}
    
	PORTA = 0x00FF;
	if (FSfclose (logFile)) while(1);
	PORTA = 0x00AA;
	while(1);

	CloseUART1();   
	CloseUART2();
}








