#include "state.h"
#include "Wire.h" //I2C library

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
	digitalWrite(triggerPin, LOW); //Make sure the
	delayMicroseconds(3);
    digitalWrite(triggerPin, HIGH);
    delayMicroseconds(11);
    digitalWrite(triggerPin, LOW);

    //Time till echo turns high, in microseconds DIVIDE BY 58 FOR CENTIMETERS
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
	magnetometer.initialise();
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

//===========Magnetometer sensor implementation
magnetometerSensor::magnetometerSensor() : lastReadX(0.0f), lastReadY(0.0f), lastReadZ(0.0f), deviceAddress(0xc4)
{
}

void magnetometerSensor::initialise()
{
	Wire.begin(); //Join the I2C bus as a master
	//Might have to move this if more devices are added to the same bus

	Wire.beginTransmission(deviceAddress);
	
	//We want to write to Control register (CTRL_REG1)
	Wire.write((byte)0x10);

	Wire.endTransmission();

	//Set up the device behaviour here
	Wire.beginTransmission(deviceAddress);
	//OR[2;0]=100/5Hz
	//OSR[1;0]=00/16
	//FR=0
	//TM=0
	//AC=1
	byte controlByte = 0;
	controlByte = (1 << 7) | 1; //Only need to enable 2 bits!
	Wire.write(controlByte); 
	Wire.endTransmission();
	
}

int magnetometerSensor::read2Bytes()
{

	if(Wire.available() >= 2)
	{
		x = Wire.read() << 8;
		x |= Wire.read();
	} 
}

void magnetometerSensor::updateMeasurement()
{
	Wire.beginTransmission(deviceAddress);

	Wire.write()

	Wire.endTransmission();

	Wire.beginTransmission(deviceAddress);
	Wire.write((byte)0x01); //register address, this is where we're gonna start reading
	Wire.requestFrom(deviceAddress, 6);

	bool dataAvailable; //False if there was no data available
	int x = read2Bytes(dataAvailable);
	int y = read2Bytes(dataAvailable);
	int z = read2Bytes(dataAvailable);

	Wire.endTransmission();

	lastReadX = x;
	lastReadY = y;
	lastReadZ = z;
}

//Helper function, to read 2 sequential bytes out of the i2c bus
int magnetometerSensor::read2Bytes(bool& dataAvailable)
{
	if(Wire.available() >= 2)
	{
		dataAvailable = true;
		x = Wire.read() << 8;
		x |= Wire.read();
	}
	else
	{
		dataAvailable = false;
	}
}