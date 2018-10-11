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

int messagingService::findFrameLimitersInBuffer(byte token)
{
	//First loop to find the start of the message 
	for(int readPosition = 0; readPosition < dataReceived - 1; ++readPosition)
	{
		if ( readByte(readPosition) == token && readByte ( readPosition + 1 ) == token )
		{
            //Frame found! Increment the index so that it would point at the second byte in the frame.
			return readPosition + 1;;
		}
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
	if( dataReceived < constants.minimumMessageLength )
	{
        return nullptr;
    }

	//Find the start of the frame, returns a 0 based index, where 0 means that that the read would happen at currentReadLocation.
    //The returned value will point at the 2nd byte in the frame.
	int readPosition = findFrameLimitersInBuffer(startOfFrame);
	
	//If a negative value is returned it means no frame was found and there's nothing to do.
	if( readPosition < 0)
		return nullptr;
	
    //Advance past the start of frame.
	readPosition  += 1;
	
	//Attempt reading the data length of the message.
	if(!readByte(readPosition, tempMessage.dataLength))
	{
		char charBuffer[60];
		sprintf(charBuffer, "Failed length read: Idx: %d,Rx:%d", readPosition, dataReceived);
		Serial.println(charBuffer);	
		return nullptr;
	}

	readPosition += 1;

	//Check if the full message has been received
	//TODO: this condition needs work, somehow expected ends up being one too low?
	//What's this + 2??
	if((tempMessage.dataLength + readPosition + 1 /*+ 2 + dataRead*/ ) > dataReceived)
	{
		char charBuffer[30];
		sprintf(charBuffer, "Expected:%d,Rx:%d", tempMessage.dataLength + readPosition + 1, dataReceived);
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

 		//Disard the data since it's probably corrupted.
 		discardData(readPosition + 1);

		return nullptr;
	}

    //Attempt to read the message ID (not sure why the readPosition isn't getting incremented here)
	if(!readByte(readPosition, tempMessage.id))
		return nullptr;

	readPosition += 1;
	
	//And now read the data that the message was carrying
	for(int i = 0; i < tempMessage.dataLength ; ++i)
	{
		//If we run out of data to be read at any point (i.e. we haven't received all of it)
		//Just return null, we can try again later.
		if(!readByte(readPosition, tempMessage.messageData[i]))
			return nullptr;

        //Note that this will end up advancing past the last read location
        //on the last cycle of the loop.
		readPosition += 1;
	}

	char charBuffer[40];
	sprintf(charBuffer, "Msg Parsed. DataRx:%d, dataRead:%d", dataReceived, readPosition);
	Serial.println(charBuffer);

    //Advance internal tracking past the data we've already processed.
    //readPosition is equivalent to the amount of data read.
	discardData(readPosition);

	return &tempMessage;
}

void messagingService::discardData(int dataRead)
{
    dataReceived -= dataRead;

    //Keep track of where we left off reading the buffer.
    currentReadLocation = (currentReadLocation + dataRead) % constants.bufferSize;
}

bool messagingService::readByte(int readPosition, byte& outputByte)
{
	//Check whether we've received enough data to do this read.
	if(readPosition >= dataReceived)
		return false;

    //Now call the unsafe readByte overload.
	outputByte = readByte(readPosition);
	return true;	
}

byte messagingService::readByte(int readPosition)
{
	return messageBuffer[(currentReadLocation + readPosition) % constants.bufferSize];
}