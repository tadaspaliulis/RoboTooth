#include "pch.h"
#include "../Firmware/serializer.h"
#include "../Firmware/ArduinoFunctionallity.h"

TEST(noSerialization, expectedLength)
{
    byte container[2] = { 0 };
    serializer s(container);

    ASSERT_EQ(0, s.getDataLength());
    ASSERT_EQ(0x00, container[0]);
    ASSERT_EQ(0x00, container[1]);
}

TEST(serializeInt, writesExpectedData)
{
    byte container[5] = { 0 };
    serializer s(container);
    
    int data = 0xaabbccdd;

    s.serializeInt(data);

    ASSERT_EQ(sizeof(int), s.getDataLength());
    ASSERT_EQ(0xdd, container[0]);
    ASSERT_EQ(0xcc, container[1]);
    ASSERT_EQ(0xbb, container[2]);
    ASSERT_EQ(0xaa, container[3]);

    ASSERT_EQ(0x0, container[4]);
}

TEST(serializeLong, writesExpectedData)
{
    // On Windows long is the same as int, but not on arduino
    byte container[5] = { 0 };
    serializer s(container);

    long data = 0xaabbccdd;

    s.serializeLong(data);

    ASSERT_EQ(sizeof(long), s.getDataLength());
    ASSERT_EQ(0xdd, container[0]);
    ASSERT_EQ(0xcc, container[1]);
    ASSERT_EQ(0xbb, container[2]);
    ASSERT_EQ(0xaa, container[3]);

    ASSERT_EQ(0x0, container[4]);
}

TEST(serializeFloat, writesExpectedData)
{
    byte container[5] = { 0 };
    serializer s(container);

    // Slightly awkward setup because there is no way
    // to assign raw data to floats otherwise.
    int raw_data = 0xaabbccdd;
    float data;
    memcpy(&data, &raw_data, sizeof(float));

    s.serializeFloat(data);

    ASSERT_EQ(sizeof(float), s.getDataLength());
    ASSERT_EQ(0xdd, container[0]);
    ASSERT_EQ(0xcc, container[1]);
    ASSERT_EQ(0xbb, container[2]);
    ASSERT_EQ(0xaa, container[3]);

    ASSERT_EQ(0x0, container[4]);
}

TEST(serializebyte, writesExpectedData)
{
    byte container[3] = { 0 };
    serializer s(container);

    byte data = 0xaa;

    s.serializeByte(data);

    ASSERT_EQ(sizeof(byte), s.getDataLength());
    ASSERT_EQ(0xaa, container[0]);
    ASSERT_EQ(0x0, container[1]);
    ASSERT_EQ(0x0, container[2]);
}

TEST(serializeByteIntLong, writesExpectedData)
{
    byte container[10] = { 0 };
    serializer s(container);

    byte dataByte = 0x11;
    s.serializeByte(dataByte);

    int dataInt = 0xaabbccdd;
    s.serializeInt(dataInt);

    long dataLong = 0x23456789;
    s.serializeLong(dataLong);

    ASSERT_EQ(sizeof(byte) + sizeof(int) + sizeof(long), s.getDataLength());
    ASSERT_EQ(0x11, container[0]);

    ASSERT_EQ(0xdd, container[1]);
    ASSERT_EQ(0xcc, container[2]);
    ASSERT_EQ(0xbb, container[3]);
    ASSERT_EQ(0xaa, container[4]);

    ASSERT_EQ(0x89, container[5]);
    ASSERT_EQ(0x67, container[6]);
    ASSERT_EQ(0x45, container[7]);
    ASSERT_EQ(0x23, container[8]);

    ASSERT_EQ(0x00, container[9]);
}