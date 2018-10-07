#include "controller.h"
#include "constants.h"
#include "actionQueue.h"
#include "motorAction.h"
#include "Debug.h"
/**** controller implementation ****/

controller::controller() : pState( NULL ), motorActionQueue(0)
{
}

void controller::initialise(state* State)
{
	pState = State;
}

void controller::processMessage( message* msg )
{
	switch( msg->id )
	{
		case constants.messageIdRx.moveControl:
			handleMoveMessage(msg);
			break;
		default:
			Serial.println("Unknown msg");
			break;
	};
}

void controller::act()
{
	//Q: Does it have to be a separate timer or can it reuse the time from general controller timer?
	//A: Don't think there can be a guarantee that the actions performed between this and the previous
	//timestamp will always take up a constant period of time. 
	//Thefore it's probably best to have a seperate timer just for this
	unsigned int elapsedTime = actionQueueTimer.timeStamp();
	if(motorActionQueue.UpdateQueue(elapsedTime, getState())) 
	{
		//If update queue returns true, it means that an action was completed.
		//Inform the control application.
		sendActionQueueActionCompleted(motorActionQueue.actionQueueId, motorActionQueue.GetLastFinishedActionId());
	}
}

void controller::updateSensorData()
{
	//Time elapsed since last sensor update
	unsigned int elapsedTime = sensorTimer.timeStamp();
	getState()->getMessenger()->receiveIncomingData();
	return;
	if( echoSensorCountdown.updateTimer( elapsedTime ) ) //If true, countdown time over, do the operation and reset
	{
		getState()->updateDistanceMeasurement();
		echoSensorCountdown.resetTimer( constants.timers.echoDistancePeriod );

		//Send message to the master to let it know the ultrasound sensor distance
		float distance = getState()->getDistance();
		sendEchoDistanceData(distance);
	}
	
	if( magnetometerSensorCountdown.updateTimer( elapsedTime ) )
	{
		getState()->updateMagnetometerMeasurement();
		magnetometerSensorCountdown.resetTimer( constants.timers.magnetometerPeriod );

		//DO SOMETHING WITH THE DATA HERE
		int orientationX = getState()->getMagnetometerOrientationX();
		int orientationY = getState()->getMagnetometerOrientationY();
		int orientationZ = getState()->getMagnetometerOrientationZ();

		sendMagnetometerData(orientationX, orientationY, orientationZ);
	}
}

state* controller::getState() { return pState; }

//**********************Message Handlers**********************//
bool controller::handleMoveMessage( message* msg )
{
	//Extract message data contents
	byte readPosition = 0;
	byte movementCommand  = msg->messageData[readPosition++];
	byte movementSpeed = msg->messageData[readPosition++];

	unsigned int executionTimeMs;
	memcpy(&executionTimeMs, msg->messageData + readPosition, sizeof(int));

	readPosition += sizeof(int);
	byte actionId = msg->messageData[readPosition++];

	motorAction::movementActionFunction action = nullptr;

	//TODO: Should change this to an enum
	//Figure out what the RoboApp is asking the robot to do and store the function pointer
	switch(movementCommand)
	{
		case 0: //0 is stop
			action = &state::stop; //Should stop be treated differently and override everything else?
		break;
		case 1: //1 is move forward
			action = &state::moveForward;
			SendStringToApp("Rx fwd");
		break;
		case 2: //2 is move backwards
			action = &state::moveBack;
			SendStringToApp("Rx back");
		break;
		case 3: //3 is turn left
			action = &state::turnLeft;
			SendStringToApp("Rx left");
		break;
		case 4: //4 is turn right
			action = &state::turnRight;
			SendStringToApp("Rx right");
		break;
		default:
			Serial.println("Unknown move type");
			return false; //Invalid command
	}
	
	motorAction tempMotorAction(action, actionId, executionTimeMs, movementSpeed);
	motorActionQueue.addNewAction(tempMotorAction);
	return true;
}

void controller::sendEchoDistanceData(float& distance)
{
	//Send message to the master to let it know the ultrasound sensor distance
	message messageDistance;
	messageDistance.id = constants.messageIdTx.echoDistanceMsg;
	messageDistance.dataLength = sizeof(float);

	memcpy(messageDistance.messageData, &distance, sizeof(float));
	getState()->getMessenger()->sendMessage(messageDistance);
}

void controller::sendMagnetometerData(int x, int y, int z)
{
	message magnetoMessage;
	magnetoMessage.id = constants.messageIdTx.magnetometerDataMsg;
	magnetoMessage.dataLength = sizeof(int) * 3; //xyz, hence times 3

	memcpy(magnetoMessage.messageData, &x, sizeof(int));
	memcpy(magnetoMessage.messageData + sizeof(int), &y, sizeof(int));
	memcpy(magnetoMessage.messageData + sizeof(int) * 2, &z, sizeof(int));

	getState()->getMessenger()->sendMessage(magnetoMessage);
}

void controller::sendActionQueueActionCompleted(byte queueId, byte actionId)
{
	message actionCompletedMessage;
	actionCompletedMessage.id = constants.messageIdTx.actionCompletedMsg;
	actionCompletedMessage.dataLength = sizeof(byte) * 2;

	memcpy(actionCompletedMessage.messageData, &queueId, sizeof(byte));
	memcpy(actionCompletedMessage.messageData + sizeof(byte), &actionId, sizeof(byte));


	getState()->getMessenger()->sendMessage(actionCompletedMessage);
}
