#pragma once

#include "interruptHandler.h"

class rotaryEncoderSensor : public interruptHandler
{
public:
    unsigned int getCounter() { return counter; }

    void reset() { counter = 0; }

    void handleInterrupt() { counter += 1; }

    void initialise(int interruptPin, int debounceMs)
    {
        interruptService::addHandlerWithDebounce(interruptPin, this, FALLING, debounceMs);
    }

private:
    volatile unsigned int counter = 0;
};
