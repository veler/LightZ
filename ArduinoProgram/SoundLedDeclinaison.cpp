#include "SoundLedDeclinaison.h"



SoundLedDeclinaison::SoundLedDeclinaison()
{
}

SoundLedDeclinaison::SoundLedDeclinaison(byte rank, CRGB color)
{
	this->rank = rank;
	this->color = color;
}


SoundLedDeclinaison::~SoundLedDeclinaison()
{
	this->rank = 0;
	this->color = NULL;
}
