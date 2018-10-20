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

void messagingServiceTests()
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

int main()
{
	//testActionQueueIndefiniteActionReplacement();
	queueTests();
	messagingServiceTests();
	return 0;
}

#endif