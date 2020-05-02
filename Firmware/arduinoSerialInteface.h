#pragma once
#include "ArduinoFunctionallity.h"

//Wrapper around various arduino serial interface functions.
class arduinoSerialInterface
{
public:
	void begin(int x)
	{
        Serial.begin(x);
		Serial3.begin(x);
		
		// The serial 3 RX pin needs to be pulled up,
		// otherwise idle state ends up floating and we read garbage data
		pinMode(15, INPUT_PULLUP);
	}

	int available() { return Serial3.available(); }

	byte read() { return Serial3.read(); }

	void println(const char* string) { Serial3.println(string); }
	void write(const char* string) { Serial3.write(string); }
	void write(byte* string, int count) { Serial3.write(string, count); }
};