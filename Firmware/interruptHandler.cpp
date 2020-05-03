#include "interruptHandler.h"
#include "ArduinoFunctionallity.h"
#include "Debug.h"
#include "constants.h"

interruptService::interruptData interruptService::interrupts[MAX_NUMBER_OF_INTERRUPTS];

void interruptService::addHandler(int interruptPin, interruptHandler* handler, int mode)
{
    addHandlerWithDebounce(interruptPin, handler, mode, 0);
}

void interruptService::addHandlerWithDebounce(int interruptPin, interruptHandler* handler, int mode, int debounceMs)
{
    auto serviceRoutine = setupServiceRoutineForHandler(interruptPin, handler, debounceMs);

    if (serviceRoutine == nullptr)
    {
        SendStringToApp("Invalid interrupt pin used.");
        return;
    }

    attachInterrupt(digitalPinToInterrupt(interruptPin),
                    serviceRoutine,
                    mode);
}

interruptService::interruptServiceRoutine interruptService::setupServiceRoutineForHandler(int pin, interruptHandler* handler, int debounceMs)
{
    interruptData* interruptForPin;
    interruptServiceRoutine interruptFunction;

    switch (pin)
    {
    case pinMapping.interrupts.interrupt1:
        interruptForPin = &interrupts[0];
        interruptFunction = []() { interruptService::handleInterrupt(0); };
        break;
    case pinMapping.interrupts.interrupt2:
        interruptForPin = &interrupts[1];
        interruptFunction = []() { interruptService::handleInterrupt(1); };
        break;
    case pinMapping.interrupts.interrupt3:
        interruptForPin = &interrupts[2];
        interruptFunction = []() { interruptService::handleInterrupt(2); };
        break;
    case pinMapping.interrupts.interrupt4:
        interruptForPin = &interrupts[3];
        interruptFunction = []() { interruptService::handleInterrupt(3); };
        break;
    case pinMapping.interrupts.interrupt5:
        interruptForPin = &interrupts[4];
        interruptFunction = []() { interruptService::handleInterrupt(4); };
        break;
    case pinMapping.interrupts.interrupt6:
        interruptForPin = &interrupts[5];
        interruptFunction = []() { interruptService::handleInterrupt(5); };
        break;
    default:
        return nullptr;
    }

    interruptForPin->handler = handler;
    interruptForPin->debounceMs = debounceMs;

    return interruptFunction;
}

void interruptService::handleInterrupt(int pin)
{
    auto currentTime = millis();
    auto timeSinceLastValidInterrupt = currentTime - interrupts[pin].lastValidInterruptTime;
    if (timeSinceLastValidInterrupt >= interrupts[pin].debounceMs)
    {
        interrupts[pin].handler->handleInterrupt();
        interrupts[pin].lastValidInterruptTime = currentTime;
    }
}