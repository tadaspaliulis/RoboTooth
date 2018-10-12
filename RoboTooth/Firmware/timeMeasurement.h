#pragma once

class timeMeasurement
{
public:
	timeMeasurement();

	//Returns time elapsed
	unsigned long timeStamp();

	//In milliseconds
	unsigned long getTimeElapsed() { return timeSinceLastTimestamp; }
	unsigned long getLastTimestamp() { return lastTimestamp; } 
private:
	unsigned long lastTimestamp;
	unsigned long timeSinceLastTimestamp;
};

class countdownTimer
{
public:
	countdownTimer();
	bool updateTimer(unsigned long timeElapsed);
	void resetTimer(unsigned long timerDuration);
private:
	long timeLeft; //Should this be unsigned?
};