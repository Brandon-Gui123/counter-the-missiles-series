=======================================
= README.txt for Counter the Missiles =
=======================================

This is a README text file for "Counter the Missiles", made in Unity3D.

"Counter the Missiles" is made by Brandon Gui (p1828865) from class DIT/FT/2A/05 in Singapore Polytechnic.

===========
= Credits =
===========

Soundtracks used:
"Analog Nostalgia" (looping variant)
"Grunge Bot" (looping variant; fixed and enhanced in GarageBand for iOS so that there are no breaks when it loops)
"Low Point" (looping variant)
"Panoramic Dystopia" (looping variant)
Soundtacks by Eric Matyas (soundimage.org)

Sound effects used:
"Explosion7", "PowerRez5" and "Space-Cannon" by Eric Matyas (soundimage.org)
"Low Ammo" and "Unusable Launcher" by Brandon Gui (that's me; I made this in GarageBand for iOS)

Background images used:
"Starry Sky Astronomy Star Milky Way Sky Night" by Max Pixel.
Available from: https://www.maxpixel.net/Starry-Sky-Astronomy-Star-Milky-Way-Sky-Night-3714858
Licensed under CC0.

Sprites used:
All of the sprites used are mine (made them in Adobe Photoshop), except for "Square", "Circle" and "Octagon"
since those are available out-of-the-box in the Unity editor.

Fonts used:
Inconsolata by Raph Levien (https://fonts.google.com/specimen/Inconsolata).
This is the font I use for readable text in settings, credits, help etc.
(It's also one of my favourite fonts for coding, to be honest!)

Megrim by Daniel Johnson (https://fonts.google.com/specimen/Megrim).
This is the font I use for headings, titles, missile launcher status texts etc.

Thank you to all my playtesters:
Jeremy Lim, You Jing and others who have tried those .apk files or .zip files
I sent every few days or so whenever I make an update.

"Counter the Missiles" is inspired by the original "Missile Command" by Atari Inc.
Went for this game because I want to feel how it's like making it and it looks interesting already.

=============
= Changelog =
=============

A lot has changed since the initial release of "Counter the Missiles (a.k.a. Missile Command by Brandon Gui)".
Buckle up because here we go!

== Overall ==
*** Brand new soundtracks ***
Soundtrack for the title screen and game screen (the one where you shoot missiles) have been changed!
Now they use "Panaromic Dystopia" and "Grunge Bot" respectively.
I changed it to lift the mood up to something less horror-filled.

*** Brand new sprites ***
The sprites for the missile launcher, cities and explosions have been changed.
Also, they now look severly damaged when exploded by an enemy missile, compared to just
disappearing entirely.

*** The Switch to TextMesh Pro ***
I switched to TextMesh Pro because I want to ensure that the text on-screen are
clearly rendered.

== Help Scene ==
*** More sprites for the help page ***
The help page now use new sprites to help convey information much more easily.

== Credits Scene ==
*** Credits page now use page navigation ***
Left for previous page, right for next page.

== Settings Scene (new) ==
*** Settings scene ***
Adjust settings in the settings scene. Right now, there's only the option to enable vibration.
Vibration is disabled by default.

== Hardware Support ==
*** Gyroscope and accelerometer support ***
The gyroscope and accelerometer is available for use, on the title screen.
Right now, you can only alter the position of the flashing stars on-screen.

*** Vibration Support ***
Your device vibrates for around a second when missile launchers or cities get destroyed.
This is off by default.

== Gameplay ==
*** Bigger touch area for missile launchers ***
I've given a generous touch area to select your missile launchers now,
compared to previously where I would use the area of the missile launcher.

*** Sound effects pertaining to missile launchers ***
When your launcher is low on ammo, it'd play a sound.
When you have no launchers selected and you try to fire, sounds will play to inform you on that.

*** Brand new combo system! ***
The more missiles you destroy with just one launched missile, the more points you get!

*** Point pop-ups (the numbers that appear when you destroy a missile) ***
Whenever you destroy missiles, points will pop-up to show how much points you've earned from that
combo. Also, additional points will add up to that pop-up. After a while, the pop-up fades away.
Also, there are animations as well, like the points pop-up expanding and shrinking quickly when you gain
a point, and the points pop-up transitioning from red to orange (with strength based on the number of points
you earned), if you get a high enough number of points.
It's because of the game "Bejeweled 3" that got me into making point pop-ups.

*** Delayed Game Over ***
Previously, you lose the game as soon as your last city is destroyed.
Now, you lose the game when your last city is destroyed AND your last combo has finished.

*** Game Over Statistics Layout ***
Now, the statistics are placed rather far apart.
The headers on the left, and the values on the right.

*** Fast-forward Icon Blinking ***
The fast-forward icon now blinks to show that something is REALLY happening.

*** Selection Indicators ***
Missile launchers now show a frame around them when you select them.
Also, the frame can quickly shrink when the launcher is deselected.


README.txt typed out on 9 July, 2019.