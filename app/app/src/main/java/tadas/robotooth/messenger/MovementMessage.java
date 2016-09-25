package tadas.robotooth.messenger;

/**
 * Created by Tadas on 21/08/2016.
 */
public class MovementMessage extends RawMessage {
    //Maybe enum is not the best way to accomplish this?
    public enum movementDirection{
        eStop,
        eForward,
        eBackwards,
        eTurnLeft,
        eTurnRight,
    }
    public MovementMessage(movementDirection direction, byte speed){
        data = new byte[2];
        data[0] = (byte)direction.ordinal();
        data[1] = speed;
        messageId = 1; //TURN THIS INTO A COSNTANT
    }

}
