# XQ Linear Sound Control
An early-stage plugin for Unity for controlling sound cues for linear playback. Use the unity package or grab just the scripts. To install a Unity package, download it to some place (eg. Unity/Packages), then from an existing Unity project, go to Assets/Import Package/Custom Package and navigate to the XQShowControl.unitypackage file. This will bring the folder structure into your project. I'm aware I haven't followed conventions for Unity Packages - this is just while it's in development and will be rectified on release.

## Documentation
When I get a chance I'll document this fully. For now, you can have a look at the code and I've commented it up. There is also a "demo" folder if you open the unity package, which contains a small demo scene with a couple of implementation examples.

## Brief overview of pipeline
* For each separate audio clip (more correctly, voice) you want, create a Game Object in the hierarchy, attach an Audio Source and assign a clip.
* Create a new GameObject called "XQ", and add a "XQManager" object.
* As a child, create a GameObject and add a "QList" object.
* In this QList, you can add a new XQ group which contains Q actions. Create Q actions with the buttons on the top of each XQ group. Each XQ will fire all contained Qs simultaneously when triggered.
* Modify Qs by changing their type, target and properties. Hover over for tooltips or look in the code for details.
* You can re-order, delete and duplicate XQs and Qs as any normal reorderable list.
* As an easy way to play, add a "QTrigger" object and assign the QList to its "Target QList" property field. When running the application, press "Next XQ" in the Inspector and it will cycle through the XQs in that QList.

The demo scene in the package contains two QLists populated and ready to be triggered by their QTrigger scripts.