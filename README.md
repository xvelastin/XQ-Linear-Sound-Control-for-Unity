# XQ Linear Sound Control
An early-stage plugin for Unity for controlling sound cues for linear playback. Download the latest [unity package](/unitypackage/) or grab just the scripts. To install a Unity package, download it to some place (eg. Unity/Packages), then from an existing Unity project, go to Assets/Import Package/Custom Package and navigate to the XQShowControl.unitypackage file. This will bring the folder structure into your project. I'm aware I haven't followed conventions for Unity Packages - this is just while it's in development and will be rectified on release.

XQ-LSC is based around linear playback by groups of single actions (Qs). Each Q either plays, fades or stops a single audio source. Qs are grouped into XQs, which trigger all its child Qs simultaneously. XQs and their Qs are created, ordered and edited in a QList object which provides a simple user interface within the Inspector. You can use as many QLists as you want, meaning it can get quite complex. The XQ system is independent of AudioMixerGroup Output and 3D Sound settings on an AudioSource so it can fit into a 3D setting, as well as playback to the regular master audio context in 2D, without upsetting any custom routing.

## Main Advantages
This plugin was created to assist those who use linear audio in Unity and who aren't already confident with audio programming or C#. You can use XQ without touching a single line of code as a linear playback script, QTrigger, is included within the package. Each XQ group in a QList can be directly called from Unity Events (such as a button) with the call NextXQ(index). Another advantage is that it is compatible with Unity's frustrating WebGL target limitations, which is useful if you need better volume and playback handling than native Unity (which sucks) but can't use middleware.

## Docs
When I get a chance I'll document this fully. Below is a brief overview of the components of the XQ system, you can also have a look at the code as I've commented it up. Additionally, there is a "demo" folder if you open with the unity package, which contains a small demo scene with a couple of implementation examples.

[View the function routing diagram](XQ-LSC%20Routing.png)

XQ-LSC contains the following objects (most also have essential custom editor/property drawers, so make sure these are in the assets directory):
### Monobehaviour (must be in scene hierarchy)
* XQManager - Routes directions received from QLists to the relevant Audio Objects.
* QList - Contains groups (XQs) of actions (Qs). This is the main element with which the user interacts.
* QTrigger (optional) - An example implementation of linear playback controlling a single QList object at runtime, either with the button in the Inspector or by calling NextXQ.
* XQAudioSourceController (optional) - Instantiated during playback with a QList, this contains the audio functions that a Q calls when triggered.

### Classes (must be in Assets)
* XQStatics - Contains static parameters and functions called by other scripts.
* StackedQ - A list of Qs with custom Property Drawer that allows for reordering and editing.
* Q - the base "action" class that controls the XQ-LSC system and holds the information to be triggered, with a custom Property Drawer that allows for reordering and editing.

## Brief overview of pipeline
* For each separate audio clip (more correctly, voice) you want, create a Game Object in the hierarchy, attach an Audio Source and assign a clip.
* Create a new GameObject called "XQ", and add a "XQManager" object.
* As a child, create a GameObject and add a "QList" object.
* In this QList, you can add a new XQ group which contains Q actions. Create Q actions with the buttons on the top of each XQ group. Each XQ will fire all contained Qs simultaneously when triggered.
* Modify Qs by changing their type, target and properties. Hover over for tooltips or look in the code for details.
* You can re-order, delete and duplicate XQs and Qs as any normal reorderable list.
* As an easy way to play, add a "QTrigger" object and assign the QList to its "Target QList" property field. When running the application, press "Next XQ" in the Inspector and it will cycle through the XQs in that QList.

The demo scene in the package contains two QLists populated and ready to be triggered by their QTrigger scripts.

## Contributing
As this is in more or less active development, I'm all ears for feedback - let me know what works and didn't work, what you used it for, whether you had to change any of it etc. Feel free to fork and suggest changes. As I mention in my [license](LICENSE.md), feel free to use and distribute but always with credit and a link back to this repo.

## Plans for next version
* GUI: Highlight currently playing XQ in QList
* Implement DontDestroyOnLoad for XQManager and as a toggle for QList objects.
