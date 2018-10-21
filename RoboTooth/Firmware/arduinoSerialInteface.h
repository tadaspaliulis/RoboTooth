#pragma once
#include "ArduinoFunctionallity.h"

//Wrapper around various arduino serial interface functions.
class arduinoSerialInterface
{
public:
	void begin(int x) { Serial.begin(x); }
	int available() { return Serial.available(); }

	byte read() { return Serial.read(); }

	void println(const char* string) { Serial.println(string); }
	void write( const char* string) { Serial.write(string); }
};