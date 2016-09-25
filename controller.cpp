#include "controller.h"
#include "constants.h"
controller::controller() : pState( NULL )
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
			break;
	};
}

void controller::act()
{
	//If close to obstacles, low Fspeed
	/*if( getState()->getDistance() < 250 )
	{
		getState()->setAllMotorSpeed( 150 );
	}
	else
	{
		getState()->setAllMotorSpeed( 255 );
	}

	//If too close to obstacle, rotate
	if( getState()->getDistance() <= 200 )
	{
		getState()->turnRight();
	} 
	else
	{
		getState()->moveForward();
	}*/	
}

void controller::updateSensorData()
{
	//Time elapsed since last sensor update
	unsigned int elapsedTime = timer.timeStamp();
	getState()->getMessenger()->receiveIncomingData();
	if( echoSensorCountdown.updateTimer( elapsedTime ) ) //If true, countdown time over, do the operation and reset
	{
		getState()->updateDistanceMeasurement();
		echoSensorCountdown.resetTimer( constants.timers.echoDistancePeriod );

		//Send message to the master to let it know the ultrasound sensor distance
		message messageDistance;
		messageDistance.id = 0;
		messageDistance.dataLength = sizeof(float);
		float distance = getState()->getDistance();

		memcpy(messageDistance.messageData, &distance, sizeof(float));
		getState()->getMessenger()->sendMessage(messageDistance);
	}
	
}

state* controller::getState() { return pState; }

//**********************Message Handlers**********************//
bool controller::handleMoveMessage( message* msg )
{
	//Extract message data contents
	byte movementCommand  = msg->messageData[0];
	byte movementSpeed = msg->messageData[1];
	//char textMessage [100];
	//sprintf(textMessage, "Movement command: %d, movement speed: %d", movementCommand, movementSpeed );
 	//Serial.println(textMessage);

	if( movementCommand == 0 ) //0 is stop
		getState()->stop();
	else if ( movementCommand == 1 ) //1 is move forward
		getState()->moveForward();
	else if ( movementCommand == 2 ) //2 is move backwards
		getState()->moveBack();
	else if ( movementCommand == 3 ) //3 is move left
		getState()->turnLeft();
	else if ( movementCommand == 4 ) //4 is move right
		getState()->turnRight();
	else
	{
		//Serial.println("No movement command match.");
		return false; //Invalid command
	} 
	
	getState()->setAllMotorSpeed( movementSpeed );

	return true;
}

/**** Timer implementation ****/

timeMeasurement::timeMeasurement() : timeSinceLastTimestamp(0)
{
	lastTimestamp = millis();
}

unsigned int timeMeasurement::timeStamp()
{
	//Store timestamp and calculate elapsed time since last one
	unsigned int tempTS = lastTimestamp;
	lastTimestamp = millis();

	timeSinceLastTimestamp = lastTimestamp - tempTS;

	return timeSinceLastTimestamp;
}

countdownTimer::countdownTimer() : timeLeft(0)
{
}

bool countdownTimer::updateTimer(unsigned int timeElapsed)
{
	timeLeft -= (int) timeElapsed;

	//Timer reached the end
	return timeLeft <= 0;
}

void countdownTimer::resetTimer(unsigned int timerDuration)
{
	timeLeft += (int) timerDuration;
}


