#include "SoundLedColor.h"


SoundLedColor::SoundLedColor()
{
}


SoundLedColor::SoundLedColor(short ledIndex, byte rank, CRGB color)
{
    this->ledIndex = ledIndex;
    this->rank = rank;
    this->color = color;
}


SoundLedColor::~SoundLedColor()
{
    this->ledIndex = 0;
    this->rank = 0;
    this->color = NULL;
}