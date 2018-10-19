#pragma once
#include "constants.h"
#include "queue.h"

/**** Message format ***/
/* preamble/start of frame (2 bytes) + data length (1 byte) + messageId ( 1 byte ) + data( x bytes )
*/
struct message
{
	byte dataLength;
	byte id;
	byte messageData[constants.maximumMessageDataLength];
};


//Singleton for communicating with the application
class messagingService
{
  public:
    static messagingService* getMessenger();

    void initialise();

    //Send a message to the master
   	void sendMessage(byte* messageData, byte messageDataSize, byte messageId );
    void sendMessage(message& msg);

   	void receiveIncomingData();

   	message* processMessage();

  private:
    messagingService();

    static messagingService* messengerSingleton;

  	const static byte startOfFrame;

    const static int bufferSize;

    //Buffer for received data
    
    byte sendBuffer[constants.sendBufferSize];
    
	queue<byte> inboundDataQueue;

    message tempMessage;

    //Helper functions
    //Looks for 2 instances of token in a row, returns -1 if not found
    int findFrameLimitersInBuffer(byte token);

    void discardData(int byteCount);

    //Reads the specific byte in the buffer, applies mod to make sure index is never out of bounds
    byte readByte(int readPosition);

    bool readByte(int readPosition, byte& outputByte);
};

