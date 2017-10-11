# LightZ

*LightZ* is a homemade connected LED strip based on Arduino. Inspired by [Philips Ambilight](http://www.philips.com/c-cs/tv/tv-ambilight.html), it can render the light from a computer's screen and sound. This project is a prototype and is not designed to target the general users, however, the source code are available here.

The main features of LightZ are :
* Connection to the LED strip via USB or Bluetooth.
* Possibility to define a manual color.
* Render the sound spectrum.
* Render the monitor borders colors (dynamically depending of the number of LEDs and the LED strip position behind the monitor).

Special thank to [Sébastien WARIN](http://sebastien.warin.fr/) who inspired me to make this project.

[More info here](http://lightz.velersoftware.com)

![clipboardzanager](http://medias.velersoftware.com/images/lightz/background.jpg)

![clipboardzanager](http://medias.velersoftware.com/images/lightz/1.png)

# Setup a development environment

You will need the following tools :
* Windows 7, 8, 8.1 or 10
* .Net 4.6
* Visual Studio 2017 with ``Windows development`` and ``C++ development``
* [Arduino IDE](https://www.arduino.cc/en/Main/Software)
* [Arduino IDE for Visual Studio](https://marketplace.visualstudio.com/items?itemName=VisualMicro.ArduinoIDEforVisualStudio)
* [FastLED](https://github.com/FastLED/FastLED). Clone this repository to ``Documents\Arduino\libraries`` and, in Visual Studio, configure the Arduino extension to use this library.

# Third part libraries

* [Bass.Net](http://bass.radio42.com/)
* [FastLED](https://github.com/FastLED/FastLED)

# License

[![WTFPL](http://www.wtfpl.net/wp-content/uploads/2012/12/wtfpl-badge-1.png)](http://www.wtfpl.net/)

```
            DO WHAT THE FUCK YOU WANT TO PUBLIC LICENSE
                    Version 2, December 2004

 Copyright (C) 2004 Sam Hocevar <sam@hocevar.net>

 Everyone is permitted to copy and distribute verbatim or modified
 copies of this license document, and changing it is allowed as long
 as the name is changed.

            DO WHAT THE FUCK YOU WANT TO PUBLIC LICENSE
   TERMS AND CONDITIONS FOR COPYING, DISTRIBUTION AND MODIFICATION

  0. You just DO WHAT THE FUCK YOU WANT TO.
```