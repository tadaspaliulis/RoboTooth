#include "state.h"

/* Ultra sound distance sensor */

echoSensor::echoSensor() :
						triggerPin(0), echoPin (0), lastDistanceReading (5.0f)
{
}

void echoSensor::initialise( byte TriggerPin, byte EchoPin )
{
	triggerPin = TriggerPin;
	echoPin = EchoPin;

	//Set up the pins
	pinMode(triggerPin, OUTPUT);
	pinMode(echoPin, INPUT);
}

float echoSensor::getLastDistanceReading() 
{
	return lastDistanceReading;
}

//Perform a distance reading
float echoSensor::measureDistance()
{
	//Trigger  the echo
    digitalWrite(triggerPin, HIGH);
    delay(1);
    digitalWrite(triggerPin, LOW);

    //Time till echo turns high
    lastDistanceReading = (float)pulseIn(echoPin, HIGH) / 2;
   
	return lastDistanceReading;
}

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
	messenger.initialise();

	//Map classes to hardware pins
	leftMotor.initialise( pinMapping.motors.leftLogic1, pinMapping.motors.leftLogic2, pinMapping.motors.leftSpeed );
	rightMotor.initialise( pinMapping.motors.rightLogic1, pinMapping.motors.rightLogic2, pinMapping.motors.rightSpeed );
	distanceUltraSoundSensor.initialise( pinMapping.sensors.echoTrig, pinMapping.sensors.echoReceive );
}

motorState* state::getMotor( motor motorNode )
{
  //Illegal value
  if ( motorNode >= eMOTOR_NUM_MAX )
    return NULL;

  if( motorNode == eMotorLeft )
    return &leftMotor;
  else if ( motorNode == eMotorRight )
    return &rightMotor;
}

void state::moveForward()
{
	motorState* pLeft = getMotor(eMotorLeft);
	motorState* pRight = getMotor(eMotorRight);

	if(pLeft != NULL && pRight != NULL )
	{
		pLeft->setMotorState(eMotorForward);
		pRight->setMotorState(eMotorForward);
	}
}

void state::moveBack()
{
	motorState* pLeft = getMotor(eMotorLeft);
	motorState* pRight = getMotor(eMotorRight);

	if(pLeft != NULL && pRight != NULL )
	{
		pLeft->setMotorState(eMotorBackward);
		pRight->setMotorState(eMotorBackward);
	}
}

void state::turnLeft()
{
	motorState* pLeft = getMotor(eMotorLeft);
	motorState* pRight = getMotor(eMotorRight);

	if(pLeft != NULL && pRight != NULL )
	{
		pLeft->setMotorState(eMotorBackward);
		pRight->setMotorState(eMotorForward);
	}	
}

void state::turnRight()
{
	motorState* pLeft = getMotor(eMotorLeft);
	motorState* pRight = getMotor(eMotorRight);

	if(pLeft != NULL && pRight != NULL )
	{
		pLeft->setMotorState(eMotorForward);
		pRight->setMotorState(eMotorBackward);
	}	
}


void state::stop()
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