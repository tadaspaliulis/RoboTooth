#include "messagingService.h"


const byte messagingService::startOfFrame = 0xbb;

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

	while ( Serial.available() > 0)
  	{
  		//Serial.write("Receive incoming data\n");
  		//If looped around the whole buffer, ove rwritte data at the beginning
  		if(currentWriteLocation == constants.bufferSize)
  			currentWriteLocation = 0;
  		
    	messageBuffer[currentWriteLocation] = Serial.read();
    	//Serial.write(messageBuffer[currentWriteLocation]);
    	++currentWriteLocation;
    	++dataReceived;
  }

  totalDataReceived += dataReceived;
}

int messagingService::findFrameLimitersInBuffer(byte token, int readStartPosition, int &dataRead)
{
	//Serial.write("Looking for frames\n");
	int i = readStartPosition;
	//First loop to find the start of the message 
	for(; dataRead != dataReceived; ++dataRead)
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
	Serial.write(sendBuffer, 2 + 1 + 1 + messageDataSize);
}

void messagingService::sendMessage(message& msg)
{
	sendMessage(msg.messageData, msg.dataLength, msg.id);
}

message* messagingService::processMessage()
{
	if( dataReceived >= constants.minimumMessageLength )
	{

		//Serial.write("Processing message\n");
		int startOfMessage = -1;

		int dataRead = 0;

		startOfMessage = findFrameLimitersInBuffer(startOfFrame, currentReadLocation, dataRead);
		if( startOfMessage < 0)
			return NULL;

		
		tempMessage.dataLength = readByte(startOfMessage + 1);

		//Check if the full message has been received
		if(tempMessage.dataLength  + 4 >= dataReceived)
			return NULL;


		//Message longer than expected
		if( tempMessage.dataLength > constants.maximumMessageDataLength )
		{
			//char buffertextmessage[50];
  			//sprintf(buffertextmessage, "Message length too large: %d", tempMessage.dataLength );
 	 		//Serial.println(buffertextmessage);
			
			return NULL;
		}

		//currentReadLocation = startOfMessage + 2;
		tempMessage.id = readByte( startOfMessage + 2 );
		 
		for(int i = 0; i < tempMessage.dataLength ; ++i)
		{
			tempMessage.messageData[i] = readByte(startOfMessage + 3 + i);
		}

		currentReadLocation += dataRead % constants.bufferSize;
		dataReceived -= dataRead;
		//Serial.write("Message received\n");
		return &tempMessage;
	}

	else return NULL;
}


byte messagingService::readByte(int readPosition)
{
	return messageBuffer[readPosition % constants.bufferSize];
}
