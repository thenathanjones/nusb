What is it?
===========
NUSB is a C# wrapper around the Windows API that can be used for communicating with USB devices

Why do this?
============
The need arose to interact with a USB build light, and just as I was a couple of years ago, there isn't a neat way to do it on Windows.  

Surely this exists?
===================
The existing libraries I've found make you deal with the Windows API calls at too low a level, or make you go down the path of libusb which I'd rather not do.  Instead of trying to make Windows more UNIX-y, this library is a way to use Windows itself to interact at a low level with USB devices.

How do I use this?
==================
Pre-requisites
--------------
I've built Luces as a .NET 4 project, so the runtime will need to be installed.
Usage
-----
I'll put some examples up as things progress, for now you'll just have to work it out yourself.  For a working example, see [Luces](https://github.com/thenathanjones/luces) which uses NUSB to control USB build lights.

Limitations
===========
At this point in time, some of the finer details around re-connections etc. aren't built into the library.  It also doesn't do everything some of the other libraries like libusbsharp do, but should be sufficient for many use cases.