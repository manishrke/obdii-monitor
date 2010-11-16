#define FCY 4000000UL

#include "FSIO.h"
#include "uart.h"

int  runs = 0;
char prompt = 0;
char   UART1RecvBuffer[32];
char * UART1RecvPtr = UART1RecvBuffer;
char   UART1RecvBytes = 0;
FSFILE *logFile;

const char ELM_RESET_STR[] = "ATZ\r\0";
const char ELM_ECHO_OFF[] = "ATE0\r\0";
const char ELM_SPACES_OFF[] = "ATS0\r\0";
const char ELM_RETURN_OFF[] = "ATL0\r\0";
const char ELM_SENSOR_RPM[] = "010C1\r\\0";

_CONFIG2(IESO_OFF & FNOSC_PRIPLL & FCKSM_CSDCMD & OSCIOFNC_OFF & POSCMOD_HS)
_CONFIG1(JTAGEN_OFF & GCP_OFF & GWRP_OFF & COE_OFF & ICS_PGx2 & FWDTEN_OFF & BKBUG_ON)


void __attribute__((__interrupt__)) _U1TXInterrupt(void){  
    IFS0bits.U1TXIF = 0;
} 

void __attribute__((__interrupt__)) _U1RXInterrupt(void){
    char ch;
    IFS0bits.U1RXIF = 0;
    while( DataRdyUART1()){
        ch = ReadUART1();
        WriteUART2(ch);
 
        if(ch=='>') prompt = 1;
        ( *(UART1RecvPtr)++) = ch;
		UART1RecvBytes += 1;
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
        PORTA = ch;
        WriteUART1(ch);
    } 
}  

void delay(void){
	int i = 0, j = 0;
   	while(i<2000){
   	    j = 0;
   	    while(j<2000){
   	        j += 1;
   	    }
   	    i += 1;
   	}
}

int main (void){
   TRISA = 0x0000;
   PORTA = 0x0000;
   TRISD = 0xFFFF;

   ConfigIntUART1(UART_RX_INT_EN & UART_RX_INT_PR6 & 
                  UART_TX_INT_DIS & UART_TX_INT_PR2);
   OpenUART1(0x8008, 0x8400, 104);     //Without loopback


   ConfigIntUART2(UART_RX_INT_EN & UART_RX_INT_PR6 & 
                  UART_TX_INT_DIS & UART_TX_INT_PR2);
   OpenUART2(0x8008, 0x8400, 104);     //Without loopback

   short int xk = 0;

   PORTA = 0x00AA;
   while(xk<6){
      PORTA = xk;
      UART1RecvBytes = 0;
      UART1RecvPtr   = UART1RecvBuffer;

      putsUART1((unsigned int *)ELM_RESET_STR);
      delay();
      xk += 1;
   }
   

   putsUART1((unsigned int *)ELM_ECHO_OFF);
   delay();
   UART1RecvBytes = 0;
   UART1RecvPtr   = UART1RecvBuffer;
   putsUART1((unsigned int *)ELM_SPACES_OFF);
   delay();
   UART1RecvBytes = 0;
   UART1RecvPtr   = UART1RecvBuffer;
   putsUART1((unsigned int *)ELM_RETURN_OFF);
   delay();
   UART1RecvBytes = 0;
   UART1RecvPtr   = UART1RecvBuffer;

   PORTA = 0x00FF;
   while (!MDD_MediaDetect());

   UART1RecvBytes = 0;
   UART1RecvPtr   = UART1RecvBuffer;

   // Initialize the library
   while (!FSInit());

   logFile = FSfopen ("RPM.TXT", "w");
   if (logFile == NULL) while(1);

	delay();

   while(1){
       while(!prompt){
           if(!(PORTD & 0x0040)){
			   if (FSfwrite (UART1RecvBuffer, 1, UART1RecvBytes, logFile) != UART1RecvBytes) while(1);
		       UART1RecvBytes = 0;
			   UART1RecvPtr   = UART1RecvBuffer;
               goto exitdesu;
           }
       }
       prompt = 0;

	   if (FSfwrite (UART1RecvBuffer, 1, UART1RecvBytes, logFile) != UART1RecvBytes) while(1);
       UART1RecvBytes = 0;
	   UART1RecvPtr   = UART1RecvBuffer;
	   PORTA = UART1RecvBytes;
       
	   if(!(PORTD & 0x0040)) break;
	   putsUART1((unsigned int *)ELM_SENSOR_RPM);
	
	   runs += 1;

   }
exitdesu:
   PORTA = 0x00FF;

   if (FSfclose (logFile)) while(1);
   PORTA = 0x00AA;
   while(1);

   CloseUART1();   
}








