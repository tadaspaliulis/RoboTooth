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
  		//If looped around the whole buffer, ove rwritte data at the beginning
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
	if( dataReceived >= constants.minimumMessageLength )
	{

		//Serial.println("Processing message\n");
		int dataRead = 0;

		int startOfMessage = findFrameLimitersInBuffer(startOfFrame, currentReadLocation, dataRead);
		if( startOfMessage < 0)
			return nullptr;

		int readPosition  = 1;
		
		//tempMessage.dataLength = readByte(startOfMessage + 1);
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
		if((tempMessage.dataLength + 2 + dataRead ) >= dataReceived)
		{
			char charBuffer[30];
  			sprintf(charBuffer, "Expected:%d,Rx:%d", tempMessage.dataLength + 2 + dataRead, dataReceived);
			Serial.println(charBuffer);
			return nullptr;
		}


		//Message longer than expected
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
		if(!readByte(startOfMessage + readPosition, tempMessage.id))
			return nullptr;

		readPosition += 1;

		for(int i = 0; i < tempMessage.dataLength ; ++i)
		{
			//tempMessage.messageData[i] = readByte(startOfMessage + 3 + i);
			if(!readByte(startOfMessage + readPosition, tempMessage.messageData[i]))
				return nullptr;

			readPosition += 1;
		}

		dataRead += readPosition;
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
	if(readPosition - currentReadLocation >= dataReceived)
		return false;

	outputByte = messageBuffer[readPosition % constants.bufferSize];
	return true;	
}


byte messagingService::readByte(int readPosition)
{
	return messageBuffer[readPosition % constants.bufferSize];
}
