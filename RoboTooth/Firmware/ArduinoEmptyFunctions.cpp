#include "ArduinoEmptyFunctions.h"

void delay(int x) {}
void delayMicroseconds(int x) {}
int millis() { return 0; }

void pinMode(int x, int y) {}
void digitalWrite(int x, int y) {}

void analogWrite(int x, char y) {}
char pulseIn(int x, int y) { return 0; }