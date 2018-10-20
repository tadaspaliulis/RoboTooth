#include "serialInterfaceMock.h"
#ifdef VS_TESTBED
#include <memory>
#include "messagingService.h"
#include "constants.h"
byte serialInterfaceMock::read()
{
	auto result = bytesQueue.front();
	bytesQueue.pop();
	return result;
}

void serialInterfaceMock::addMessageBytesForReading(const message& msg)
{
	addMessagePreamble();

	addByte(msg.dataLength);
	addByte(msg.id);
	for (int i = 0; i < constants.maximumMessageDataLength && i < msg.dataLength; ++i)
	{
		addByte(msg.messageData[i]);
	}
}

void serialInterfaceMock::addMessagePreamble()
{
	//Preamble (value shouldn't be hardcoded... deal with that in due time.
	addByte(0xbb);
	addByte(0xbb);
}

void serialInterfaceMock::addByte(byte b)
{
	bytesQueue.push(b);
}

#endif