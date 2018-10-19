#pragma once

#include "timeMeasurement.h"
#include "Debug.h"
#include <stdio.h>
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
	Serial.println("Adding action!");
	//There's no protective mechanism from queue 'overflowing', it's possible for th/e currently executed action to be overwritten
	//if too many actions get queued in a row. Leave it up to roboApp to manage this..? Maybe
	/*auto lastQueueAction = getQueuedActionsCount() - 1; //Is this correct??

	//If the currently queued action is permanent, and a new action is added, this action needs to be removed from
	//the queue and the newly added action needs to be started immediately

	//RoboApp should track assign the action ID, otherwise the whole sync problem is gonna be a huge pain

	//Check if the last action in the queue is permanent, and replace it if it is
	//If it's the current action, the new action will start on the next update
	if(lastQueueAction >= 0 && getAction(lastQueueAction)->isIndefinite()) 
	{
		Serial.println("Ovrwrite indef act");
		//Decrement while making sure we don't get negative numbers here
		nextInsertSlotIndex = lastQueueAction;
	}*/

	//Assuming getQueuedActionsCount works ok
	//getAction is normalized, so it effectively gets the current action when passed 0
	// So when there was 1 timed action and and 1 indef action, the count should return 1, and with -1 the idx ends up being 0
	//So when it's check for indef, it checks current (which is in fact indefinite)
	//But then the overwrite would happen at actual 0, which was infact timed action
	//And since the insert index was 0, nexInsertSLot and currentIndex end up being the same, which means it does not even attempt to start any action
	//BUT WHY does it start working at ID 5 and not ID 3??

	//IF the current action is indefinite, we want to override it with the new one.

  //11/10/2018 - If the last action in the queue is indefinite,
  //but the currently processed action is timed, any new indefinite actions won't actually replace the 
  //last action in the queue but just add it onto the list. NOT CORRECT.

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

	//char charBuffer[30];
	//sprintf(charBuffer, "Added at:%d,tm:%d", nextInsertSlotIndex, action.getExecutionTimeMs());
	//Serial.println(charBuffer);

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
	char charBuffer[24];
	sprintf(charBuffer, "Completed id:%d", lastFinishedActionId);
	Serial.println(charBuffer);
	internalQueue.popFront();
}

template<class T>
bool actionQueue<T>::StartNextAction()
{
	auto currAction = GetCurrentAction();

	char charBuffer2[40]; //Is this it????
	sprintf(charBuffer2, "Attempt start, id:%d", currAction->getActionId());
	Serial.println(charBuffer2);

	//An action has already been started, do nothing.
	if(currAction->isStarted())
		return false;

	//There's an action that can be started.
	char charBuffer[30]; //Is this it????
	sprintf(charBuffer, "Action start, time:%d", currAction->getExecutionTimeMs());
	Serial.println(charBuffer);
	//SendStringToApp(charBuffer);
	currAction->start();

	//If execTime is 0 the action will be performed indefinitely or till a timed action supercedes it
	if(!currAction->isIndefinite())
	{
		//Serial.println("Act is indef");
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

	//Serial.println("Actions queued");

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
  