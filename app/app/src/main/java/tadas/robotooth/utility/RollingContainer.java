package tadas.robotooth.utility;

/**
 * Created by Tadas on 20/08/2016.
 */
public class RollingContainer {
    private byte [] buffer;
    private int startOfBuffer;
    private int endOfBuffer;

    public RollingContainer(int bufferSize){
        buffer = new byte[bufferSize];
    }

    //Overall size of the buffer
    public int getMaximumSize(){
        return buffer.length;
    }

    public int getAvailableDataSize(){
        /***[][startOfBuffer][][][endOfBuffer][]]**/
        if(startOfBuffer < endOfBuffer) {
            return endOfBuffer - startOfBuffer;
        }
        else if (startOfBuffer == endOfBuffer) {
            /***[][][][][endOfBuffer:startOfBuffer][][]**/
            return 0;
        } else {
            /***[][endOfBuffer][][][][startOfBuffer][]**/
            return getMaximumSize() - startOfBuffer + endOfBuffer;
        }
    }

    public void write(byte b){
        endOfBuffer = (endOfBuffer + 1) % getMaximumSize();
        buffer[endOfBuffer] = b;
    }

    public void write(byte[] dataBlock){
        if(dataBlock.length > getMaximumSize())
            throw new ArrayStoreException("Provided dataBlock (size: " + dataBlock.length + ") is too big. Available size: " + getMaximumSize());

        for(int i = 0; i < dataBlock.length; ++i)
        {
            write(dataBlock[i]);
        }
    }

    public byte read(int index){
        if(index >= getAvailableDataSize())
            throw new ArrayIndexOutOfBoundsException("Rolling buffer index out of bounds.\n Available: " + getAvailableDataSize() + " Index used: " + index);

        int actualReadLocation = (startOfBuffer + index) % getMaximumSize();
        return buffer[actualReadLocation];
    }

    public byte[] copy(int startIndex, int copyLength){
        if(copyLength < 0)
            throw new IllegalArgumentException("RollingContainer.copy function was passed a negative copyLength: " + copyLength);
        if(copyLength > (getAvailableDataSize() - startIndex))
            throw new RuntimeException("Tried to copy " + copyLength + "byte from the RollingBuffer when only " + getAvailableDataSize() + " byte available");

        byte[] copy = new byte[copyLength];
    
        for(int i = 0; i < copyLength; ++i)
        {
            copy[i] = read(startIndex + i);
        }

        return copy;
    }

    //Discards the data at the virtual start of the buffer
    public void discardData(int discardLength){
        if(discardLength >= getAvailableDataSize())
            startOfBuffer = endOfBuffer; // No available data left
        else
            startOfBuffer = (startOfBuffer + discardLength) % getMaximumSize();
    }
}
