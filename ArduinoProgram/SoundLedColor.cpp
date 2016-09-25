#include "SoundLedColor.h"
#include "SoundLedDeclinaison.h"


SoundLedColor::SoundLedColor()
{
}


SoundLedColor::SoundLedColor(short ledIndex, byte rank, CRGB color)
{
	this->ledIndex = ledIndex;
	this->rank = rank;
	this->color = color;

	int declinaisonCount = 9;
	int rankLimit = 70;
	int rankDecrease = rankLimit / declinaisonCount;
	int lightDecrease = 255 / declinaisonCount;
	for (size_t i = 0; i < declinaisonCount; i++)
	{
		this->declinaisons[i] = SoundLedDeclinaison(rank - (rankDecrease * (i + 1)), color.fadeLightBy(lightDecrease * (i + 1)));
	}
}


SoundLedColor::~SoundLedColor()
{
	this->ledIndex = 0;
	this->rank = 0;
	this->color = NULL;
}