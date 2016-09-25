#pragma once

#include <Arduino.h>
#include "FastLED.h"
#include "SoundLedDeclinaison.h"

class SoundLedColor
{
public:
    SoundLedColor();
    SoundLedColor(short ledIndex, byte rank, CRGB color);
    ~SoundLedColor();

    short ledIndex;
    byte rank;
    CRGB color;
	SoundLedDeclinaison declinaisons[9];
};