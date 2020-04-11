#pragma once

#include "state.h"
#include "actionQueue.h"
#include "motorAction.h"
#include "timeMeasurement.h"

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
	void sendActionQueueActionCompleted(byte queueId, byte actionId);


	//Main logic function
	void act();

protected:
    state* getState();
    timeMeasurement sensorTimer;
    timeMeasurement actionQueueTimer;
private:
	actionQueue<motorAction> motorActionQueue;

	countdownTimer echoSensorCountdown;
	countdownTimer magnetometerSensorCountdown;
    countdownTimer rotaryEncoderSensorCountdown;
	
	state* pState;

	//Message handlers
	bool handleMoveMessage(message* msg);
};

