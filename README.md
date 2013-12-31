OverlayDropper by ACLPro
==============

Program designed to be used by the stream team to syncronise hiding in game overlays and ATEM mixer overlays. The program should be run on the spectator PC.

![Screenshot](https://raw.github.com/sneat/OverlayDropper/master/OverlayDropper/Resources/Screenshot.png "Screenshot")

Overlays will only toggle via in-game Hotkeys if all of the following are met:
* ATEM is connected
* ATEM Program is the same as Game Source Dropdown
* Hotkey Binding is Enabled
* Game has been selected
* Selected game is the active window (i.e. will not trigger when you have Alt-Tabbed out of the game)

When overlays are dropped, the ATEM Software Control Panel will be left in a "blank state" (i.e. no keyers On Air, and no keyers Tied).
When overlays are returned, the ATEM Software Control Panel will be returned to it's original state, including any keyer Tie states.

OverlayDropper will use the Mix Transition to perform the toggles (even if Mix isn't selected as the Transition method).

How To Use
======

* ATEM Software label lists the version of the ATEM Software that should be running.
* Enter the IP Address of the ATEM and press Connect
* Game Source dropdown will automatically be populated, Select the Source that corresponds to the Game Input (i.e. if you have the Spectator PC plugged into HDMI 1 on the ATEM, select 1 from the dropdown)
* Select the game that is being spectated from the Game Dropdown (it will tell you the hotkeys that it will respond to)
* Select Enabled for the Hotkey Binding (**note:** you can toggle this checkbox at any time by pressing Ctrl+Alt+Shift+O even when you are mid-game)
* That's it!
