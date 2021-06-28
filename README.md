# EvM
This is an interactive screensaver I made with Unity to learn it's dev environment. It looks something like this:

![Alt Text](https://media.giphy.com/media/iOvB2WfLZj1ol2jtCn/giphy.gif)

Installation:
To "install" it as a screensaver on Windows you need to move contents of 'build' folder to Windows/System32 and also change .exe to .scr

After that, open up screensaver settings, choose EvM from drop-down list (preview won't work, and you'll also expirience screensaver launching on you 2 times - close it by pressing Esc)
It works like that because build lacks code to distinguish between previewing and actually loading up full width screensaver. However, it'll work perfectly as an actual screensaver that
boots up after set number of minutes.

Controls:

p - generate a symmetrical pattern;

r - generate random pattern;

c - clear screen;

s - make a step;

a - automate steps;
