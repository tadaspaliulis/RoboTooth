#include "serializer.h"

void serializer::serializeInt(int data)
{
    size_t dataSize = sizeof(int);
    serializeFromVoidPointer(&data, dataSize);
}

void serializer::serializeLong(long data)
{
    size_t dataSize = sizeof(data);
    serializeFromVoidPointer(&data, dataSize);
}

void serializer::serializeByte(byte data)
{
    size_t dataSize = sizeof(byte);
    serializeFromVoidPointer(&data, dataSize);
}

void serializer::serializeFloat(float data)
{
    size_t dataSize = sizeof(float);
    serializeFromVoidPointer(&data, dataSize);
}

void serializer::serializeFromVoidPointer(void* data, size_t dataSize)
{
    memcpy(destination + writtenDataLength, data, dataSize);
    writtenDataLength += dataSize;
}