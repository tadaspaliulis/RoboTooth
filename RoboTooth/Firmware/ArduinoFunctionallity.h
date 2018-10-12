#pragma once
#ifdef VS_TESTBED
//If this is being built for Visual Studio testing, use the empty function definitions.

//Adding this in to make sprintf's work, which have been for debugging throughout.
#define _CRT_SECURE_NO_WARNINGS
#include "ArduinoEmptyFunctions.h"
#else
//If this is being built for arduino, use the actual arduino libraries.
#include "Arduino.h"
#endif