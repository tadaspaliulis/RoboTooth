#pragma once

class debouncer
{
public:
    debouncer() = default;
    explicit debouncer(unsigned long DebounceMs) : debounceMs{ DebounceMs } { }

    bool isInterruptValid(unsigned long currentTime)
    {
        auto timeSinceLastValidInterrupt = currentTime - lastValidInterrupt;

        if (lastValidInterrupt == 0 || timeSinceLastValidInterrupt >= debounceMs)
        {
            lastValidInterrupt = currentTime;
            return true;
        }
        
        return false;
    }

private:
    unsigned long lastValidInterrupt{ 0 };
    unsigned long debounceMs{ 0 };
};