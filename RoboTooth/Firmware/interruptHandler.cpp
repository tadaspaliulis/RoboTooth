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
    switch (pin)
    {
    case pinMapping.interrupts.interrupt1:
        interruptForPin = &interrupts[0];
        return []() { interruptService::handleInterrupt(0); };
    case pinMapping.interrupts.interrupt2:
        interruptForPin = &interrupts[1];
        return []() { interruptService::handleInterrupt(1); };
    case pinMapping.interrupts.interrupt3:
        interruptForPin = &interrupts[2];
        return []() { interruptService::handleInterrupt(2); };
    case pinMapping.interrupts.interrupt4:
        interruptForPin = &interrupts[3];
        return []() { interruptService::handleInterrupt(3); };
    case pinMapping.interrupts.interrupt5:
        interruptForPin = &interrupts[4];
        return []() { interruptService::handleInterrupt(4); };
    case pinMapping.interrupts.interrupt6:
        interruptForPin = &interrupts[5];
        return []() { interruptService::handleInterrupt(5); };
    default:
        return nullptr;
    }

    interruptForPin->handler = handler;
    interruptForPin->debounceMs = debounceMs;
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