package tadas.robotooth.messenger;
import android.os.Handler;
import android.os.Message;
import android.provider.ContactsContract;
import android.util.Log;

import java.nio.ByteBuffer;
import java.util.ArrayList;
import java.util.concurrent.locks.ReentrantLock;

import tadas.robotooth.ManageBTConnection;
import tadas.robotooth.RoboConstants;
import tadas.robotooth.utility.RollingContainer;

/**
 * Created by Tadas on 04/06/2016.
 */
public class MessagingService extends Handler {

    private RollingContainer receivedDataBuffer;

    private ReentrantLock DataWriteLock = new ReentrantLock();

    private int receivedDataByteCount;
    private final byte  startOfFrame = (byte)0xbb;
    private final int startOfFrameIdLength = 2;
    private final Handler mReceiveHandle;
    private ManageBTConnection BlueToothConnectionManager;

    public MessagingService(Handler receiveHandle)
    {
        mReceiveHandle = receiveHandle;
        receivedDataBuffer = new RollingContainer(1024);
    }

    public synchronized void handleMessage(Message msg) {

        switch(msg.what)
        {
            case RoboConstants.BT_DATA_RECEIVED:
                byte[] receivedDataBuffer = (byte[]) msg.obj;

                HandleReceivedRawData(receivedDataBuffer);
                break;
            case RoboConstants.CONNECTION_ESTABLISHED:
                //Start performing messaging services, inform upper layers of established connection
                BlueToothConnectionManager = (ManageBTConnection) (msg.obj);

                //Let the connection handler know too!
                mReceiveHandle.obtainMessage(RoboConstants.CONNECTION_ESTABLISHED)
                        .sendToTarget();
                break;
        }


    }

    public void sendMessage(RawMessage message){
        if(BlueToothConnectionManager == null)
            throw new RuntimeException("Send failed. BluetoothConnectionManager is not initialised.");

        int messageSize = message.data.length + 1 + 2 + 1; //1 for length, 1 for id, 2 for frame
        byte [] sendBuffer = new byte[messageSize];

        System.arraycopy(message.data, 0, sendBuffer, 4, message.data.length);

        sendBuffer[2] = (byte)message.data.length;
        sendBuffer[3] = message.messageId;
        sendBuffer[0] = sendBuffer[1] = (byte)0xbb;

        BlueToothConnectionManager.write(sendBuffer);
    }

    private void RawDataLogDump(String tag, byte[] rawData) {
        StringBuilder output = new StringBuilder();
        output.append("RawData.length: " + rawData.length + " ");
        for (byte r:rawData) {
            output.append(String.format("%02x ", r));
        }
        Log.v(tag, output.toString());
    }
    protected void HandleReceivedRawData(byte[] rawData) {
        DataWriteLock.lock();
        try {

            RawDataLogDump("IncomingDataDump", rawData);

            StoreRawData(rawData);
            //Analyse received data and look for messages
            RawMessage message;
            while ((message = LookForMessagesInRawData()) != null) {
                mReceiveHandle.obtainMessage(RoboConstants.ROBO_MESSAGE_RECEIVED, 0, 0, message)
                        .sendToTarget();
            }
        } finally {
            DataWriteLock.unlock();
        }
    }

    //Returns an index of the first byte of the preamble
    //If nothing found returns -1
    private int FindStartOfFrame(int startPosition){
        for(int i = 0; i < receivedDataBuffer.getAvailableDataSize() - 1; ++i){
            //Log.v("FindStartOfFrame", "i: " + receivedDataBuffer.read(i) + " i + 1: " + receivedDataBuffer.read(i + 1));

            if(receivedDataBuffer.read(i) == startOfFrame &&
                    receivedDataBuffer.read(i + 1) == startOfFrame)

                return i;
        }

        return -1;
    }

    /*** Message format ***
    /* preamble/start of frame (2 bytes) + data length (1 byte) + messageId ( 1 byte ) + data( x bytes )
    */
    protected RawMessage LookForMessagesInRawData(){
        //Check if there's enough data for at least one message
        //Log.v("smt", "Starting LookForMessages function");
        if(receivedDataBuffer.getAvailableDataSize() < getMinimumMessageSize()) {
            //Log.v("smt", "Not enough data: " + receivedDataBuffer.getAvailableDataSize());
            return null; //Don't have enough data yet, do nothing
        }
        //Log.v("smt", "There's enough available data. " + receivedDataBuffer.getAvailableDataSize());
        int startOfFrame = FindStartOfFrame(0);
        if(startOfFrame < 0)//Negative if start of frame not found
            return null;

        //Log.v("smt", "Found start of frame at ." + startOfFrame);
        int readLocation = startOfFrame + startOfFrameIdLength;
        if(readLocation + 1  > receivedDataBuffer.getAvailableDataSize())
            return null;

        int messageLength = receivedDataBuffer.read(readLocation++);

        //Make sure we have received enough data to read the msg id and the full data payload
        if(messageLength + 1 > (receivedDataBuffer.getAvailableDataSize() - readLocation))
            return null;

        byte messageId = receivedDataBuffer.read(readLocation++);

        RawMessage message = new RawMessage();
        message.messageId = messageId;
        message.data = receivedDataBuffer.copy(readLocation, messageLength);

        float f = ByteBuffer.wrap(message.data).getFloat();
        //Log.v("MsgFound", "Returned Value: " + Float.toString(f));
        //RawDataLogDump("DistanceValueRaw", message.data);
        //Log.v("MsgFound", "Found message.ID:" + messageId + " messageLength:" + messageLength);
        //Discard the 'read' data
        receivedDataBuffer.discardData(readLocation + messageLength);

        return message;
    }

    private int getMinimumMessageSize(){
        return 4; //Replace this with something smarter!
    }

    //Hold data till it gets analyzed
    private void StoreRawData(byte[] rawData){

        receivedDataBuffer.write(rawData);

        receivedDataByteCount += rawData.length;
    }
}
