#include "state.h"
#include "Debug.h"
#include <string.h>

/* Motor state implementation */

motorState::motorState( ) 
           : state(eMotorIdle), motorSpeed(0), logicPin1(0), logicPin2(0), speedPin(0)
{
	
}

void motorState::initialise( byte LogicPin1, byte LogicPin2, byte SpeedPin )
{
	logicPin1 = LogicPin1;
	logicPin2 = LogicPin2;
	speedPin = SpeedPin;

	pinMode(logicPin1, OUTPUT);
	pinMode(logicPin2, OUTPUT);
	pinMode(speedPin, OUTPUT);
}

void motorState::setMotorState ( motorStateEnum State )
{ 
  switch( State )
  {
    case eMotorIdle:
      digitalWrite( getLogicPin1(), LOW );
      digitalWrite( getLogicPin2(), LOW );
      break;
      
    case eMotorForward:
      digitalWrite( getLogicPin1(), LOW );
      digitalWrite( getLogicPin2(), HIGH );
      break;
      
    case eMotorBackward:
      digitalWrite( getLogicPin1(), HIGH );
      digitalWrite( getLogicPin2(), LOW );
      break;
      
    case eMotorStop:
      digitalWrite( getLogicPin1(), HIGH );
      digitalWrite( getLogicPin2(), HIGH );
      break;
      
    default:
      return; //Do nothing, invalid state passed
  }

   //Record the current state
   state = State;
}

void motorState::setMotorSpeed ( byte MotorSpeed )
{
	analogWrite( getSpeedPin(), MotorSpeed );
	motorSpeed = MotorSpeed;
}

/* State class implementations */

state::state()
{
}

void state::initialise()
{
    //Start the bluetooth service
    getMessenger()->initialise();

    //Map classes to hardware pins
    leftMotor.initialise(pinMapping.motors.leftLogic1, pinMapping.motors.leftLogic2, pinMapping.motors.leftSpeed);
    rightMotor.initialise(pinMapping.motors.rightLogic1, pinMapping.motors.rightLogic2, pinMapping.motors.rightSpeed);

    distanceUltraSoundSensor.initialise(pinMapping.sensors.echoTrig, pinMapping.sensors.echoReceive);
    magnetometer.initialise();

    rightWheelEncoder.initialise(pinMapping.interrupts.interrupt1, 20);
    leftWheelEncoder.initialise(pinMapping.interrupts.interrupt2, 20);
}

motorState* state::getMotor( motor motorNode )
{
    //Illegal value
    if ( motorNode >= eMOTOR_NUM_MAX )
        return nullptr;

    if( motorNode == eMotorLeft )
        return &leftMotor;
    else if ( motorNode == eMotorRight )
        return &rightMotor;

    return nullptr;
}

void state::moveForward(byte movementSpeed)
{
	motorState* pLeft = getMotor(eMotorLeft);
	motorState* pRight = getMotor(eMotorRight);

	if(pLeft != NULL && pRight != NULL )
	{
		pLeft->setMotorState(eMotorForward);
		pRight->setMotorState(eMotorForward);
	}

	setAllMotorSpeed(movementSpeed);
}

void state::moveBack(byte movementSpeed)
{
	motorState* pLeft = getMotor(eMotorLeft);
	motorState* pRight = getMotor(eMotorRight);

	if(pLeft != NULL && pRight != NULL )
	{
		pLeft->setMotorState(eMotorBackward);
		pRight->setMotorState(eMotorBackward);
	}

	setAllMotorSpeed(movementSpeed);
}

void state::turnLeft(byte movementSpeed)
{
	motorState* pLeft = getMotor(eMotorLeft);
	motorState* pRight = getMotor(eMotorRight);

	if(pLeft != NULL && pRight != NULL )
	{
		pLeft->setMotorState(eMotorBackward);
		pRight->setMotorState(eMotorForward);
	}

	setAllMotorSpeed(movementSpeed);	
}

void state::turnRight(byte movementSpeed)
{
	motorState* pLeft = getMotor(eMotorLeft);
	motorState* pRight = getMotor(eMotorRight);

	if(pLeft != NULL && pRight != NULL )
	{
		pLeft->setMotorState(eMotorForward);
		pRight->setMotorState(eMotorBackward);
	}

	setAllMotorSpeed(movementSpeed);	
}


void state::stop(byte movementSpeed)
{
	motorState* pLeft = getMotor(eMotorLeft);
	motorState* pRight = getMotor(eMotorRight);

	if(pLeft != NULL && pRight != NULL )
	{
		pLeft->setMotorState( eMotorIdle );
		pRight->setMotorState( eMotorIdle );
	}		
}

float state::getDistance()
{
	return distanceUltraSoundSensor.getLastDistanceReading();
}

void state::setAllMotorSpeed( byte speed )
{
	getMotor(eMotorLeft)->setMotorSpeed(speed);
	getMotor(eMotorRight)->setMotorSpeed(speed);
}

void state::updateDistanceMeasurement()
{
	distanceUltraSoundSensor.measureDistance();
}

void state::updateMagnetometerMeasurement()
{
	magnetometer.updateMeasurement();
}

int state::getMagnetometerOrientationX()
{
	return magnetometer.getLastReadingX();
}

int state::getMagnetometerOrientationY()
{
	return magnetometer.getLastReadingY();
}
int state::getMagnetometerOrientationZ()
{
	return magnetometer.getLastReadingZ();
}

unsigned int state::getRotaryEncoderCount(motor motorNode)
{
    switch (motorNode)
    {
    case eMotorLeft:
        return leftWheelEncoder.getCounter();
    case eMotorRight:
        return rightWheelEncoder.getCounter();
    default:
        return 0;
    }
}

void state::printEncoderCounters()
{
    unsigned int counterLeft = leftWheelEncoder.getCounter();
    unsigned int counterRight = rightWheelEncoder.getCounter();
    char debugString[100];

    sprintf(debugString, "Left wheel count: %d, Right wheel count: %d", counterLeft, counterRight);
    Serial.println(debugString);
}
