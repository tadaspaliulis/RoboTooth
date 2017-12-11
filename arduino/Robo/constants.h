#pragma once
#include "Arduino.h"

struct
{
    struct
    {

      const static byte leftLogic1 = 12;
      const static byte leftLogic2 = 13;
      const static byte leftSpeed = 11;
      
      const static byte rightLogic1 = 2;
      const static byte rightLogic2 = 4;
      const static byte rightSpeed = 3;

    } motors;

    struct 
    {
      const static byte echoTrig = 7;
      const static byte echoReceive = 8;
    } sensors;
    
 } pinMapping;

 struct
 {
    const static int sendBufferSize = 28;//24 + 2 + 1 + 1;
    const static int bufferSize = 256;
    const static int minimumMessageLength = 6;
    const static int maximumMessageDataLength = 24;
    struct
    {
      const static unsigned int echoDistancePeriod = 100; //10Hz or every 100ms
      const static unsigned int magnetometerPeriod = 200; //5Hz or every 200ms
    } timers;

    struct 
    {
      const static byte heartbeatMsg = 0; //NOT USED
      const static byte echoDistanceMsg = 0;
      const static byte magnetometerDataMsg = 1;
      const static byte actionCompletedMsg = 2;
    } messageIdTx;

    struct 
    {
      const static byte moveControl = 1;
    } messageIdRx;

 } constants;