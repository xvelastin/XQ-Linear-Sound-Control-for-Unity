# XQ Linear Sound Control
An early-stage plugin for Unity for controlling sound cues for linear playback. Download the latest [unity package](/unitypackage/) or grab just the scripts. To install a Unity package, download it to some place, then from an existing Unity project, go to Assets/Import Package/Custom Package and navigate to the XQShowControl.unitypackage file. This will bring the folder structure into your project. I'm aware I haven't followed conventions for Unity Packages - this is just while it's in development and will be rectified on release.

XQ-LSC is based around linear playback by groups of single actions (**Qs**). Each Q either plays, fades or stops a single audio source, which must exist on a gameobject in the scene hierarchy. Qs are nested into **XQs**, which function as groups to trigger all its child Qs simultaneously. XQs and their Qs are created, ordered and edited in a **QList** object which provides a simple user interface within the Inspector. You can use as many QLists as you want, meaning it can get quite complex. The XQ system is independent of AudioMixerGroup Output and 3D Sound settings on an AudioSource so it can fit into a 3D setting, as well as playback to the regular master audio context in 2D, without upsetting any custom routing.

## Main Advantages
This plugin was created to assist those who use linear audio in Unity and who aren't already confident with audio programming or C#. You can use XQ without touching a single line of code as a linear playback script, QTrigger, is included within the package. Each XQ group in a QList can be directly called from Unity Events (such as a button) with the call PlayXQ(index) on a QList or NextXQ(index) on a QTrigger. Another advantage is that it is compatible with Unity's frustrating WebGL target limitations, which is useful if you need better volume and playback handling than native Unity (which sucks) but can't use middleware.

## Docs
Below is a brief overview of the components of the XQ system, you can also have a look at the code as I've commented it up. Additionally, there is a "demo" folder if you open with the unity package, which contains a small demo scene with a couple of implementation examples.

![View the function routing diagram](XQ-LSC%20Routing.png)

All files are needed for XQ-LSC to function properly. XQ-LSC's class structure is found in **XQBaseClasses**. All interactable classes have custom property drawers and editors which must be in a folder named "Editor". The **XQManager** and at least one **QList** must be in the scene hierarchy. I've also included **QTrigger**, an optional implementation of linear playback controlling a single QList object at runtime, either with the button in the Inspector or by calling NextXQ.

### XQManager
Handles routing of all parameters and actions in the XQ-LSC system. Must be present in the scene hierarchy, ideally in its own root folder to make sure it doesn't get deleted.

### QList
Provides an interface to create and edit actions that affect target audio objects. Starts with an empty XQ. You can add more with the buttons on the bottom, and reorder them by dragging them. The name of the QList is editable.

### XQ
Contains groups of Qs which will all be fired simultaneously. Add Qs to an XQ group using the buttons at the top. Remove by right clicking or with the - button at the bottom of the XQ. Each XQ's name is editable, and will show if collapsed, so give it a name that covers the actions of all its containing Qs (eg. "Start and Fade Up Ambience").

### Q
Holds information to be passed to the target audio objects (GameObjects with AudioSource components attached). Each Q has different parameters depending on its type. "Play" starts the audio and sets a playback rate. "Fade" interpolates between two volume levels by a given fade curve. "Stop" pauses or completely stops playback. Different Q types are colour-coded for neatness. Each Q only addresses one target and only does one thing, so most situations will require several Qs per trigger. To help with timing, each Q has a "Pre-Wait Time" parameter that delays its action by a given number of seconds. Each Q's name is editable, and will show if collapsed; if left unfilled, it will default to its action and target name (eg. "Start Background Music").

## Pipeline / Get Started
* For each separate audio clip (more correctly, voice) you want, create a Game Object in the hierarchy, attach an Audio Source and assign a clip.
* Create a new GameObject called "XQ", and add a "XQManager" object.
* As a child, create a GameObject and add a "QList" object.
* This QList will by default contain a single empty XQ, which groups Q actions. Create Q actions with the buttons on the top of each XQ group. Each XQ will fire all contained Qs simultaneously when triggered.
* Modify Qs by changing their type, target and properties. Hover over for tooltips or look in the code for details.
* You can re-order, delete and duplicate XQs and Qs, although you can't currently move Qs between XQ groups (although you can right click, copy, and paste values to a new one).
* As an easy way to play, add a "QTrigger" object and assign the QList to its "Target QList" property field. When running the application, press "Next XQ" in the Inspector and it will cycle through the XQs in that QList.
* If making your own implementation, all you need to do is to call PlayXQ(index) on the QList.

The demo scene in the package contains two QLists populated and ready to be triggered by QTrigger scripts.

## Contributing
As this is in more or less active development, I'm all ears for feedback - let me know what works and didn't work, what you used it for, whether you had to change any of it etc. Feel free to fork and suggest changes. As I mention in my [license](LICENSE.md), feel free to use and distribute but always with credit and a link back to this repo.

## Plans for next version
* GUI: Highlight currently playing XQ in QList
* Preview Q/XQ in Editor?
* Move whole thing to Scriptable Objects, better data structure and ability to save edits at runtime.
