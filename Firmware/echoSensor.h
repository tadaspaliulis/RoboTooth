#pragma once
#include "ArduinoFunctionallity.h"

class echoSensor
{
public:
    echoSensor() = default;
    void initialise(byte TriggerPin, byte EchoPin);

    float measureDistance();
    float getLastDistanceReading();

private:
    byte triggerPin{ 0 };
    byte echoPin{ 0 };
    float lastDistanceReading{ 0.0f };
};