=======================================
= README.txt for Counter the Missiles =
=======================================

Counter the Missiles is made by Brandon Gui (admin no.: p1828865) from Singapore Polytechnic (class DIT/FT/2A/05).
This game is inspired by the original Missile Command made by Atari Inc.

===========
= Credits =
===========

All of the music and sound effects (except for a minority like "Low Ammo" and "Unusable Launcher, which was made by me)
are done by Eric Matyas (www.soundimage.org).

=============
= Changelog =
=============

There are many new features added since the v1.0. Yes, it is quite a bit so get ready...
*inhales*

- Name Change -
* Changed name from "Missile Command" to "Counter the Missiles" to prevent possible copyright.
* Added a small splash text on the title screen to show that this is a derivative of "Missile Command".

- New Sprites -
* Changed the look of the various sprites in the game, such as the explosion, cities and missile launchers.
  These sprites are now standardized to 128 pixels per unit.
* Those images in the "Help" menu are replaced with newer ones, as the help menu gets more detailed.

- New Soundtracks -
* Some of the in-game music has been changed with new ones so that it gives a bit more of a exciting(?) feel. (SPOILER: It is the title screen and gameplay music!)

- New Sound Effects -
* Additional sound effects for when the launcher becomes low on ammo, or if you have no launcher selected. These sound effects are made by me.

- New Combo System -
* Missiles that come into contact with your explosion will explode and award points. If those explosions were to hit other
  missiles, they will explode and award double the points, and if those explosions hit other missiles... you get the point.

- The Switch to TextMeshPro -
* Switched to TextMeshPro so that text can render properly and clearly.

- Scoring -
* Destroying missiles now show you how many points it will award. And as you destroy more missiles from your one-missile explosion,
  those score popups will add up.
* E.g. say you destroyed a missile which awards 50 points. This will appear in the popup. But, if you destroy another
  missile with the explosion of your destroyed missile, it will give you 100 points and the popup will now show 150.

- New Animations -
* Score pop has animations! Each time you gain a score, it does its popup animation. After a while, when no additional score
  is gained, it will shrink. Also, all animations can interrupt each other, so it will correctly show the gained score when
  you get more points when it is shrinking.

- Changed Mechanics -
* As soon as you lose your last city, the game will forbid you to fire any more missiles and the game will wait for the
  last player-caused explosion to end, before declaring it game over.