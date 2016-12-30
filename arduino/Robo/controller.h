#pragma once
#include "state.h"

class timeMeasurement
{
public:
	timeMeasurement();

	//Returns time elapsed
	unsigned int timeStamp();

	//In milliseconds
	unsigned int getTimeElapsed() { return timeSinceLastTimestamp; }
	unsigned int getLastTimestamp() { return lastTimestamp; } 
private:
	unsigned int lastTimestamp;
	unsigned int timeSinceLastTimestamp;
};

class countdownTimer
{
public:
	countdownTimer();
	bool updateTimer(unsigned int timeElapsed);
	void resetTimer(unsigned int timerDuration);
private:
	int timeLeft;
};

/*Controller deals with inputs from the Master Device,
sensory data as well as some basic behavioural logic.*/

class controller
{
public:
	controller();
	void initialise(state* State);

	//Deals with commands from the master
	void processMessage(message* msg);

	//Prompts state sensors to get new data
	void updateSensorData();

	//Comms
	void sendEchoDistanceData(float& distances);
	void sendMagnetometerData(int x, int y, int z);

	//Main logic function
	void act();

protected:
    state* getState();
    timeMeasurement timer;

private:
	countdownTimer echoSensorCountdown;
	countdownTimer magnetometerSensorCountdown;
	
	state* pState;

	//Message handlers
	bool handleMoveMessage(message* msg);
};

