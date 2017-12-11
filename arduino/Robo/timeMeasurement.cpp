#include "timeMeasurement.h"
#include "Arduino.h" //This should not be necessary..? but I guess a good idea to have it anyway
/**** Timer implementation ****/

timeMeasurement::timeMeasurement() : timeSinceLastTimestamp(0)
{
	lastTimestamp = millis();
}

unsigned long timeMeasurement::timeStamp()
{
	//Store timestamp and calculate elapsed time since last one
	unsigned long tempTS = lastTimestamp;
	lastTimestamp = millis();

	timeSinceLastTimestamp = lastTimestamp - tempTS;

	return timeSinceLastTimestamp;
}

countdownTimer::countdownTimer() : timeLeft(0)
{
}

bool countdownTimer::updateTimer(unsigned long timeElapsed)
{
	timeLeft -= (long) timeElapsed;

	//Timer reached the end
	return timeLeft <= 0;
}

void countdownTimer::resetTimer(unsigned long timerDuration)
{
	timeLeft += (long) timerDuration;
}

