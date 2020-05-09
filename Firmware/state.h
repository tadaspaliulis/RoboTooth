#pragma once

#include "constants.h"
#include "messagingService.h"
#include "ArduinoSerialInteface.h"
#include "rotaryEncoderSensor.h"
#include "magnetometerSensor.h"
#include "echoSensor.h"

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

    unsigned int getRotaryEncoderCount(motor motorNode);

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
