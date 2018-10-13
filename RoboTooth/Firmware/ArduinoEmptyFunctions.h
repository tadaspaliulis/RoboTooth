#pragma once

/*  Empty definitions of various arduino functions to make compillation in VS possible. 
	Won't get used on the arduino itself. */

//Arduino defines a byte type which is in fact just one byte.
typedef char byte;

//Need the ifdef to make sure that arduino IDE won't try to import the includes,
//Even though it shouldn't attempt to compile this file at all!
#ifdef VS_TESTBED
#include <memory>
#include "SerialEmpty.h"
#endif

void delay(int x);
void delayMicroseconds(int x);
int millis();

void pinMode(int x, int y);
void digitalWrite(int x, int y);

void analogWrite(int x, char y);
char pulseIn(int x, int y);

//Create our own definitions of the various arduino constants.
const int HIGH = 1;
const int LOW = 0;

const int OUTPUT = 1;
const int INPUT = 0;
