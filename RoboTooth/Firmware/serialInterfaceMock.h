#pragma once
#include "ArduinoFunctionallity.h"
#ifdef VS_TESTBED
#include <queue>
#include "messagingService.h"

//Mocks arduino serial interface.
class serialInterfaceMock
{
public:
	//Doesn't need to do anything in a mocked environment.
	void begin(int x) {}

	int available() { return bytesQueue.size(); }

	byte read();

	//Does nothing for now.
	void println(const char* string) {}

	//Does nothing for now.
	void write( const char* string) {}

	void addByte(byte b);

	//Adds the message to the queue to be read by serial interface consumers.
	void addMessageBytesForReading(const message& msg);

	//Adds the message to the queue to be read by serial interface consumers.
	//Uses the provided data length when copying the data instead of the one in the message.
	void addMessageBytesForReading(const message& msg, byte dataLengWhenCopying);

	void addMessagePreamble();

private:
	std::queue<byte> bytesQueue;
};

#endif