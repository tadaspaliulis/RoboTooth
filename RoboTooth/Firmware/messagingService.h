#pragma once
#include "constants.h"
#include "queue.h"
#include "message.h"

//Singleton for communicating with the application
template<class SerialInterface>
class messagingService
{
  public:
    static messagingService* getMessenger();

	messagingService(SerialInterface* serialInterface);

    void initialise();

    //Send a message to the master
   	void sendMessage(byte* messageData, byte messageDataSize, byte messageId );
    void sendMessage(message& msg);

   	void receiveIncomingData();

   	message* processMessage();

  private:
	SerialInterface* serialInterface;

    static messagingService* messengerSingleton;

  	const static byte startOfFrame;

	//This is a very awkward workound, but for some reason
	//arduino compiler started to refuse to compile with sendBufferSize
	//used directly after messagingService became a template class.
    const static int bufferSize = constants.sendBufferSize;
    byte sendBuffer[bufferSize];
    
	//Buffer for received data.
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

#pragma region Implementations

template<class SerialInterface>
const byte messagingService<SerialInterface>::startOfFrame = 0xbb;

template<class SerialInterface>
messagingService<SerialInterface>* messagingService<SerialInterface>::messengerSingleton = nullptr;

template<class SerialInterface>
messagingService<SerialInterface>* messagingService<SerialInterface>::getMessenger()
{
	if(messengerSingleton == nullptr)
		messengerSingleton = new messagingService(new SerialInterface());

	return messengerSingleton;
}

template<class SerialInterface>
messagingService<SerialInterface>::messagingService(SerialInterface* serialInterface) : inboundDataQueue(constants.bufferSize),
	serialInterface(serialInterface)
{
}

template<class SerialInterface>
void messagingService<SerialInterface>::initialise()
{
	//Set BAUD rate
	serialInterface->begin( 9600 );
  	//Set up Wifi connection name
  	serialInterface->write("AT+NAMERoboTooth");
  	delay(400);
}

template<class SerialInterface>
void messagingService<SerialInterface>::receiveIncomingData()
{	
	char output[6];
	while ( serialInterface->available() > 0)
    {
  		//Serial.write("Receive incoming data\n");
		inboundDataQueue.tryAdd(serialInterface->read());
		sprintf(output, "%x", *inboundDataQueue.last());
    	serialInterface->println(output);
    }
}

template<class SerialInterface>
int messagingService<SerialInterface>::findFrameLimitersInBuffer(byte token)
{
	//First loop to find the start of the message 
	for(int readPosition = 0; readPosition < inboundDataQueue.getSize() - 1; ++readPosition)
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

template<class SerialInterface>
void messagingService<SerialInterface>::sendMessage(byte* messageData, byte messageDataSize, byte messageId )
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

template<class SerialInterface>
void messagingService<SerialInterface>::sendMessage(message& msg)
{
	sendMessage(msg.messageData, msg.dataLength, msg.id);
}

template<class SerialInterface>
message* messagingService<SerialInterface>::processMessage()
{
	//Check if we've received enough data for there to be at least one message.
	if( inboundDataQueue.getSize() < constants.minimumMessageLength )
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
		sprintf(charBuffer, "Failed length read: Idx: %d,Rx:%d", readPosition, inboundDataQueue.getSize());
		serialInterface->println(charBuffer);	
		return nullptr;
	}

    //Advance past the data length field.
	readPosition += 1;

    //Check if the received message is longer than expected.
    //Might indicate an error in processing code or a corruption during transmission.
    if( tempMessage.dataLength > constants.maximumMessageDataLength )
    {
        char buffertextmessage[50];
        sprintf(buffertextmessage, "Msg too long:%d", tempMessage.dataLength );

        serialInterface->println(buffertextmessage);

        //TODO: Add this back in
        //SendStringToApp(buffertextmessage);

        //Disard the data since it's probably corrupted.
        discardData(readPosition + 1);

        return nullptr;
    }

	//Check if the full message has been received.
	if((tempMessage.dataLength + readPosition + 1) > inboundDataQueue.getSize())
	{
		char charBuffer[30];
		sprintf(charBuffer, "Expected:%d,Rx:%d", tempMessage.dataLength + readPosition + 1, inboundDataQueue.getSize());
		serialInterface->println(charBuffer);
		return nullptr;
	}

    //Attempt to read the message ID.
	if(!readByte(readPosition, tempMessage.id))
		return nullptr;

    //Advance past the ID field.
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
	sprintf(charBuffer, "Msg Parsed. DataRx:%d, dataRead:%d", inboundDataQueue.getSize(), readPosition);
	serialInterface->println(charBuffer);

    //Advance internal tracking past the data we've already processed.
    //readPosition is equivalent to the amount of data read.
	discardData(readPosition);

	return &tempMessage;
}

template<class SerialInterface>
void messagingService<SerialInterface>::discardData(int dataRead)
{
	inboundDataQueue.popFront(dataRead);
}

template<class SerialInterface>
bool messagingService<SerialInterface>::readByte(int readPosition, byte& outputByte)
{
	auto incomingByte = inboundDataQueue.get(readPosition);
	if(incomingByte == nullptr)
		return false;

	outputByte = *incomingByte;
	return true;	
}

template<class SerialInterface>
byte messagingService<SerialInterface>::readByte(int readPosition)
{
	//No protections here for nullptr referencing. Beware!
	return *inboundDataQueue.get(readPosition);
}

#pragma endregion