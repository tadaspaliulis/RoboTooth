#pragma once

#define DEBUG
#ifdef DEBUG
#include "messagingService.h"
#include "constants.h"
//Global function for sending short debug string to the application.
//Abuses the fact that the messaging service is a singleton
static void SendStringToApp(const char* debugString)
{
	/*size_t stringLength = strlen(debugString);
	if(stringLength >= constants.maximumMessageDataLength)
		stringLength = constants.maximumMessageDataLength - 1;

	message msg;
	msg.id = constants.messageIdTx.debugStringMsg;
	msg.dataLength = stringLength;
	memcpy(msg.messageData, debugString, stringLength);
	//messagingService::getMessenger()->sendMessage(msg);*/
	Serial.println(debugString);
}

#endif