#include "motorAction.h"

void motorAction::start() 
{
	started = true;
	(motorState->*movementAction)(speed); 
}

motorAction::motorAction() : started(false), actionId(0), executionTimeMs(0), motorState(nullptr),
							movementAction(nullptr), speed(0)
{
}

motorAction::motorAction(state* MotorState, movementActionFunction Action, byte ActionId, unsigned int ExecutionTimeMs, byte Speed)
	: started(false), actionId(ActionId), executionTimeMs(ExecutionTimeMs), motorState(MotorState), movementAction(Action), speed(Speed)
{
}

//Should rename this to onLastQueueAction
void motorAction::onActionCompleted()
{
	//Called only if there's no further actions queued
	motorState->stop(0);
}