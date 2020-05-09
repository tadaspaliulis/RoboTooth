#include "ArduinoEmptyFunctions.h"

void delay(int x) {}
void delayMicroseconds(int x) {}
unsigned long millis() { return 0; }

void pinMode(int x, int y) {}
void digitalWrite(int x, int y) {}

void analogWrite(int x, char y) {}
unsigned long pulseIn(int x, int y) { return 0; }

int digitalPinToInterrupt(int p ) { return p; }

void attachInterrupt(int interruptPin, void (*interruptService)(), int mode) {}