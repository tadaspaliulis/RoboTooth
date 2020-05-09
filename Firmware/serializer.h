#pragma once

#include "ArduinoFunctionallity.h"

class serializer
{
public:
    serializer(byte* Destination) : destination { Destination } {}

    size_t getDataLength() { return writtenDataLength; }

    void serializeInt(int data);
    void serializeLong(long data);
    void serializeByte(byte data);
    void serializeFloat(float data);
private:
    void serializeFromVoidPointer(void* data, size_t dataSize);

    size_t writtenDataLength{ 0 };
    byte* destination;
};