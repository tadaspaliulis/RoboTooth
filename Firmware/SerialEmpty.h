#pragma once

//Empty implementations of various arduino calls to make compiling in VS possible.
class SerialEmpty
{
public:
	char read()
	{
		return 0x00;
	}

	void println(const char* a ) {}
	void begin(int x) { } 
	int available() { return 0; }
	void write(const char * a) { }
	void write(byte * a, int count) { }
};

static SerialEmpty Serial;
static SerialEmpty Serial1;
static SerialEmpty Serial2;
static SerialEmpty Serial3;