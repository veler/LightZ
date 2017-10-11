#include "FastLED.h" // Available here : https://github.com/FastLED/FastLED
#include "SoundLedColor.h"
#include "SoundLedDeclinaison.h"

#define NUM_LEDS 20
#define DATA_PIN 3
#define CLOCK_PIN 2

#define DATA_MAX_SIZE 64
#define MODE_OR_BRIGHTNESS_DATA_SIZE 4
#define LED_DATA_SIZE 4
#define AUDIO_DATA_SIZE 4

#define AUDIO_RANK_BETWEEN_LEDS 64
#define AUDIO_PEAK_DOWN_FREQUENCY 2
#define AUDIO_PEAK_STAY_TOP_FREQUENCY 4

enum Target
{
    MODE = 0,
    BRIGHTNESS = 1,
    ALLLEDS = 2,
    AUDIO = 3
};

enum Mode
{
    OFF = 0,
    MANUAL = 1,
    SCREEN = 2,
    SOUND = 3,
    UNKNOW = 4
};



CRGB leds[NUM_LEDS];
byte mode = UNKNOW;

uint8_t soundLedPeakRight = 0;
uint8_t soundPeakStayTopFrequencyRight = 0;
uint8_t soundPeakDownFrequencyRight = 0;
uint8_t soundLedPeakLeft = 13;
uint8_t soundPeakStayTopFrequencyLeft = 0;
uint8_t soundPeakDownFrequencyLeft = 0;
SoundLedColor soundLeds[NUM_LEDS];




void setup()
{
    initializeSoundLeds();

    FastLED.addLeds<WS2801, DATA_PIN, CLOCK_PIN, GRB>(leds, NUM_LEDS);
    FastLED.setCorrection(Typical8mmPixel);
    FastLED.setTemperature(Typical8mmPixel);
    FastLED.setDither(0);

    Serial.begin(115200);

    showBrightness(255);
    turnOffIfPossible();
}

void loop()
{
    byte receivedData[DATA_MAX_SIZE];
    int availableBytes = Serial.available();
    int i = 0;

    if (availableBytes > 0 && availableBytes % 4 == 0)
    {
        while (i < availableBytes && i < DATA_MAX_SIZE)
        {
            receivedData[i] = Serial.read();
            i++;
        }
        if (i > 0)
        {
            receivedData[i] = -1;
            proceedReceivedData(receivedData, i);
            Serial.flush();
        }
    }

    turnOffIfPossible();
}






void proceedReceivedData(const byte data[DATA_MAX_SIZE], const int &number_of_bytes_received)
{
    int i = 0;

    while (i < number_of_bytes_received)
    {
        switch (data[i])
        {
        case MODE:
            mode = data[i + 1];
            i += MODE_OR_BRIGHTNESS_DATA_SIZE;
            break;

        case BRIGHTNESS:
            showBrightness(data[i + 1]);
            i += MODE_OR_BRIGHTNESS_DATA_SIZE;
            break;

        case ALLLEDS:
            showColor(-1, CRGB(data[i + 1], data[i + 2], data[i + 3]), true);
            i += LED_DATA_SIZE;
            break;

        case AUDIO:
            showSound(data[i + 1], data[i + 2]);
            i += AUDIO_DATA_SIZE;
            break;

        default:
            if (data[i] >= 4 && data[i] <= 23)
            {
                showColor(data[i] - 4, CRGB(data[i + 1], data[i + 2], data[i + 3]), true);
                i += LED_DATA_SIZE;
            }
            break;
        }
    }
}





void showSound(const byte &left, const byte &right)
{
    FastLED.setBrightness(255);
    showColor(-1, CRGB::Black, false);

    for (size_t i = 0; i < 7; i++)
        if (right > soundLeds[i].rank)
            leds[soundLeds[i].ledIndex] = soundLeds[i].color;
    for (size_t i = 17; i < NUM_LEDS; i++)
        if (right > soundLeds[i].rank)
            leds[soundLeds[i].ledIndex] = soundLeds[i].color;

    for (size_t i = 7; i < 17; i++)
        if (left > soundLeds[i].rank)
            leds[soundLeds[i].ledIndex] = soundLeds[i].color;

    // peak right
    uint8_t peakRight = (uint8_t)(right / AUDIO_RANK_BETWEEN_LEDS);
    if (peakRight >= soundLedPeakRight)
    {
        soundPeakStayTopFrequencyRight = 0;
        soundPeakDownFrequencyRight = 0;
        soundLedPeakRight = peakRight;
        if (right > 220)
            soundLedPeakRight = 3;
    }
    else if (soundPeakStayTopFrequencyRight <= AUDIO_PEAK_STAY_TOP_FREQUENCY)
        soundPeakStayTopFrequencyRight++;
    else if (soundPeakDownFrequencyRight <= AUDIO_PEAK_DOWN_FREQUENCY)
        soundPeakDownFrequencyRight++;
    else if (soundLedPeakRight > 0)
    {
        soundPeakDownFrequencyRight = 0;
        soundLedPeakRight--;
    }

    if (soundLedPeakRight >= 1)
    {
        leds[soundLedPeakRight] = soundLeds[soundLedPeakRight].color;
        if (soundLedPeakRight == 3)
            for (size_t i = 4; i < 7; i++)
                leds[i] = soundLeds[soundLedPeakRight].color;
    }

    // peak left
    uint8_t peakLeft = 13 - (uint8_t)(left / AUDIO_RANK_BETWEEN_LEDS);
    if (peakLeft <= soundLedPeakLeft)
    {
        soundPeakStayTopFrequencyLeft = 0;
        soundPeakDownFrequencyLeft = 0;
        soundLedPeakLeft = peakLeft;
        if (left > 220)
            soundLedPeakLeft = 10;
    }
    else if (soundPeakStayTopFrequencyLeft <= AUDIO_PEAK_STAY_TOP_FREQUENCY)
        soundPeakStayTopFrequencyLeft++;
    else if (soundPeakDownFrequencyLeft <= AUDIO_PEAK_DOWN_FREQUENCY)
        soundPeakDownFrequencyLeft++;
    else if (soundLedPeakLeft < 13)
    {
        soundPeakDownFrequencyLeft = 0;
        soundLedPeakLeft++;
    }

    if (soundLedPeakLeft <= 12)
    {
        leds[soundLedPeakLeft] = soundLeds[soundLedPeakLeft].color;
        if (soundLedPeakLeft == 10)
            for (size_t i = 7; i < 10; i++)
                leds[i] = soundLeds[soundLedPeakLeft].color;
    }

    FastLED.show();
}

void showColor(const short &ledIndex, const CRGB &color, const bool &updateLedStrip)
{
    if (ledIndex < -1 || ledIndex > 19)
        return;

    if (ledIndex == -1)
        for (size_t i = 0; i < NUM_LEDS; i++)
            leds[i] = color;
    else
        leds[ledIndex] = color;

    if (updateLedStrip)
        FastLED.show();
}

void showBrightness(const byte &brightness)
{
    FastLED.setBrightness(brightness);
    FastLED.show();
}

void turnOffIfPossible()
{
    if (mode == OFF || mode == UNKNOW)
        showColor(-1, CRGB::Black, true);
}

void initializeSoundLeds()
{
    for (size_t i = 0; i < NUM_LEDS; i++)
    {
        if (i == 0 || (i >= 13 && i <= NUM_LEDS))
            soundLeds[i] = SoundLedColor(i, 0, CRGB(0, 0, 255));
        else if (i == 1 || i == 12)
            soundLeds[i] = SoundLedColor(i, 75, CRGB(0, 255, 0));
        else if (i == 2 || i == 11)
            soundLeds[i] = SoundLedColor(i, 150, CRGB(255, 150, 0));
        else if (i >= 3 && i <= 10)
            soundLeds[i] = SoundLedColor(i, 220, CRGB(255, 0, 0));
    }
}