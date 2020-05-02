#include "pch.h"
#include "../Firmware/Queue.h"

TEST(tryAdd, returnsTrueOnSuccess)
{
    queue<int> q(1);
    bool successfulAdd = false;
    successfulAdd = q.tryAdd(1);

    EXPECT_TRUE(successfulAdd);
}

TEST(tryAdd, returnsFalseOnMaxSizeReached)
{
    queue<int> q(5);
    bool successfulAdd = false;
    q.tryAdd(1);
    q.tryAdd(2);
    q.tryAdd(3);
    q.tryAdd(4);
    successfulAdd = q.tryAdd(5);

    EXPECT_TRUE(successfulAdd);

    successfulAdd = q.tryAdd(6);
    EXPECT_FALSE(successfulAdd);
}

TEST(popFront, sizeGoesDownAsExpected)
{
    queue<int> q(5);
    q.tryAdd(1);
    (q.getSize() == 1);

    q.popFront();

    ASSERT_EQ(0, q.getSize());
}

TEST(popFront, popMultipleItems)
{
    queue<int> q(4);
    q.tryAdd(1);
    q.tryAdd(2);
    q.tryAdd(3);
    q.tryAdd(4);

    q.popFront(3);

    ASSERT_EQ(4 , *q.get(0));
    ASSERT_EQ(1, q.getSize());
}

TEST(popFront, popMultipleItemsWithWraparound)
{
    queue<int> q(4);
    q.tryAdd(1);
    q.tryAdd(2);
    q.tryAdd(3);
    q.tryAdd(4);
    q.popFront(3);

    q.tryAdd(5);
    q.tryAdd(6);
    q.tryAdd(7);

    q.popFront(3);

    ASSERT_EQ(7, *q.get(0));
    ASSERT_EQ(1, q.getSize());
}

TEST(popFront, popAllItemsSizeBecomes0)
{
    queue<int> q(4);
    q.tryAdd(1);
    q.tryAdd(2);
    q.tryAdd(3);
    q.tryAdd(4);

    q.popFront(4);

    ASSERT_EQ(nullptr, q.get(0));
    ASSERT_EQ(0, q.getSize());
}

TEST(get, getsItemAtIndx)
{
    queue<int> q(4);
    q.tryAdd(12);

    ASSERT_EQ(12, *q.get(0));
}

TEST(get, outOfBoundsGetReturnsNull)
{
    queue<int> q(4);
    q.tryAdd(1);

    ASSERT_EQ(nullptr, q.get(2));
}

TEST(replace, replacesItemAtIndex)
{
    queue<int> q(4);

    q.tryAdd(1);
    q.tryAdd(2);
    q.tryAdd(3);
    ASSERT_EQ(2, *q.get(1));

    q.replace(1, 10);
    ASSERT_EQ(10, *q.get(1));
}

TEST(first, emptyQueueReturnsNull)
{
    queue<int> q(4);
    q.tryAdd(1);
    q.popFront();
    ASSERT_EQ(nullptr, q.first());
}

TEST(first, returnsFirstCurrentItem)
{
    queue<int> q(4);
    q.tryAdd(1);
    q.tryAdd(42);
    q.popFront();
    ASSERT_EQ(42, *q.first());
}

TEST(last, emptyQueueReturnsNull)
{
    queue<int> q(4);
    q.tryAdd(1);
    q.popFront();

    ASSERT_EQ(nullptr, q.last());
}

TEST(last, returnsLastCurrentItem)
{
    queue<int> q(4);
    q.tryAdd(1);
    q.tryAdd(11);
    q.popFront();
    q.tryAdd(42);

    ASSERT_EQ(42, *q.last());
}

TEST(popFrontTryAdd, wrapsAroundAsExpected)
{
    queue<int> q(4);

    q.tryAdd(1);
    q.tryAdd(2);
    q.tryAdd(3);
    q.tryAdd(4);
    ASSERT_EQ(4, q.getSize());

    q.popFront();
    q.popFront();
    q.popFront();
    ASSERT_EQ(1, q.getSize());
    ASSERT_EQ(4, *q.get(0));

    q.tryAdd(5);
    q.tryAdd(6);
    q.tryAdd(7);

    ASSERT_EQ(4, q.getSize());

    ASSERT_EQ(4, *q.get(0));
    ASSERT_EQ(5, *q.get(1));
    ASSERT_EQ(6, *q.get(2));
    ASSERT_EQ(7, *q.get(3));
}