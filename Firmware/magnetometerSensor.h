#pragma once

/*Class for controlling MAG3110 magnetometer*/
class magnetometerSensor
{
public:
    magnetometerSensor() = default;
    void initialise();
    void updateMeasurement();

    int getLastReadingX() { return lastReadX; }
    int getLastReadingY() { return lastReadY; }
    int getLastReadingZ() { return lastReadZ; }

    const int deviceAddress = 0x0e;
private:
    //Helper function, to read 2 sequential bytes from the i2c bus
    int read2Bytes(bool& dataAvailable);

    int lastReadX{ 0 };
    int lastReadY{ 0 };
    int lastReadZ{ 0 };
};