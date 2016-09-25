#pragma once
#include "constants.h"

/**** Message format ***/
/* preamble/start of frame (2 bytes) + data length (1 byte) + messageId ( 1 byte ) + data( x bytes )
*/
struct message
{
	byte dataLength;
	byte id;
	byte messageData[24];
};


class messagingService
{
  public:
    messagingService();

    void initialise();

    //Send a message to the master
   	void sendMessage(byte* messageData, byte messageDataSize, byte messageId );
    void sendMessage(message& msg);

   	void receiveIncomingData();

   	message* processMessage();

  private:

  	const static byte startOfFrame;
  	const static byte endOfFrame;

    const static int bufferSize;

    //Buffer for received data
    byte messageBuffer[constants.bufferSize];
    byte sendBuffer[constants.sendBufferSize];
    int dataReceived;
    int currentReadLocation;
    int currentWriteLocation;

    message tempMessage;

    //State tracking

    //Data Received since session start 
    long totalDataReceived; //(Bytes)

    //Number of received messages what were not finished or didn't match the defined data length
    int mangledMessages;

    //Helper functions
    //Looks for 2 instances of token in a row, returns -1 if not found
    int findFrameLimitersInBuffer(byte token, int readStartPosition, int &dataRead);

    //Reads the specific byte in the buffer, applies mod to make sure index is never out of bounds
    byte readByte(int readPosition);
};

