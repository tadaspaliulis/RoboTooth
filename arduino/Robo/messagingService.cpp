#include "messagingService.h"
#include "Debug.h"
#include <stdio.h>

#define MESSENGER_DEBUG

const byte messagingService::startOfFrame = 0xbb;

messagingService* messagingService::messengerSingleton = nullptr;

messagingService* messagingService::getMessenger()
{
	if(messengerSingleton == nullptr)
		messengerSingleton = new messagingService();

	return messengerSingleton;
}

messagingService::messagingService() : dataReceived ( 0 ), currentReadLocation ( 0 ), currentWriteLocation ( 0 ), totalDataReceived ( 0 ) 
{
	
}

void messagingService::initialise()
{
	//Set BAUD rate
	Serial.begin( 9600 );
  	//Set up Wifi connection name
  	Serial.write("AT+NAMERoboTooth");
  	delay(400);
}

void messagingService::receiveIncomingData()
{	
	//Reset data since last processing
	//dataReceived = 0;
	char output[6];
	while ( Serial.available() > 0)
  	{
  		//Serial.write("Receive incoming data\n");
  		//If looped around the whole buffer, overwritte data at the beginning
  		if(currentWriteLocation == constants.bufferSize)
  			currentWriteLocation = 0;
  		
    	messageBuffer[currentWriteLocation] = Serial.read();
    	sprintf(output, "%x", messageBuffer[currentWriteLocation]);
    	Serial.println(output);
    	++currentWriteLocation;
    	++dataReceived;
  }

  totalDataReceived += dataReceived;
}

int messagingService::findFrameLimitersInBuffer(byte token, int readStartPosition, int &dataRead)
{
	//Serial.write("Looking for frames\n");
	
	//First loop to find the start of the message 
	for(int i = readStartPosition; dataRead < dataReceived - 1; ++dataRead)
	{
		if ( readByte(i) == token && readByte ( i + 1 ) == token )
		{
			//Serial.write("Frame found\n");
			++dataRead; //Another byte read
			return (i + 1) % constants.bufferSize; //Frame found! Make sure it's not pointing out of bounds
		}

		++i;
	}

	//Identifier not found, return -1 as to indicate the failure
	return -1;
}

void messagingService::sendMessage(byte* messageData, byte messageDataSize, byte messageId )
{
	//Indicate the start of the message
	sendBuffer[0] = sendBuffer[1] = startOfFrame;
	//First byte after preamble indicates how much data there will be in the message
	sendBuffer[2] = messageDataSize;
	//Second byte after preamble indicates the message Id
	sendBuffer[3] = messageId;

	//Finally it's the actual data
	memcpy(sendBuffer + 4, messageData, messageDataSize);
	//And then SEND!!
	//Serial.write(sendBuffer, 2 + 1 + 1 + messageDataSize);
}

void messagingService::sendMessage(message& msg)
{
	sendMessage(msg.messageData, msg.dataLength, msg.id);
}

message* messagingService::processMessage()
{
	//Check if we've received enough data for there to be at least one message.
	if( dataReceived >= constants.minimumMessageLength )
	{

		//Serial.println("Processing message\n");
		int dataRead = 0;

		//Find the start of the frame, returns an index within the data buffer which points to the end of the frame.
		//Note that the return value here is using the real indexes, and the readPosition then gets added onto that.

		//This should probably return the offset from the currentReadLocation instead, letting the readByte functions then handle addition as well.
		//The code would probably be more comprehensible.
		int startOfMessage = findFrameLimitersInBuffer(startOfFrame, currentReadLocation, dataRead);
		
		//If a negative value is returned it means no frame was found and there's nothing to do.
		if( startOfMessage < 0)
			return nullptr;

		
		int readPosition  = 1;
		
		//tempMessage.dataLength = readByte(startOfMessage + 1);
		//Attempt reading 
		if(!readByte(startOfMessage + readPosition, tempMessage.dataLength))
		{
			char charBuffer[60];
  			sprintf(charBuffer, "Failed length read: Idx: %d,Rx:%d", startOfMessage + readPosition, dataReceived);
			Serial.println(charBuffer);	
			return nullptr;
		}

		readPosition += 1;

		//Check if the full message has been received
		//dataLength = 4, readPosition = 2, startOfMessage = 1,
		//TODO: this condition needs work, somehow expected ends up being one too low?
		//What's this + 2??
		if((tempMessage.dataLength + 2 + dataRead ) >= dataReceived)
		{
			char charBuffer[30];
  			sprintf(charBuffer, "Expected:%d,Rx:%d", tempMessage.dataLength + 2 + dataRead, dataReceived);
			Serial.println(charBuffer);
			return nullptr;
		}


		//Check if the received message is longer than expected
		//Might indicate an error in processing code or a corruption during transmission.
		if( tempMessage.dataLength > constants.maximumMessageDataLength )
		{
			char buffertextmessage[50];
  			sprintf(buffertextmessage, "Msg too long:%d", tempMessage.dataLength );

 	 		Serial.println(buffertextmessage);

 	 		//TODO: Add this back in
 	 		//SendStringToApp(buffertextmessage);

 	 		//Disard the data since it's probably corrupted
 	 		discardData(dataRead);

			return nullptr;
		}

		//currentReadLocation = startOfMessage + 2;
		//tempMessage.id = readByte( startOfMessage + 2 );
		//Attempt to read the message ID (not sure why the readPosition isn't getting incremented here)
		if(!readByte(startOfMessage + readPosition, tempMessage.id))
			return nullptr;

		readPosition += 1;
		
		//And now read the data that the message was carrying
		for(int i = 0; i < tempMessage.dataLength ; ++i)
		{
			//tempMessage.messageData[i] = readByte(startOfMessage + 3 + i);
			//If we run out of data to be read at any point (i.e. we haven't received all of it)
			//Just return null, we can try again later.
			if(!readByte(startOfMessage + readPosition, tempMessage.messageData[i]))
				return nullptr;

			readPosition += 1;
		}

		dataRead += readPosition;

		//Keep track of where we left off reading the buffer.
		currentReadLocation = (currentReadLocation + dataRead) % constants.bufferSize;

		char charBuffer[40];
		sprintf(charBuffer, "Msg Parsed. DataRx:%d, dataRead:%d", dataReceived, dataRead);
		Serial.println(charBuffer);

		discardData(dataRead);

		return &tempMessage;
	}

	else return NULL;
}

void messagingService::discardData(int byteCount)
{
	dataReceived -= byteCount;
}

bool messagingService::readByte(int readPosition, byte& outputByte)
{
	//Check whether we've received enough data to do this read.
	//Read position is essentially currentReadLocation + offset from the starting point.
	if(readPosition - currentReadLocation >= dataReceived)
		return false;

	outputByte = messageBuffer[readPosition % constants.bufferSize];
	return true;	
}


byte messagingService::readByte(int readPosition)
{
	return messageBuffer[readPosition % constants.bufferSize];
}
