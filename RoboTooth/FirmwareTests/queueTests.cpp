#include "pch.h"
#include "../Firmware/Queue.h"

TEST(tryAdd, falseOnMaxSizeReached)
{
	queue<int> q(5);
	bool successfulAdd = false;
	successfulAdd = q.tryAdd(1);
	successfulAdd = q.tryAdd(2);
	successfulAdd = q.tryAdd(3);
	successfulAdd = q.tryAdd(4);
	successfulAdd = q.tryAdd(5);

	EXPECT_TRUE(successfulAdd);

	successfulAdd = q.tryAdd(6);
	EXPECT_FALSE(successfulAdd);
}