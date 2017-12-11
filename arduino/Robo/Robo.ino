#include "Arduino.h"
#include "state.h"
#include "controller.h"

#define SERIAL_DEBUG

state State;
controller Controller;

void setup() 
{

	// Initialise all hardware pins, start the bluetooth service
	State.initialise();
	delay(500);

	Controller.initialise(&State);
}

void loop() 
{
	//Get new state data from the sensors
	Controller.updateSensorData();

	//Feed messages to the controller
	message* newMessage = NULL;

	while ( ( newMessage = State.getMessenger()->processMessage() ) != NULL )
	{
		Controller.processMessage( newMessage ); 
	}

	//Behave based on inputs and messages
	//Controller.act();

	delay(10);
  
}
