#pragma once
#include "ArduinoFunctionallity.h"

struct pinMappingStruct
{
    struct motorsConstants
    {

      const static byte leftLogic1 = 12;
      const static byte leftLogic2 = 13;
      const static byte leftSpeed = 11;
      
      const static byte rightLogic1 = 2;
      const static byte rightLogic2 = 4;
      const static byte rightSpeed = 3;

    } motors;

    struct sensorsConstants
    {
      const static byte echoTrig = 7;
      const static byte echoReceive = 8;
    } sensors;
    
 };

 struct constantsStruct
 {
    const static int sendBufferSize = 28;//24 + 2 + 1 + 1;
    const static int bufferSize = 256;
    const static int minimumMessageLength = 6;
    const static int maximumMessageDataLength = 24;

    struct timersConstants
    {
      const static unsigned int echoDistancePeriod = 100; //10Hz or every 100ms
      const static unsigned int magnetometerPeriod = 200; //5Hz or every 200ms
    } timers;

    struct messageIdTxConstants
    {
      const static byte heartbeatMsg = 0; //NOT USED
      const static byte echoDistanceMsg = 0;
      const static byte magnetometerDataMsg = 1;
      const static byte actionCompletedMsg = 2;
      const static byte debugStringMsg = 3;
    } messageIdTx;

    struct messageIdRxConstants
    {
      const static byte moveControl = 1;
    } messageIdRx;

 };

 static constantsStruct constants;
 static pinMappingStruct pinMapping;