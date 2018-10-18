#pragma once

/* A queue class for FIFO item storage.*/
template<class T>
class queue
{
public:
	queue(int MaximumCapacity);

	~queue();

	int getMaximumCapacity() const { return maximumCapacity; } 

	//Returns a pointer to an item at virtual index.
	//Returns null if there's nothing at the index.
	T* get(int index);

	//Retrieves the first element of the queue.
	//Returns nullptr if the queue is empty.
	T* first();

	//Retrieves the last element of the queue.
	//Returns nullptr if the queue is empty.
	T* last();

	//Attemps to add a new item.
	//If unsuccessful (i.e. the add would overwrite still active data) returns false.
	bool tryAdd(T item);

	//Replaces the item stored at the provided index.
	//Does nothing if the index is outside the bounds of the queue.
	void replace(int index, T replacementItem);

	//Removes the item at the front of the queue.
	//Does nothing if the queue is empty.
	void popFront();

	//Removes several items at the front of the queue.
	//If the parameter exceeds or equals the size of the queue, the queue will become empty.
	//Does nothing if the queue is already empty.
	void popFront(int numberOfItemsToPop);

	//Returns the number of items currently in the queue.
	int getSize();

private:
	T* itemStorage;
	int maximumCapacity;

	//Index of the first item in the queue.
	int startOfQueueIndex;

	//Number of items stored in the queue.
	int itemCount;

};

#pragma region Implementations

template<class T>
queue<T>::queue(int MaximumCapacity) : maximumCapacity(MaximumCapacity), startOfQueueIndex(0), itemCount(0)
{
	//Create the internal storage for the queue items.
	itemStorage = new T[maximumCapacity];
}

template<class T>
queue<T>::~queue()
{
	delete [] itemStorage;
}

template<class T>
T* queue<T>::get(int index)
{
	if(index >= getSize())
		return nullptr;

	//The parameter is a virtual index, which means that
	//externally it looks like the queue always starts at index 0.
	//However, internally the start index changes every time an item is removed from the front.
	//So here we convert to the 'real' index.
	auto realIndex = (startOfQueueIndex + index) % getMaximumCapacity();
	return &itemStorage[realIndex];
}

template<class T>
T* queue<T>::first()
{
	if(getSize() == 0)
		return nullptr;

	return &itemStorage[startOfQueueIndex];
}

template<class T>
T* queue<T>::last()
{
	if(getSize() == 0)
		return nullptr;

	auto lastItemIndex = (startOfQueueIndex + getSize() - 1) % getMaximumCapacity();

	return &itemStorage[lastItemIndex];
}

template<class T>
bool queue<T>::tryAdd(T item)
{
	//Check if we've already filled up the queue.
	if (getSize() == getMaximumCapacity())
		return false;
	
	//Then add the new item to the queue.
	auto insertIndex = (startOfQueueIndex + itemCount) % getMaximumCapacity();
	itemStorage[insertIndex] = item;
	++itemCount;

	return true;
}

template<class T>
void queue<T>::replace(int index, T item)
{
	if(index >= getSize())
		return;
	
	//Replace the existing item with the new one.
	auto realReplacementIndex = (startOfQueueIndex + index) % getMaximumCapacity();
	itemStorage[realReplacementIndex] = item;
}

template<class T>
void queue<T>::popFront()
{
	popFront(1);
}

template<class T>
void queue<T>::popFront(int numberOfItemsToPop)
{
	//Make sure there's something to pop.
	if (getSize() == 0)
		return;

	//Check if we're poping all of the items in the queue.
	if (numberOfItemsToPop >= getSize())
	{
		itemCount = 0;
		return;
	}
	//Remove first x items from the queue by moving the index forward and reducing the size;
	startOfQueueIndex = (startOfQueueIndex + numberOfItemsToPop) % getMaximumCapacity();
	itemCount -= numberOfItemsToPop;
}

template<class T>
int queue<T>::getSize()
{
	return itemCount;
}

#pragma endregion