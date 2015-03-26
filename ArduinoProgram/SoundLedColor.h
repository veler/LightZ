#pragma once

#include <Arduino.h>
#include "FastLED.h"

class SoundLedColor
{
public:
    SoundLedColor();
    SoundLedColor(short ledIndex, byte rank, CRGB color);
    ~SoundLedColor();

    short ledIndex;
    byte rank;
    CRGB color;
};