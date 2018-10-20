#include "state.h"
#include "actionQueue.h"
#include "queue.h"
//Need the ifdef to make sure that arduino IDE won't try to latch onto
//the main and ignore the real one.
#ifdef VS_TESTBED
#include "serialInterfaceMock.h"
#include "messagingService.h"
#include <cassert>

class actionMock
{
public:
	actionMock(): executionTime(0), actionId(0) {}

	actionMock(unsigned int ExecutionTime, byte ActionId): executionTime(ExecutionTime), actionId(ActionId) {}

	bool isIndefinite() { return executionTime == 0; }
	bool isStarted() { return false; }
	unsigned int getExecutionTimeMs() const { return executionTime; }
	void onActionCompleted() {}
	byte getActionId() const { return actionId; }
	void start() {}
private:
	unsigned int executionTime;
	byte actionId;
};

void testActionQueueIndefiniteActionReplacement()
{
	actionQueue<actionMock> queue(0);

	actionMock timedAction(1, 0);
	actionMock indefAction1(0, 1);
	actionMock indefAction2(0, 2);
	actionMock indefAction3(0, 3);
	queue.addNewAction(timedAction);
	
	//Indefinite actions should overwrite each at the end of the queue
	queue.addNewAction(indefAction1);
	queue.addNewAction(indefAction2);
	queue.addNewAction(indefAction3);

	queue.UpdateQueue(0);
}

void testQueueAddLimit()
{
	queue<int> q(5);
	bool successfulAdd = false;
	successfulAdd = q.tryAdd(1);
	successfulAdd = q.tryAdd(2);
	successfulAdd = q.tryAdd(3);
	successfulAdd = q.tryAdd(4);
	successfulAdd = q.tryAdd(5);
	assert(successfulAdd == true);
	//Expected false.
	successfulAdd = q.tryAdd(6);
	assert(successfulAdd == false);
}

void testQueuePopSizeChange()
{
	queue<int> q(5);
	q.tryAdd(1);
	assert(q.getSize() == 1);
	q.popFront();
	assert(q.getSize() == 0);
}

void testQueueWraparound()
{
	queue<int> q(4);

	q.tryAdd(1);
	q.tryAdd(2);
	q.tryAdd(3);
	q.tryAdd(4);
	assert(q.getSize() == 4);

	q.popFront();
	q.popFront();
	q.popFront();
	assert(q.getSize() == 1);
	assert(*q.get(0) == 4);

	q.tryAdd(5);
	q.tryAdd(6);
	q.tryAdd(7);
	assert(q.getSize() == 4);

	assert(*q.get(0) == 4);
	assert(*q.get(1) == 5);
	assert(*q.get(2) == 6);
	assert(*q.get(3) == 7);
}

void testQueueGetOutOfBounds()
{
	queue<int> q(4);
	q.tryAdd(1);

	assert(q.get(2) == nullptr);
}

void testQueueReplace()
{
	queue<int> q(4);

	q.tryAdd(1);
	q.tryAdd(2);
	q.tryAdd(3);
	assert(*q.get(1) == 2);

	q.replace(1, 10);
	assert(*q.get(1) == 10);
}

void testQueueEmptyFirst()
{
	queue<int> q(4);
	q.tryAdd(1);
	q.popFront();
	assert(q.first() == nullptr);
}

void testQueueEmptyLast()
{
	queue<int> q(4);
	q.tryAdd(1);
	q.popFront();
	assert(q.last() == nullptr);
}

void testQueueFirst()
{
	queue<int> q(4);
	q.tryAdd(1);
	q.tryAdd(42);
	q.popFront();
	assert(*q.first() == 42);
}

void testQueueLast()
{
	queue<int> q(4);
	q.tryAdd(1);
	q.tryAdd(11);
	q.popFront();
	q.tryAdd(42);
	assert(*q.last() == 42);
}

void testQueuePopXItems()
{
	queue<int> q(4);
	q.tryAdd(1);
	q.tryAdd(2);
	q.tryAdd(3);
	q.tryAdd(4);
	q.popFront(3);
	assert(*q.get(0) == 4);
	assert(q.getSize() == 1);
}

void testQueuePopXItemsWithWraparound()
{
	queue<int> q(4);
	q.tryAdd(1);
	q.tryAdd(2);
	q.tryAdd(3);
	q.tryAdd(4);
	q.popFront(3);

	q.tryAdd(5);
	q.tryAdd(6);
	q.tryAdd(7);
	q.popFront(3);
	assert(*q.get(0) == 7);
	assert(q.getSize() == 1);
}

void testQueuePopXItemsPopAllItems()
{
	queue<int> q(4);
	q.tryAdd(1);
	q.tryAdd(2);
	q.tryAdd(3);
	q.tryAdd(4);

	q.popFront(4);

	assert(q.get(0) == nullptr);
	assert(q.getSize() == 0);
}

void queueTests()
{
	testQueueAddLimit();
	testQueuePopSizeChange();
	testQueueWraparound();
	testQueueGetOutOfBounds();
	testQueueReplace();
	testQueueEmptyFirst();
	testQueueEmptyLast();
	testQueueFirst();
	testQueueLast();
	testQueuePopXItems();
	testQueuePopXItemsWithWraparound();
	testQueuePopXItemsPopAllItems();
}

void assertMessagesEqual(const message& a, const message& b)
{
	assert(a.dataLength == b.dataLength);
	assert(a.id == b.id);
	for (int i = 0; i < a.dataLength && i < constants.maximumMessageDataLength; ++i)
	{
		assert(a.messageData[i] == b.messageData[i]);
	}
}

void messagingServiceZeroDataLengthMessage()
{
	serialInterfaceMock serialMock;

	message sentMessage;
	sentMessage.dataLength = 0;
	sentMessage.id = 12;
	serialMock.addMessageBytesForReading(sentMessage);

	messagingService<serialInterfaceMock> messaging(&serialMock);

	messaging.receiveIncomingData();
	message* receivedMessage = messaging.processMessage();
	assert(receivedMessage != nullptr);

	assertMessagesEqual(sentMessage, *receivedMessage);
}

void messagingServiceMessageParse()
{
	serialInterfaceMock serialMock;

	message sentMessage;
	sentMessage.dataLength = 5;
	sentMessage.id = 12;
	sentMessage.messageData[0] = 0;
	sentMessage.messageData[1] = 1;
	sentMessage.messageData[2] = 2;
	sentMessage.messageData[3] = 3;
	sentMessage.messageData[4] = 4;

	serialMock.addMessageBytesForReading(sentMessage);

	messagingService<serialInterfaceMock> messaging(&serialMock);

	messaging.receiveIncomingData();
	message* receivedMessage = messaging.processMessage();
	assert(receivedMessage != nullptr);

	assertMessagesEqual(sentMessage, *receivedMessage);
}

void messagingServiceParseMessageReceivedInSingleBytes()
{
	serialInterfaceMock serialMock;

	//Create a message that we'll try to compare against.
	message sentMessage;
	sentMessage.dataLength = 1;
	sentMessage.id = 12;
	sentMessage.messageData[0] = 42;

	const auto arraySize = constants.minimumMessageLength + 1;
	byte messageAsByteArray[arraySize];
	messageAsByteArray[0] = messageAsByteArray[1] = 0xbb;
	memcpy(messageAsByteArray + 2, &sentMessage, sizeof(sentMessage.dataLength) + sentMessage.id + sizeof(byte));

	messagingService<serialInterfaceMock> messaging(&serialMock);

	//Receive the bytes one by one and try looking for a message each time.
	for (int i = 0; i < arraySize - 1; ++i)
	{
		serialMock.addByte(messageAsByteArray[i]);

		messaging.receiveIncomingData();
		assert(messaging.processMessage() == nullptr);
	}

	//Add the final byte and try processing again.
	//This time it should be successful.
	serialMock.addByte(messageAsByteArray[arraySize - 1]);

	messaging.receiveIncomingData();
	message* receivedMessage = messaging.processMessage();
	assert(receivedMessage != nullptr);
	assertMessagesEqual(sentMessage, *receivedMessage);
}

void messagingServiceMessageParseNotAllDataInitially()
{
	serialInterfaceMock serialMock;

	message sentMessage;
	sentMessage.dataLength = 5;
	sentMessage.id = 12;
	sentMessage.messageData[0] = 0;
	sentMessage.messageData[1] = 1;
	sentMessage.messageData[2] = 2;
	sentMessage.messageData[3] = 3;
	sentMessage.messageData[4] = 4;

	serialMock.addMessageBytesForReading(sentMessage, 2);

	messagingService<serialInterfaceMock> messaging(&serialMock);

	//Not all data received, shouldn't get a received message.
	messaging.receiveIncomingData();
	message* receivedMessage = messaging.processMessage();
	assert(receivedMessage == nullptr);

	serialMock.addByte(2);
	serialMock.addByte(3);
	serialMock.addByte(4);

	//Receive the rest of the data and make sure the message is there too.
	messaging.receiveIncomingData();
	receivedMessage = messaging.processMessage();
	assert(receivedMessage != nullptr);

	assertMessagesEqual(sentMessage, *receivedMessage);
}

void messagingServiceMessageParseGarbageAtTheStart()
{
	serialInterfaceMock serialMock;

	//Add some 'gargabage' data to the data stream.
	for (int i = 0; i < 6; ++i)
	{
		serialMock.addByte(42);
	}
	
	//And only then send the actual message.
	message sentMessage;
	sentMessage.dataLength = 5;
	sentMessage.id = 12;
	sentMessage.messageData[0] = 0;
	sentMessage.messageData[1] = 1;
	sentMessage.messageData[2] = 2;
	sentMessage.messageData[3] = 3;
	sentMessage.messageData[4] = 4;

	serialMock.addMessageBytesForReading(sentMessage);

	messagingService<serialInterfaceMock> messaging(&serialMock);

	messaging.receiveIncomingData();
	message* receivedMessage = messaging.processMessage();
	assert(receivedMessage != nullptr);

	assertMessagesEqual(sentMessage, *receivedMessage);
}

void messagingServiceGarbageDataOnly()
{
	serialInterfaceMock serialMock;

	//Add some 'gargabage' data to the data stream.
	for (int i = 0; i < 20; ++i)
	{
		serialMock.addByte(42 + i);
	}

	messagingService<serialInterfaceMock> messaging(&serialMock);

	messaging.receiveIncomingData();
	message* receivedMessage = messaging.processMessage();
	assert(receivedMessage == nullptr);
}

void messagingServiceMessageParseTwoInARow()
{
	serialInterfaceMock serialMock;

	//Queue up two messages in a row.
	message sentMessage1;
	sentMessage1.dataLength = 5;
	sentMessage1.id = 12;
	sentMessage1.messageData[0] = 0;
	sentMessage1.messageData[1] = 1;
	sentMessage1.messageData[2] = 2;
	sentMessage1.messageData[3] = 3;
	sentMessage1.messageData[4] = 4;

	serialMock.addMessageBytesForReading(sentMessage1);

	message sentMessage2;
	sentMessage2.dataLength = 4;
	sentMessage2.id = 42;
	sentMessage2.messageData[0] = 4;
	sentMessage2.messageData[1] = 5;
	sentMessage2.messageData[2] = 6;
	sentMessage2.messageData[3] = 7;

	serialMock.addMessageBytesForReading(sentMessage2);

	messagingService<serialInterfaceMock> messaging(&serialMock);

	messaging.receiveIncomingData();

	//Make sure we've received the the two messages
	message* receivedMessage1 = messaging.processMessage();
	assert(receivedMessage1 != nullptr);
	assertMessagesEqual(sentMessage1, *receivedMessage1);

	message* receivedMessage2 = messaging.processMessage();
	assert(receivedMessage2 != nullptr);
	assertMessagesEqual(sentMessage2, *receivedMessage2);
}

void messagingServiceDataLengthTooGreat()
{
	serialInterfaceMock serialMock;

	message sentMessage;
	//Set the data length to be invalid.
	sentMessage.dataLength = constants.maximumMessageDataLength + 1;
	sentMessage.id = 12;

	serialMock.addMessageBytesForReading(sentMessage, 2);

	messagingService<serialInterfaceMock> messaging(&serialMock);

	messaging.receiveIncomingData();
	message* receivedMessage = messaging.processMessage();
	assert(receivedMessage == nullptr);
}

void messagingServiceTests()
{
	messagingServiceZeroDataLengthMessage();
	messagingServiceMessageParse();
	messagingServiceParseMessageReceivedInSingleBytes();
	messagingServiceMessageParseNotAllDataInitially();
	messagingServiceMessageParseGarbageAtTheStart();
	messagingServiceGarbageDataOnly();
	messagingServiceMessageParseTwoInARow();
	messagingServiceDataLengthTooGreat();
}

int main()
{
	//testActionQueueIndefiniteActionReplacement();
	queueTests();
	messagingServiceTests();
	return 0;
}

#endif