#pragma once

class interruptHandler
{
public:
    interruptHandler() {}

    virtual void handleInterrupt() = 0;
};

class interruptService
{
public:
    static void addHandler(int interruptPin, interruptHandler* handler, int mode);
    static void addHandlerWithDebounce(int interruptPin, interruptHandler* handler, int mode, int debounceMs);
    static void handleInterrupt(int pin);

private:
    typedef void (*interruptServiceRoutine)();

    struct interruptData
    {
        interruptHandler* handler;
        unsigned int debounceMs;
        unsigned int lastValidInterruptTime;
    };

    static interruptServiceRoutine setupServiceRoutineForHandler(int pin, interruptHandler* handler, int debounceMs);
    static const int MAX_NUMBER_OF_INTERRUPTS = 6;
    static interruptData interrupts[MAX_NUMBER_OF_INTERRUPTS];
};
