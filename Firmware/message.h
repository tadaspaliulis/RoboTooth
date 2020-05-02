#pragma once
#include "ArduinoFunctionallity.h"

/**** Message format ***/
/* preamble/start of frame (2 bytes) + data length (1 byte) + messageId ( 1 byte ) + data( x bytes )
*/
struct message
{
	static const int maximumDataLength = 24;

	byte dataLength;
	byte id;
	byte messageData[maximumDataLength];
};