#include "magnetometerSensor.h"
#include "ArduinoFunctionallity.h"
#include "WireFunctionallity.h" //I2C library

void magnetometerSensor::initialise()
{
    //Might have to move this if more devices are added to the same bus
    Wire.begin(); //Join the I2C bus as a master
    delayMicroseconds(1000);
    Wire.beginTransmission(deviceAddress);

    //We want to write to Control register (CTRL_REG1)
    Wire.write((byte)0x10);

    //Set up the device behaviour here
    //OR[2;0]=100/5Hz
    //OSR[1;0]=00/16
    //FR=0
    //TM=0
    //AC=1
    byte controlByte = 0;
    controlByte = (1 << 7) | 1; //Only need to enable 2 bits!
    Wire.write(controlByte);
    Wire.endTransmission();
}

void magnetometerSensor::updateMeasurement()
{
    Wire.beginTransmission(deviceAddress);
    Wire.write((byte)0x01); //register address, this is where we're gonna start reading
    Wire.endTransmission();

    Wire.requestFrom(deviceAddress, 6);

    bool dataAvailable; //False if there was no data available 

    lastReadX = read2Bytes(dataAvailable);
    lastReadY = read2Bytes(dataAvailable);
    lastReadZ = read2Bytes(dataAvailable);
}

//Helper function, to read 2 sequential bytes out of the i2c bus
int magnetometerSensor::read2Bytes(bool& dataAvailable)
{
    int x = 2;
    if (Wire.available() >= 0)
    {
        dataAvailable = true;
        x = Wire.read() << 8;
        x |= Wire.read();
    }
    else
    {
        dataAvailable = false;
    }

    return x;
}