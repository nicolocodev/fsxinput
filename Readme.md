# FsXinput

This repository contains a F# wrapper around [XInput Api](http://msdn.microsoft.com/en-us/library/windows/desktop/hh405053%28v=vs.85%29.aspx) and a Demo console application

## Overview

I wrote this code just for fun and learning purposes, the FsXinput code is a translation of the code from [this great article](http://blogs.msdn.com/b/pstubbs/archive/2006/02/13/531008.aspx), but of course, a bit more functional.

I added an event-driven behaivor, so, you can get an Controller instance and subscribe to the event.

Also you can use observable (in a FRP way) to query the events stream.

This project uses the 1.4 XInput version, in order to improve it and use it as a PCL in others platforms. You can learn more about XInput versions [here](http://msdn.microsoft.com/en-us/library/windows/desktop/hh405051%28v=vs.85%29.aspx)

## Usage

Connect your XBox Controller, start the Demo application and press some buttons.