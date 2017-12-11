#pragma once

#include "state.h"
#include "timeMeasurement.h"

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
	bool UpdateQueue(unsigned int elapsedTime, state* pState);

	byte GetLastFinishedActionId() { return lastFinishedActionId; }

	//This shouldn't work? Compiler peculiarity maybe?
	const static int queueLength = 10;

	//A variable used to identify the queue when communicating with the controller (in case there are several queues)
	const byte actionQueueId;
private:
	void CompleteCurrentAction();

	//Returns true if it started an action
	bool StartNextAction(state* pState);

	//Treats the action queue as a virtual array, which starts at the 'CurrentAction'
	T* getAction(int index);

	//Number of currently actions
	int getQueuedActionsCount();

	countdownTimer timer;

	//Index for the action that the robot is currently carrying out
	int currentActionIndex;
	//Index for the next action insertion slot. This wraps around the array
	int nextInsertSlotIndex;
	T queue[queueLength];

	byte nextActionId;

	//Updated every time an action is finished
	byte lastFinishedActionId;
};

/**** actionQueue implementation ****/

//template<class T>
//const int actionQueue<T>::queueLength = 10;

template<class T>
actionQueue<T>::actionQueue(byte ActionQueueId): currentActionIndex(0), nextInsertSlotIndex(0), nextActionId(0),
 actionQueueId(ActionQueueId)
{

}

template<class T>
T* actionQueue<T>::getAction(int index)
{
	auto normalizedIndex = currentActionIndex + index % queueLength; //Avoid trying to access elements out of bounds
	//[2] [3n] [4] [c0] [1]
	return &queue[normalizedIndex];
}

template<class T>
int actionQueue<T>::getQueuedActionsCount()
{
	//This is effectively a circular buffer, hence this function not being straightforward
	if(currentActionIndex == nextInsertSlotIndex) //[nc] [] [] [] []
		return 0;

	if(currentActionIndex > nextInsertSlotIndex)
	{
		return queueLength - (currentActionIndex + 1) + nextInsertSlotIndex; //[] [n] [] [c] []
	}
	else
	{
		return nextInsertSlotIndex - currentActionIndex; //[] [c] [] [n] []
	}
}

template<class T>
void actionQueue<T>::addNewAction(const T& action)
{
	//There's no protective mechanism from queue 'overflowing', it's possible for th/e currently executed action to be overwritten
	//if too many actions get queued in a row. Leave it up to roboApp to manage this..? Maybe
	auto lastQueueAction = getQueuedActionsCount() - 1;

	//If the currently queued action is permanent, and a new action is added, this action needs to be removed from
	//the queue and the newly added action needs to be started immediately

	//RoboApp should track assign the action ID, otherwise the whole sync problem is gonna be a huge pain

	//Check if the last action in the queue is permanent, and replace it if it is
	//If it's the current action, the new action will start on the next update
	if(lastQueueAction >= 0 && getAction(lastQueueAction)->isIndefinite())
	{
		//Decrement while making sure we don't get negative numbers here
		nextInsertSlotIndex = ((nextInsertSlotIndex - 1) < 0 ) ? 0 : nextInsertSlotIndex - 1;
	}

	queue[nextInsertSlotIndex] = action;
	++nextInsertSlotIndex;
	nextInsertSlotIndex = nextInsertSlotIndex % queueLength;
}

template<class T>
bool actionQueue<T>::AnyActionsQueued()
{
	return currentActionIndex != nextInsertSlotIndex;
}

template<class T>
T* actionQueue<T>::GetCurrentAction()
{
	return &queue[currentActionIndex];
}

template<class T>
void actionQueue<T>::CompleteCurrentAction()
{
	lastFinishedActionId = currentActionIndex;

	++currentActionIndex;
	currentActionIndex = currentActionIndex % queueLength;
}

template<class T>
bool actionQueue<T>::StartNextAction(state* pState)
{
	auto currAction = GetCurrentAction();
	if(!currAction->isStarted())
	{
		currAction->start(pState);

		//If execTime is 0 the action will be performed indefinitely or till a timed action supercedes it
		if(currAction->isIndefinite())
			timer.resetTimer(currAction->getExecutionTimeMs());

		return true;
	}

	return false; //Did not start an action
}

template<class T>
bool actionQueue<T>::UpdateQueue(unsigned int elapsedTime, state* pState)
{
	//1. Check if there are any actions queued up, maybe no need to do anything here!
	//2. If there's an action queued, check if it has been started yet
	//2.1 If it has been started, update its timer and check if it's completed yet,
	//2.1.1 If it has completed, remove it from the queue and send a message to the control application to let it know
	//2.2 If it hasn't been started, set the flag to true, start the timer and start the action
	if(AnyActionsQueued())
	{
		if(StartNextAction(pState))
			return false; //We just started a new action, so no need to do anything else

		//Get current action before we 'dispose' of it
		auto currentAction = GetCurrentAction();

		//Check if time for the current action has run out, if so, check if we can start a new one
		if(!currentAction->isIndefinite() && timer.updateTimer(elapsedTime))
		{
			CompleteCurrentAction();
			
			//Check again if we still have any actions and then start the next action
			if(AnyActionsQueued())
				StartNextAction(pState);
			else
				currentAction->onActionCompleted(pState); //We completed an action, and there was nothing else queued

			return true; //We have finished an action
		}
	}

	return false; //No actions completed
}