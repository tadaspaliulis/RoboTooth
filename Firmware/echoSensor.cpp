#include "echoSensor.h"
#include "ArduinoFunctionallity.h"

void echoSensor::initialise(byte TriggerPin, byte EchoPin)
{
    triggerPin = TriggerPin;
    echoPin = EchoPin;

    //Set up the pins
    pinMode(triggerPin, OUTPUT);
    pinMode(echoPin, INPUT);
}

float echoSensor::getLastDistanceReading()
{
    return lastDistanceReading;
}

float echoSensor::measureDistance()
{
    //Trigger the echo
    digitalWrite(triggerPin, LOW);
    delayMicroseconds(3);
    digitalWrite(triggerPin, HIGH);
    delayMicroseconds(11);
    digitalWrite(triggerPin, LOW);

    //Time till echo turns high, in microseconds DIVIDE BY 58 FOR CENTIMETERS
    lastDistanceReading = (float)pulseIn(echoPin, HIGH) / 2;

    return lastDistanceReading;
}