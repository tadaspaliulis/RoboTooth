#pragma once

#include "constants.h"
#include "messagingService.h"
#include "ArduinoSerialInteface.h"
#include "rotaryEncoderSensor.h"

enum motorStateEnum
{
  eMotorStateInvalid = -1,
  eMotorIdle = 0,
  eMotorForward,
  eMotorBackward,
  eMotorStop,
  eMOTOR_STATE_MAX
};  

enum motor
{
  eMotorLeft = 0,
  eMotorRight,
  eMOTOR_NUM_MAX
};

//Ultrasound distance sensor
class echoSensor
{
  public:
   echoSensor( );
   void initialise( byte TriggerPin, byte EchoPin );

   //Perform 
   float measureDistance();
   float getLastDistanceReading();

 private:
   byte triggerPin;
   byte echoPin;
   float lastDistanceReading;
};

/* Class tracking the state of an individual motor. */
class motorState
{
  public:
    motorState( );
    
    void initialise( byte LogicPin1, byte LogicPin2, byte SpeedPin );

    void setMotorState ( motorStateEnum state );
    motorStateEnum getMotorState ( ) { return state; }

    byte getMotorSpeed ( ) { return motorSpeed; }
    void setMotorSpeed ( byte MotorSpeed );

  protected:
    byte getLogicPin1() const { return logicPin1; }
    byte getLogicPin2() const { return logicPin2; }
    byte getSpeedPin() const { return speedPin; }
  
  private:
    motorStateEnum state;
    byte motorSpeed;

    byte logicPin1;
    byte logicPin2;
    byte speedPin;
};

/*Class for controlling MAG3110 magnetometer*/
class magnetometerSensor
{
public:
  magnetometerSensor();
  void initialise();
  void updateMeasurement();

  int getLastReadingX() { return lastReadX; }
  int getLastReadingY() { return lastReadY; }
  int getLastReadingZ() { return lastReadZ; }

  const int deviceAddress;
private:
  //Helper function, to read 2 sequential bytes out of the i2c bus
  int read2Bytes(bool& dataAvailable);

  int lastReadX;
  int lastReadY;
  int lastReadZ;
};

/* Class tracking the state of  the robot. */
class state
{
  public:
    state();
    
    void initialise();

    void moveForward(byte movementSpeed);
    void moveBack(byte movementSpeed);
    void turnLeft(byte movementSpeed);
    void turnRight(byte movementSpeed);
    void stop(byte movementSpeed);
    void setAllMotorSpeed( byte speed );

    float getDistance();
    int getMagnetometerOrientationX();
    int getMagnetometerOrientationY();
    int getMagnetometerOrientationZ();

    void updateDistanceMeasurement();
    void updateMagnetometerMeasurement();

    void printEncoderCounters();

    messagingService<arduinoSerialInterface>* getMessenger() { return messagingService<arduinoSerialInterface>::getMessenger(); }

  private:
    motorState* getMotor( motor motorNode );
    
    echoSensor distanceUltraSoundSensor;
    magnetometerSensor magnetometer;

    motorState leftMotor;
    motorState rightMotor;

    rotaryEncoderSensor leftWheelEncoder;
    rotaryEncoderSensor rightWheelEncoder;
};
