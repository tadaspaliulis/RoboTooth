#pragma once

#include "constants.h"
#include "messagingService.h"

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

/* Class tracking the state of  the robot. */
class state
{
  public:
    state();
    
    void initialise();

    void moveForward();
    void moveBack();
    void turnLeft();
    void turnRight();
	  void stop();
    void setAllMotorSpeed( byte speed );

    float getDistance();
    void updateDistanceMeasurement();

    messagingService* getMessenger() { return &messenger; }

  private:
    motorState* getMotor( motor motorNode );
    
    echoSensor distanceUltraSoundSensor;

    motorState leftMotor;
    motorState rightMotor;

    messagingService messenger;
};

