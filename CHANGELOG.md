# Changelog

## 0.5.1 - 2021-10-07
### Added
- Function Routing diagramme to directory, to better visualise how the scripts work together.

### Modified
- Defined Q default values for parameters so that when changing type, they are created with better values eg. curveShape at 0.5 (s-curve) rather than 0 (exponential).
- Tidied comments throughout scripts.
- Updated the UnityPackage with the above changes.

## 0.5.0 Alpha - 2021-10-04
### Added
- Created base structure of classes Q/StackedQ/QList with respective Drawers and Editors to handle interface between Q data and Unity GUI.
- Tested using go-button style component QTrigger: seems stable.
- Created stable unitypackage version that includes base scripts and a demo/example scene.

### Unfixed Issues
- Creating a new Q with the SQ buttons doesn't always trigger a layout change in the QList. Not sure how to call this directly. Workaround: trigger a layout change eg. by expanding one of the dropdown arrows or adding a space to a name.



