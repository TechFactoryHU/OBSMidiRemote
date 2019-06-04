<img src="OBSMidiRemote/icons/icon.png" alt="[logo]" width="48"/> OBSMidiRemote - windows
=======================

With this app you can turn any MIDI controller to an Open Broadcaster Software (OBS) remote. 
Serial devices also supported, so, you can build your own controller using microcontroller (Arduino/ESP8266/etc...)


#### Main features

- MIDI device support
- Translate MIDI commands to OBS (through OBS-Websocket plugin)
- MIDI mappings stored in XML format, XSD available
- LED feedback configurable
- Devices through SERIAL port also supported (Arduino ...)


#### Requirements

Microsoft [.NET Framework 4.5.2] or higher, Microsoft [Visual C++ 2015 Redistributable] (x86)
and [OBS Websocket plugin]


#### Development

- [Visual Studio 2015] & [.NET Framework 4.5.2 Developer Pack] are required.
- NuGets: Newtonsoft.JSON, websocket-sharp


#### Components
```
OBSWebsocketdotnet (MIT)	https://github.com/Palakis/obs-websocket-dotnet
PureMidi (Apache 2.0)		https://archive.codeplex.com/?p=puremidi
Newtonsoft.Json (MIT)		https://github.com/JamesNK/Newtonsoft.Json
websocket-sharp (MIT)		https://github.com/sta/websocket-sharp
```

#### Screenshot

![alt text](http://techfactory.hu/static/content/obsmidiremote_app_scr.jpg)



[.NET Framework 4.5.2]: https://www.microsoft.com/en-US/download/details.aspx?id=53344
[Visual C++ 2015 Redistributable]: https://www.microsoft.com/en-us/download/details.aspx?id=53840