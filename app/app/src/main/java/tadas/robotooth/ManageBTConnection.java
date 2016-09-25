package tadas.robotooth;

import android.bluetooth.*;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import android.os.Handler;

/**
 * Created by Tadas on 03/01/2016.
 */
public  class   ManageBTConnection extends Thread {
    private final BluetoothSocket mmSocket;
    private final InputStream mmInStream;
    private final OutputStream mmOutStream;
    private final Handler mConnectionEventHandler;
    public ManageBTConnection(BluetoothSocket socket, Handler connectionEventHandler) {
        mmSocket = socket;
        InputStream tmpIn = null;
        OutputStream tmpOut = null;
        mConnectionEventHandler = connectionEventHandler;
        // Get the input and output streams, using temp objects because
        // member streams are final
        try {
            tmpIn = socket.getInputStream();
            tmpOut = socket.getOutputStream();
        } catch (IOException e) { }

        mmInStream = tmpIn;
        mmOutStream = tmpOut;
    }

    public void run() {
        byte[] buffer = new byte[256];  // buffer store for the stream
        int numOfbytes; // bytes returned from read()

        mConnectionEventHandler.obtainMessage(RoboConstants.CONNECTION_ESTABLISHED, this)
                .sendToTarget();

        // Keep listening to the InputStream until an exception occurs
        while (true) {
            try {
                // Read from the InputStream
                numOfbytes = mmInStream.read(buffer);
                byte[] dataPackage = new byte[numOfbytes];
                System.arraycopy(buffer, 0, dataPackage, 0, numOfbytes);
                // Send the obtained bytes to the UI activity
                mConnectionEventHandler.obtainMessage(RoboConstants.BT_DATA_RECEIVED, numOfbytes, -1, dataPackage)
                        .sendToTarget();
            } catch (IOException e) {
                break;
            }
        }
    }

    /* Call this from the main activity to send data to the remote device */
    public void write(byte[] bytes) {
        try {
            mmOutStream.write(bytes);
        } catch (IOException e) { }
    }

    /* Call this from the main activity to shutdown the connection */
    public void cancel() {
        try {
            mmSocket.close();
        } catch (IOException e) { }
    }
}
