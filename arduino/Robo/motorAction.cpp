#include "motorAction.h"

void motorAction::start(state* pState) 
{
	started = true;
	(pState->*movementAction)(speed); 
}

motorAction::motorAction() : started(false), actionId(0), executionTimeMs(0),
							movementAction(nullptr), speed(0)
{
}

motorAction::motorAction(movementActionFunction Action, byte ActionId, unsigned int ExecutionTimeMs, byte Speed)
	: started(false), actionId(ActionId), executionTimeMs(ExecutionTimeMs), movementAction(Action), speed(Speed)
{
}

void motorAction::onActionCompleted(state* pState)
{
	//Called only if there's no further actions queued
	pState->stop(0);
}

/*void motorAction::initialise(movementActionFunction Action, byte ActionId, unsigned int ExecutionTimeMs)
	started = false;
	movementAction = Action;
	actionId = ActionId;
	executionTimeMs = ExecutionTimeMs;
}*/