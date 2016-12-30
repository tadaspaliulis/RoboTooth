package tadas.robotooth;

import android.app.Activity;
import android.content.Intent;
import android.os.Message;
import android.os.Bundle;
import android.view.MotionEvent;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.bluetooth.*;
import android.os.Handler;
import java.lang.System;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.util.Set;

import tadas.robotooth.messenger.MessagingService;
import tadas.robotooth.messenger.MovementMessage;
import tadas.robotooth.messenger.RawMessage;

public class MainActivity extends Activity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        btAdapter = BluetoothAdapter.getDefaultAdapter();
        messagingService = new MessagingService(handler);
        //Retrieve button views
        buttonConnect = (Button) findViewById(R.id.button_connect);
        buttonForward = (Button) findViewById(R.id.button_move_forward);
        buttonBackward = (Button) findViewById(R.id.button_move_back);
        buttonLeft = (Button) findViewById(R.id.button_turn_left);
        buttonRight = (Button) findViewById(R.id.button_turn_right);

        buttonForward.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                if (event.getAction() == MotionEvent.ACTION_DOWN) {
                    // Pressed
                    moveForwardButton();
                } else if (event.getAction() == MotionEvent.ACTION_UP) {
                    // Released
                    stopRobotButtonReleased();
                }
                return true;
            }
        });

        buttonBackward.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                if (event.getAction() == MotionEvent.ACTION_DOWN) {
                    // Pressed
                    moveBackwardsButton();
                } else if (event.getAction() == MotionEvent.ACTION_UP) {
                    // Released
                    stopRobotButtonReleased();
                }
                return true;
            }
        });

        buttonLeft.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                if (event.getAction() == MotionEvent.ACTION_DOWN) {
                    // Pressed
                    turnLeftButton();
                } else if (event.getAction() == MotionEvent.ACTION_UP) {
                    // Released
                    stopRobotButtonReleased();
                }
                return true;
            }
        });

        buttonRight.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                if (event.getAction() == MotionEvent.ACTION_DOWN) {
                    // Pressed
                    turnRightButton();
                } else if (event.getAction() == MotionEvent.ACTION_UP) {
                    // Released
                    stopRobotButtonReleased();
                }
                return true;
            }
        });
    }

    //Connect button handler
    public void connectButton(View view) {
        TextView text = (TextView) findViewById(R.id.text_connection_status);
        text.setText(R.string.text_connection_label_searching);

        //Request Bluetooth enable
        if (!btAdapter.isEnabled()) {
            Intent enableBtIntent = new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE);
            startActivityForResult(enableBtIntent, REQUEST_ENABLE_BT);
        } else {
            findBluetoothDevicesAndConnect();
        }
    }

    public void sendRobotMessage( RawMessage message) {

        messagingService.sendMessage(message);
    }

    public void moveForwardButton(){
        sendRobotMessage(new MovementMessage(MovementMessage.movementDirection.eForward, (byte)0xff));
    }

    public void moveBackwardsButton(){
        sendRobotMessage(new MovementMessage(MovementMessage.movementDirection.eBackwards, (byte)0xff));
    }

    public void turnLeftButton(){
        sendRobotMessage(new MovementMessage(MovementMessage.movementDirection.eTurnLeft, (byte)0xff));
    }

    public void turnRightButton(){
        sendRobotMessage(new MovementMessage(MovementMessage.movementDirection.eTurnRight, (byte)0xff));
    }

    public void stopRobotButtonReleased() {
        sendRobotMessage(new MovementMessage(MovementMessage.movementDirection.eStop, (byte)0x00));
    }

    //Activity result handler
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        switch (requestCode) {
            case REQUEST_ENABLE_BT:
                if (resultCode == RESULT_OK)
                    findBluetoothDevicesAndConnect();
                break;
            default:
                break;
        }
    }

    protected void findBluetoothDevicesAndConnect() {
        Set<BluetoothDevice> pairedDevices = btAdapter.getBondedDevices();
        // If there are paired devices
        BluetoothDevice roboDevice = null;
        if (pairedDevices.size() > 0) {
            // Loop through paired devices
            for (BluetoothDevice device : pairedDevices) {
                // Add the name and address to an array adapter to show in a ListView
                String s = device.getName();
                if (device.getName().compareTo("RoboTooth") == 0) {
                    roboDevice = device;
                    break;
                }
            }

            //If a paired device is RoboTooth, connect, otherwise perform discovery
            if (roboDevice != null) {
                connectToRobot(roboDevice);
            } else {

            }
        }
    }

    void connectToRobot(BluetoothDevice device) {
        btConnectThread = new BluetoothConnectThread(device, btAdapter, messagingService);
        btConnectThread.start();
    }

    private MessagingService messagingService;
    private BluetoothAdapter btAdapter;
    private BluetoothConnectThread btConnectThread = null;

    private Button buttonLeft;
    private Button buttonRight;
    private Button buttonForward;
    private Button buttonConnect;
    private Button buttonBackward;

    /*Handle blueetooth(?) events*/
    private final Handler handler = new Handler() {

        // Create handleMessage function

        public void handleMessage(Message msg) {
            switch (msg.what) {
                case RoboConstants.CONNECTION_ESTABLISHED:
                    TextView text = (TextView) findViewById(R.id.text_connection_status);
                    text.setText(R.string.text_connection_label_connected);

                    //Bluetooth connection established, Enable Robo Control buttons
                    buttonLeft.setEnabled(true);
                    buttonRight.setEnabled(true);
                    buttonForward.setEnabled(true);
                    buttonBackward.setEnabled(true);
                    buttonConnect.setEnabled(false);

                    //Can now start the sending/writing messages?

                    break;
                case RoboConstants.ROBO_MESSAGE_RECEIVED:
                    TextView distanceDisplay = (TextView) findViewById(R.id.text_display_distance);
                    RawMessage message = (RawMessage) msg.obj;

                    if(message.messageId == 0) //TEMP, REPLACE WITH A CONSTANT
                    {
                        float f = ByteBuffer.wrap(message.data).order(ByteOrder.LITTLE_ENDIAN).getFloat();
                        distanceDisplay.setText("Distance: " + Float.toString(f));
                    } else if (message.messageId == 1) //TEMP, REPLACE WITH A CONSTANT
                    {
                        int x = ByteBuffer.wrap(message.data, 0, 4).order(ByteOrder.LITTLE_ENDIAN).getInt();
                        int y = ByteBuffer.wrap(message.data, 4, 4).order(ByteOrder.LITTLE_ENDIAN).getInt();
                        int z = ByteBuffer.wrap(message.data, 8, 4).order(ByteOrder.LITTLE_ENDIAN).getInt();

                        TextView magnetometerDisplay = (TextView) findViewById(R.id.text_display_magnetometer);
                        magnetometerDisplay.setText("X: " + Integer.toString(x)
                                + "Y: " + Integer.toString(y)
                                + "Z: " + Integer.toString(z));
                    }
                    break;
                default:
                    break;
            }
        }
    };

    protected static final int REQUEST_ENABLE_BT = 1;


}
