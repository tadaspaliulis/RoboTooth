package tadas.robotooth;

import android.bluetooth.*;
import android.os.Handler;

import java.io.IOException;
import java.util.UUID;

/**
 * Created by Tadas on 02/01/2016.
 */
public class BluetoothConnectThread extends Thread {
    private final BluetoothSocket mmSocket;
    //private final BluetoothDevice mmDevice;
    private final BluetoothAdapter mmAdapter;
    private final Handler mUIHandler;

    public BluetoothConnectThread(BluetoothDevice device, BluetoothAdapter adapter, Handler UIHandler) {
        // Use a temporary object that is later assigned to mmSocket,
        // because mmSocket is final
        BluetoothSocket tmp = null;
        mUIHandler = UIHandler;
        //mmDevice = device;
        mmAdapter = adapter;
        // Get a BluetoothSocket to connect with the given BluetoothDevice
        try {
            // MY_UUID is the app's UUID string, also used by the server code
            tmp = device.createRfcommSocketToServiceRecord(UUID.fromString("0001101-0000-1000-8000-00805F9B34FB"));
        } catch (IOException e) {
        }
        mmSocket = tmp;
    }

    public void run() {
        // Cancel discovery because it will slow down the connection
        mmAdapter.cancelDiscovery();

        try {
            // Connect the device through the socket. This will block
            // until it succeeds or throws an exception
            mmSocket.connect();
        } catch (IOException connectException) {
            // Unable to connect; close the socket and get out
            try {
                mmSocket.close();
                return;
            } catch (IOException closeException) {
                return;
            }
        }
        // Do work to manage the connection (in a separate thread)
        manageConnectedSocket(mmSocket);
    }

    public void manageConnectedSocket(BluetoothSocket socket)
    {
        new ManageBTConnection(socket, mUIHandler).start();
    }

    /** Will cancel an in-progress connection, and close the socket */
    public void cancel() {
        try {
            mmSocket.close();
        } catch (IOException e) { }
    }
}
