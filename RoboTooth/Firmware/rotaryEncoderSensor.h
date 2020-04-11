#pragma once

#include "interruptHandler.h"

class rotaryEncoderSensor : public interruptHandler
{
public:
    unsigned int getCounter() { return counter; }

    void reset() { counter = 0; }

    void handleInterrupt() { counter += 1; }

    void initialise(int interruptPin)
    {
        interruptService::addHandler(interruptPin, this, FALLING);
    }

private:
    volatile unsigned int counter = 0;
};
