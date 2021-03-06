#include "controller.h"
#include "constants.h"
#include "actionQueue.h"
#include "motorAction.h"
#include "Debug.h"
#include "serializer.h"

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
	if(motorActionQueue.UpdateQueue(elapsedTime))
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

    //If true, countdown time over, do the operation and reset
    if (echoSensorCountdown.updateTimer(elapsedTime))
    {
        getState()->updateDistanceMeasurement();
        echoSensorCountdown.resetTimer(constants.timers.echoDistancePeriod);

        //Send message to the master to let it know the ultrasound sensor distance
        float distance = getState()->getDistance();
        sendEchoDistanceData(distance);
    }

    if (magnetometerSensorCountdown.updateTimer(elapsedTime))
    {
        getState()->updateMagnetometerMeasurement();
        magnetometerSensorCountdown.resetTimer(constants.timers.magnetometerPeriod);

        //DO SOMETHING WITH THE DATA HERE
        int orientationX = getState()->getMagnetometerOrientationX();
        int orientationY = getState()->getMagnetometerOrientationY();
        int orientationZ = getState()->getMagnetometerOrientationZ();

        sendMagnetometerData(orientationX, orientationY, orientationZ);
    }

    if (rotaryEncoderSensorCountdown.updateTimer(elapsedTime))
    {
        rotaryEncoderSensorCountdown.resetTimer(constants.timers.rotaryEncoderPeriod);

        sendRotaryEncodersData(getState()->getRotaryEncoderCount(eMotorLeft), getState()->getRotaryEncoderCount(eMotorRight));

        //Temporary serial output for debugging purposes
        getState()->printEncoderCounters();
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
	
	//Store the action in a queue.
	motorAction tempMotorAction(getState(), action, actionId, executionTimeMs, movementSpeed);
	motorActionQueue.addNewAction(tempMotorAction);
	return true;
}

void controller::sendEchoDistanceData(float& distance)
{
    message distanceMessage;
    distanceMessage.id = constants.messageIdTx.echoDistanceMsg;

    serializer writer(distanceMessage.messageData);

    writer.serializeFloat(distance);

    distanceMessage.dataLength = writer.getDataLength();

    getState()->getMessenger()->sendMessage(distanceMessage);
}

void controller::sendMagnetometerData(int x, int y, int z)
{
    message magnetoMessage;
    magnetoMessage.id = constants.messageIdTx.magnetometerDataMsg;

    serializer writer(magnetoMessage.messageData);

    writer.serializeInt(x);
    writer.serializeInt(y);
    writer.serializeInt(z);

    magnetoMessage.dataLength = writer.getDataLength();

    getState()->getMessenger()->sendMessage(magnetoMessage);
}

void controller::sendRotaryEncodersData(unsigned int leftWheelCounter, unsigned int rightWheelCounter)
{
    message encodersData;
    encodersData.id = constants.messageIdTx.rotaryEncodersMsg;

    serializer writer(encodersData.messageData);

    writer.serializeInt(leftWheelCounter);
    writer.serializeInt(rightWheelCounter);

    encodersData.dataLength = writer.getDataLength();

    getState()->getMessenger()->sendMessage(encodersData);
}

void controller::sendActionQueueActionCompleted(byte queueId, byte actionId)
{
    message actionCompletedMessage;
    actionCompletedMessage.id = constants.messageIdTx.actionCompletedMsg;

    serializer writer(actionCompletedMessage.messageData);

    writer.serializeByte(queueId);
    writer.serializeByte(actionId);

    actionCompletedMessage.dataLength = writer.getDataLength();

    getState()->getMessenger()->sendMessage(actionCompletedMessage);
}