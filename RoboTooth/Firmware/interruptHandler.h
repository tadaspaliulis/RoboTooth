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
    static void handleInterrupt(int pin);

private:
    typedef void (*interruptServiceRoutine)();

    static interruptServiceRoutine setupServiceRoutineForHandler(int pin, interruptHandler* handler);
    static const int MAX_NUMBER_OF_INTERRUPTS = 6;
    static interruptHandler* handlers[MAX_NUMBER_OF_INTERRUPTS];
};
