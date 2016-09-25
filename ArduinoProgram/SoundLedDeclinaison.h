#pragma once

#include <Arduino.h>
#include "FastLED.h"

class SoundLedDeclinaison
{
public:
	SoundLedDeclinaison();
	SoundLedDeclinaison(byte rank, CRGB color);
	~SoundLedDeclinaison();

	byte rank;
	CRGB color;
};

