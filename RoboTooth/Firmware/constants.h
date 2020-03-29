#pragma once
#include "ArduinoFunctionallity.h"
#include "message.h"

struct pinMappingStruct
{
    struct motorsConstants
    {
	  const static byte leftLogic1 = 51; // Digital Pin 51
	  const static byte leftLogic2 = 50; // Digital Pin 50
	  const static byte leftSpeed = 4; // Digital Pin 4 (PWM)
      
	  const static byte rightLogic1 = 53; // Digital Pin 53
	  const static byte rightLogic2 = 52; // Digital Pin 52
      const static byte rightSpeed = 5; // Digital Pin 5 (PWM)

    } motors;

    struct sensorsConstants
    {
	  const static byte echoTrig = 7;
      const static byte echoReceive = 8;
    } sensors;
 };

 struct constantsStruct
 {
    const static int bufferSize = 256;
	const static int preambleSize = sizeof(byte) * 2;
    const static int maximumMessageDataLength = message::maximumDataLength;
	const static int minimumMessageLength = preambleSize + sizeof(message) - maximumMessageDataLength;
	const static int sendBufferSize = preambleSize + sizeof(message);

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