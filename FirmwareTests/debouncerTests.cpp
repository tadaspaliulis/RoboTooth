#include "pch.h"
#include "../Firmware/debouncer.h"

TEST(isInterruptValid, notEnoughTimeElapsedReturnsFalse)
{
    debouncer d(1000);

    unsigned long currentTime = 1;

    EXPECT_TRUE(d.isInterruptValid(currentTime));

    currentTime += 10;
    EXPECT_FALSE(d.isInterruptValid(currentTime));
}

TEST(isInterruptValid, enoughTimeElapsedReturnsTrue)
{
    debouncer d(1000);

    unsigned long currentTime = 1;

    EXPECT_TRUE(d.isInterruptValid(currentTime));

    currentTime += 1000;
    EXPECT_TRUE(d.isInterruptValid(currentTime));
}

TEST(isInterruptValid, firstInterruptAlwaysTrue)
{
    debouncer d(1000);

    unsigned long currentTime = 0;

    EXPECT_TRUE(d.isInterruptValid(currentTime));
}

TEST(isInterruptValid, defaultConstructorAlwaysTrue)
{
    debouncer d;

    unsigned long currentTime = 0;

    EXPECT_TRUE(d.isInterruptValid(currentTime));
    EXPECT_TRUE(d.isInterruptValid(currentTime));
    EXPECT_TRUE(d.isInterruptValid(currentTime));
}