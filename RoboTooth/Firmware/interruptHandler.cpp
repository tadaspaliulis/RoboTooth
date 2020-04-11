#include "interruptHandler.h"
#include "ArduinoFunctionallity.h"
#include "Debug.h"
#include "constants.h"

interruptHandler* interruptService::handlers[MAX_NUMBER_OF_INTERRUPTS];

void interruptService::addHandler(int interruptPin, interruptHandler* handler, int mode)
{
    auto serviceRoutine = setupServiceRoutineForHandler(interruptPin, handler);

    if (serviceRoutine == nullptr)
    {
        SendStringToApp("Invalid interrupt pin used.");
        return;
    }

    attachInterrupt(digitalPinToInterrupt(interruptPin), 
                    serviceRoutine,
                    mode);
}

interruptService::interruptServiceRoutine interruptService::setupServiceRoutineForHandler(int pin, interruptHandler* handler)
{
    switch (pin)
    {
    case pinMapping.interrupts.interrupt1:
        handlers[0] = handler;
        return []() { interruptService::handleInterrupt(0); };
    case pinMapping.interrupts.interrupt2:
        handlers[1] = handler;
        return []() { interruptService::handleInterrupt(1); };
    case pinMapping.interrupts.interrupt3:
        handlers[2] = handler;
        return []() { interruptService::handleInterrupt(2); };
    case pinMapping.interrupts.interrupt4:
        handlers[3] = handler;
        return []() { interruptService::handleInterrupt(3); };
    case pinMapping.interrupts.interrupt5:
        handlers[4] = handler;
        return []() { interruptService::handleInterrupt(4); };
    case pinMapping.interrupts.interrupt6:
        handlers[5] = handler;
        return []() { interruptService::handleInterrupt(5); };
    }

    //Invalid pin
    return nullptr;
}

void interruptService::handleInterrupt(int pin)
{
    handlers[pin]->handleInterrupt();
}