#pragma once

#include "timeMeasurement.h"
#include "Debug.h"
#include "queue.h"

enum movevementStateEnum
{
  eMoveStateInvalid = -1,
  eMoveStateStop = 0,
  eMoveStateForward = 1,
  eMoveStateBackwards = 2,
  eMoveStateLeft = 3,
  eMoveStateRight = 4,
  
  eMOVE_STATE_MAX
};

/* Responsible for executing timed action queues. Informs the control after each action is finished */
template<class T>
class actionQueue
{
public:
	actionQueue(byte ActionQueueId);

	void addNewAction(const T& action);

	bool AnyActionsQueued();

	T* GetCurrentAction();

	//Returns TRUE if an action was finished
	bool UpdateQueue(unsigned int elapsedTime);

	byte GetLastFinishedActionId() { return lastFinishedActionId; }

	//This shouldn't work? Compiler peculiarity maybe?
	const static int queueLength = 10;

	//A variable used to identify the queue when communicating with the controller (in case there are several queues)
	const byte actionQueueId;
private:
	void CompleteCurrentAction();

	//Returns true if it started an action
	bool StartNextAction();

	//A FIFO container for storing actions.
	queue<T> internalQueue;

	countdownTimer timer;

	//Updated every time an action is finished
	byte lastFinishedActionId;
};

/**** actionQueue implementation ****/

//template<class T>
//const int actionQueue<T>::queueLength = 10;

template<class T>
actionQueue<T>::actionQueue(byte ActionQueueId) : actionQueueId(ActionQueueId), internalQueue(queueLength) {}

template<class T>
void actionQueue<T>::addNewAction(const T& action)
{
	//If the last action in the queue is indefinite, replace it with the new action that
	//is getting added.
	if(AnyActionsQueued() && internalQueue.last()->isIndefinite())
	{
		auto lastItemIndex = internalQueue.getSize() - 1;
		internalQueue.replace(lastItemIndex, action);
	}
	else
	{
		//Should check the result here.
		internalQueue.tryAdd(action);
	}
}

template<class T>
bool actionQueue<T>::AnyActionsQueued()
{
	return internalQueue.getSize() != 0;
}

template<class T>
T* actionQueue<T>::GetCurrentAction()
{
	return internalQueue.first();
}

template<class T>
void actionQueue<T>::CompleteCurrentAction()
{
	lastFinishedActionId = GetCurrentAction()->getActionId();
	internalQueue.popFront();
}

template<class T>
bool actionQueue<T>::StartNextAction()
{
	auto currAction = GetCurrentAction();

	//An action has already been started, do nothing.
	if(currAction->isStarted())
		return false;

	//There's an action that can be started.
	currAction->start();

	//If execTime is 0 the action will be performed indefinitely or till a timed action supercedes it
	if(!currAction->isIndefinite())
	{
		timer.resetTimer(currAction->getExecutionTimeMs());
	}

	return true;	
}

template<class T>
bool actionQueue<T>::UpdateQueue(unsigned int elapsedTime)
{
	//1. Check if there are any actions queued up, maybe no need to do anything here!
	//2. If there's an action queued, check if it has been started yet
	//2.1 If it has been started, update its timer and check if it's completed yet,
	//2.1.1 If it has completed, remove it from the queue and send a message to the control application to let it know
	//2.2 If it hasn't been started, set the flag to true, start the timer and start the action
	if(!AnyActionsQueued())
	{
		//No actions completed.
		return false; 
	}

	//Attempt to start a new action.
	if(StartNextAction())
	{
		//We just started a new action, so no need to do anything else.
		return false; 
	}

	//Get current action before we 'dispose' of it
	auto currentAction = GetCurrentAction();

	//Check if time for the current action has run out, if so, check if we can start a new one.
	if(currentAction->isIndefinite() || !timer.updateTimer(elapsedTime))
		return false;

	CompleteCurrentAction();
			
	//Check again if we still have any actions and then start the next action
	if(AnyActionsQueued())
	{
		StartNextAction();
	}
	else
	{
		//We completed an action, and there was nothing else queued.
		//Normally an action is simply replaced by another action, but since this is the last one,
		//we need to make sure that all on-going processes related to the action (such as motors spinning) are stopped.
		currentAction->onActionCompleted(); 
	}

  //We have finished an action.
  return true;
}
  