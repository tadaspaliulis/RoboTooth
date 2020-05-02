using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboTooth.Model.MessagingService
{
    internal class CircularBuffer<T> : IEnumerable<T>
    {
        private T[] buffer;
        private int startOfBuffer;
        private int endOfBuffer;

        public CircularBuffer(int bufferSize)
        {
            buffer = new T[bufferSize];
        }

        public int Count
        {
            get
            {
                return getAvailableDataSize();
            }
        }

        //Overall size of the buffer
        public int getMaximumSize()
        {
            return buffer.Length;
        }

        public int getAvailableDataSize()
        {
            /***[][startOfBuffer][][][endOfBuffer][]]**/
            if (startOfBuffer < endOfBuffer)
            {
                return endOfBuffer - startOfBuffer;
            }
            else if (startOfBuffer == endOfBuffer)
            {
                /***[][][][][endOfBuffer:startOfBuffer][][]**/
                return 0;
            }
            else
            {
                /***[][endOfBuffer][][][][startOfBuffer][]**/
                return getMaximumSize() - startOfBuffer + endOfBuffer;
            }
        }

        public void Add(T[] dataBlock)
        {
            Add(dataBlock, dataBlock.Length);
        }

        public void Add(T[] dataBlock, int length)
        {
            if (length > dataBlock.Length)
                throw new Exception("length is greater than the length of the dataBlock.");
            if (length > getMaximumSize())
                throw new Exception("Requested dataBlock Add length" + length + " is too big. Available size: " + getMaximumSize());

            for (int i = 0; i < length; ++i)
            {
                Add(dataBlock[i]);
            }
        }

        public T Read(int index)
        {
            if (index >= getAvailableDataSize())
                throw new IndexOutOfRangeException("CircularBuffer index out of bounds.\n Available: " + getAvailableDataSize() + " Index used: " + index);

            int actualReadLocation = (startOfBuffer + index) % getMaximumSize();
            return buffer[actualReadLocation];
        }

        public T[] copy(int startIndex, int copyLength)
        {
            if (copyLength < 0)
                throw new InvalidOperationException("CircularBuffer.copy function was passed a negative copyLength: " + copyLength);
            if (copyLength > (getAvailableDataSize() - startIndex))
                throw new InvalidOperationException("Tried to copy " + copyLength + "byte from the CircularBuffer when only " + getAvailableDataSize() + " byte available");

            T[] copy = new T[copyLength];

            for (int i = 0; i < copyLength; ++i)
            {
                copy[i] = Read(startIndex + i);
            }

            return copy;
        }

        //Discards the data at the virtual start of the buffer
        public void discardData(int discardLength)
        {
            if (discardLength >= getAvailableDataSize())
                startOfBuffer = endOfBuffer; // No available data left
            else
                startOfBuffer = (startOfBuffer + discardLength) % getMaximumSize();
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Add(T item)
        {
            //Extend the length of the buffer and rollover in case we've reached the end of the array
            endOfBuffer = (endOfBuffer + 1) % getMaximumSize();
            buffer[endOfBuffer] = item;
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i != getAvailableDataSize(); ++i)
                yield return Read(i);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            List<T> enumerable = new List<T>(getAvailableDataSize());
            for (int i = 0; i != getAvailableDataSize(); ++i)
            {
                enumerable.Add(Read(i));
            }

            return enumerable.GetEnumerator();
        }
    }
}
