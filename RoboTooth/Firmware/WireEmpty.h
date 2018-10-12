#pragma once

//Empty implementations of various arduino calls to make compiling in VS possible.
class WireEmpty
{
public:
	void begin() {}
	void beginTransmission(int x) { }
	void write(char x) { }
	void endTransmission() {}
	void requestFrom(int x, int y) {}
	char read() { return 0; }
	int available() { return 0; }
};

static WireEmpty Wire;