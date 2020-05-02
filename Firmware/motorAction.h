#pragma once

#include "state.h" //Circular dependency maybe? Keep an eye on this..

class motorAction
{
public:
	//Maybe motor/movement control should be moved out of state class?
	typedef void (state::*movementActionFunction)(byte movementSpeed);

	//Default constructor, however the object must not be used before initialise function is called
	motorAction(); //How do I prevent his from being called outside the actionQueue

	motorAction(state* MotorState, movementActionFunction Action, byte ActionId, unsigned int ExecutionTimeMs, byte speed);

	//void initialise(movementActionFunction Action, byte ActionId, unsigned int ExecutionTimeMs);
	bool isStarted() const { return started; }
	
	unsigned int getExecutionTimeMs() const { return executionTimeMs; }
	byte getActionId() const { return actionId; }
	bool isIndefinite() const { return getExecutionTimeMs() == 0; }
	//Starts the action
	void start();

	//Called after the action is completed, but ONLY if there are no further actions queued
	void onActionCompleted();

private:
	//The operation that will be carried out when this action starts
	movementActionFunction movementAction;
	state* motorState;

	byte speed;
	unsigned int executionTimeMs; //Initial execution time
	
	//Id that the controller will identify this action by
	byte actionId;

	bool started;
};