using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(Q))]
public class QDrawer : PropertyDrawer
{
    public static int basepropertyHeightInLines = 6;

    private int lines;
    private float propertyHeightSum;
    private static float singleLineHeight = EditorGUIUtility.singleLineHeight;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent contlabelent)
    {
        return propertyHeightSum;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // ** Base Q Properties **
        SerializedProperty name = property.FindPropertyRelative("name");
        SerializedProperty cueType = property.FindPropertyRelative("cueType");
        CueType cueTypeAsEnum = (CueType)cueType.enumValueIndex;
        SerializedProperty target = property.FindPropertyRelative("target");
        SerializedProperty targetASC = property.FindPropertyRelative("targetASC");
        SerializedProperty prewaitTime = property.FindPropertyRelative("prewaitTime");

        // ** Cuetype-Specific Properties **
        // play q 
        SerializedProperty loop = property.FindPropertyRelative("_loop");
        SerializedProperty clip = property.FindPropertyRelative("clip");
        SerializedProperty outputVolume = property.FindPropertyRelative("_outputVolume");
        SerializedProperty startingVolume = property.FindPropertyRelative("_startingVolume");
        SerializedProperty playSpeed = property.FindPropertyRelative("_playSpeed");

        // fade q     
        SerializedProperty targetVol = property.FindPropertyRelative("_targetVol");
        SerializedProperty fadeTime = property.FindPropertyRelative("_fadeTime");
        SerializedProperty curveShape = property.FindPropertyRelative("_curveShape");

        // stop q
        SerializedProperty pause = property.FindPropertyRelative("_pause");

        // ** GUI Styles **
        // Colours
        Color defaultColor = GUI.color;
        Color highlightColor = XQGUIUtility.QTypeToColor(cueTypeAsEnum);

        // Highlighted
        GUIStyle highlighted = new GUIStyle();
        highlighted.normal.textColor = highlightColor;
        highlighted.fontStyle = FontStyle.Bold;

        // Foldout
        GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
        XQGUIUtility.ColorAllTextInStyle(foldoutStyle, highlightColor);
        foldoutStyle.fontStyle = FontStyle.Bold;

        // Enum
        GUIStyle enumPopUpHighlighted = new GUIStyle(EditorStyles.popup);
        XQGUIUtility.ColorAllTextInStyle(enumPopUpHighlighted, highlightColor);
        enumPopUpHighlighted.fontStyle = FontStyle.Bold;

        // Editable Text
        GUIStyle editableText = new GUIStyle(EditorStyles.textField);
        XQGUIUtility.ColorAllTextInStyle(editableText, highlightColor);

        // ** GUI Render **
        position = XQGUIUtility.MoveRect.Shift(position, 10, 0, singleLineHeight);

        // create foldout menu
        property.isExpanded = EditorGUI.Foldout(
            position,
            property.isExpanded,
            "Q",
            foldoutStyle);

        // create foldout menu title
        position = XQGUIUtility.MoveRect.ShiftRight(position);
        name.stringValue = EditorGUI.TextField(position, name.stringValue, editableText);
        EditorGUI.LabelField(position, new GUIContent("", "Edit the display name of this action."));
        position = XQGUIUtility.MoveRect.ShiftLeft(position);
        position = XQGUIUtility.MoveRect.LineBreak(position);

        // populate q drawer with variables
        if (property.isExpanded)
        {
            // Checks for change in either Cue Type or Target.
            EditorGUI.BeginChangeCheck();

            // q.cuetype (Enum)
            EditorGUI.LabelField(position, "Type", highlighted);
            position = XQGUIUtility.MoveRect.ShiftRight(position);
            cueTypeAsEnum = (CueType)EditorGUI.EnumPopup(position, cueTypeAsEnum, enumPopUpHighlighted);
            EditorGUI.LabelField(position, new GUIContent("", "Change the action type."));
            cueType.enumValueIndex = (int)cueTypeAsEnum;
            position = XQGUIUtility.MoveRect.ShiftLeft(position);

            // q.target (GameObject)            
            position = XQGUIUtility.MoveRect.LineBreak(position);
            EditorGUI.LabelField(position, "Target Object");
            position = XQGUIUtility.MoveRect.ShiftRight(position);
            EditorGUI.ObjectField(position, target, GUIContent.none);
            EditorGUI.LabelField(position, new GUIContent("", "Drag or select the audio object from the hierarchy that will receive this action."));
            position = XQGUIUtility.MoveRect.ShiftLeft(position);

            // If the user has changed one, rename to fit, as a timesaver.
            if (EditorGUI.EndChangeCheck())
            {
                GameObject obj = target.objectReferenceValue as GameObject;
                name.stringValue = cueTypeAsEnum.ToString() + " " + obj.name;
            }

            position = XQGUIUtility.MoveRect.LineBreak(position);
            position = XQGUIUtility.MoveRect.LineBreak(position);
            EditorGUI.LabelField(position, "Options", EditorStyles.boldLabel);

            // q.prewaitTime (float)
            position = XQGUIUtility.MoveRect.LineBreak(position);
            EditorGUI.LabelField(position, "Pre Wait Time");
            position = XQGUIUtility.MoveRect.ShiftRight(position);
            prewaitTime.floatValue = EditorGUI.Slider(position, prewaitTime.floatValue, 0.0f, 60.0f);
            EditorGUI.LabelField(position, new GUIContent("", "Delays this action by given time (in seconds)."));
            position = XQGUIUtility.MoveRect.ShiftLeft(position);
            position = XQGUIUtility.MoveRect.LineBreak(position);

            // Show further modifiable options based on cue type.
            switch (cueTypeAsEnum)
            {
                case CueType.Play:
                    position = ShowAsPlayCue(position, clip, loop, outputVolume, startingVolume, playSpeed);
                    break;

                case CueType.Fade:
                    position = ShowAsFadeCue(position, targetASC, targetVol, fadeTime, curveShape);
                    break;

                case CueType.Stop:
                    position = ShowAsStopCue(position, pause);
                    break;
                default:
                    lines = basepropertyHeightInLines;
                    break;
            }

            // Draw space depending on Q Type
            lines = XQGUIUtility.QTypeToLines(cueTypeAsEnum) + basepropertyHeightInLines;
            propertyHeightSum = singleLineHeight * lines;
        }
        else
        {
            lines = 1;
            propertyHeightSum = singleLineHeight * lines;
        }
        property.serializedObject.ApplyModifiedProperties();
        EditorGUI.EndProperty();
    }

    private Rect ShowAsPlayCue(Rect position, SerializedProperty clip, SerializedProperty loop, SerializedProperty outputVol, SerializedProperty startingVol, SerializedProperty playSpeed)
    {
        EditorGUI.LabelField(position, "Loop Clip?");
        position = XQGUIUtility.MoveRect.ShiftRight(position);
        loop.boolValue = EditorGUI.Toggle(position, loop.boolValue);
        EditorGUI.LabelField(position, new GUIContent("", "Determines whether the clip will be looped when the play is triggered."));
        position = XQGUIUtility.MoveRect.ShiftLeft(position);

        position = XQGUIUtility.MoveRect.LineBreak(position);
        EditorGUI.LabelField(position, "Output Volume");
        position = XQGUIUtility.MoveRect.ShiftRight(position);
        outputVol.floatValue = EditorGUI.Slider(position, outputVol.floatValue, XQAudioUtility.minimum, 24.0f);
        EditorGUI.LabelField(position, new GUIContent("", "Sets the maximum volume of this audio object (in decibels). Note that the sum of volumes will be clamped to the original sound file's maximum."));
        position = XQGUIUtility.MoveRect.ShiftLeft(position);

        position = XQGUIUtility.MoveRect.LineBreak(position);
        EditorGUI.LabelField(position, "Starting Volume");
        position = XQGUIUtility.MoveRect.ShiftRight(position);
        startingVol.floatValue = EditorGUI.Slider(position, startingVol.floatValue, XQAudioUtility.minimum, 24.0f);
        EditorGUI.LabelField(position, new GUIContent("", "Sets the starting volume of this audio object (in decibels)."));
        position = XQGUIUtility.MoveRect.ShiftLeft(position);

        position = XQGUIUtility.MoveRect.LineBreak(position);
        EditorGUI.LabelField(position, "Playback Speed");
        position = XQGUIUtility.MoveRect.ShiftRight(position);
        playSpeed.floatValue = EditorGUI.Slider(position, playSpeed.floatValue, -3, 3);
        EditorGUI.LabelField(position, new GUIContent("", "Sets the playback speed (affecting pitch) of the clip. A negative value plays the clip in reverse."));
        position = XQGUIUtility.MoveRect.ShiftLeft(position);
        return position;
    }

    private Rect ShowAsFadeCue(Rect position, SerializedProperty targetASC, SerializedProperty targetVol, SerializedProperty fadeTime, SerializedProperty curveShape)
    {
        Color curveCol = XQGUIUtility.QTypeToColor(CueType.Fade);

        EditorGUI.LabelField(position, "Target Volume");
        position = XQGUIUtility.MoveRect.ShiftRight(position);
        targetVol.floatValue = EditorGUI.Slider(position, targetVol.floatValue, XQAudioUtility.minimum, 0);
        EditorGUI.LabelField(position, new GUIContent("", "Sets the volume (in decibels) of the target by the end of the fade."));
        position = XQGUIUtility.MoveRect.ShiftLeft(position);

        position = XQGUIUtility.MoveRect.LineBreak(position);
        EditorGUI.LabelField(position, "Fade Time");
        position = XQGUIUtility.MoveRect.ShiftRight(position);
        fadeTime.floatValue = EditorGUI.Slider(position, fadeTime.floatValue, 0, XQAudioSourceController.maximumFadeTime);
        EditorGUI.LabelField(position, new GUIContent("", "Sets the time (in seconds) for the sound to get from its current volume to the target volume."));
        position = XQGUIUtility.MoveRect.ShiftLeft(position);

        position = XQGUIUtility.MoveRect.LineBreak(position);
        EditorGUI.LabelField(position, "Curve Shape");
        position = XQGUIUtility.MoveRect.ShiftRight(position);
        curveShape.floatValue = EditorGUI.Slider(position, curveShape.floatValue, 0.0f, 1.0f);
        EditorGUI.LabelField(position, new GUIContent("", "Describes the rate that the volume changes over the course of the fade."));

        // Draw an animation curve at full width to visually represent the curve shape.
        int fadeCurveLines = 4;
        position = new Rect(position.x - 100, position.y + singleLineHeight, position.width + 100, singleLineHeight * fadeCurveLines);
        EditorGUI.CurveField(position, XQAudioUtility.DrawFadeCurve(curveShape.floatValue), curveCol, new Rect(0, 0, 1, 1));
        EditorGUI.LabelField(position, new GUIContent("", "(Read Only) A visual representation of the Curve Shape function above."));
        position = XQGUIUtility.MoveRect.Shift(position, 0, fadeCurveLines * singleLineHeight, singleLineHeight);

        return position;
    }

    private Rect ShowAsStopCue(Rect position, SerializedProperty pause)
    {
        EditorGUI.LabelField(position, "Pause Clip?");
        position = XQGUIUtility.MoveRect.ShiftRight(position);
        pause.boolValue = EditorGUI.Toggle(position, pause.boolValue);
        EditorGUI.LabelField(position, new GUIContent("", "If toggled on, a call to Play again will resume from the position it stopped at. If toggled off, the clip will be unloaded from memory and a further Play call will start the clip from the beginning."));
        position = XQGUIUtility.MoveRect.ShiftLeft(position);

        return position;
    }

    // private void DeleteButton()
    // {
    //     //Todo :: Get functionality working - how to get StackedQ ('s qIndex) from each Q to delete individually? Bit fiddly.

    //     Rect DeleteButtonPosition = new Rect(Screen.width - Screen.width / 4, position.y + singleLineHeight, Screen.width / 5, singleLineHeight);
    //     GUIStyle DeleteButtonStyle = new GUIStyle(EditorStyles.toolbarButton);
    //     XQGUIUtility.ColorAllTextInStyle(DeleteButtonStyle, highlightColor);
    //     DeleteButtonStyle.hover.textColor = Color.red;
    //     DeleteButtonStyle.onHover.textColor = Color.red;
    //     DeleteButtonStyle.fontStyle = FontStyle.Bold;
    //     if (GUI.Button(DeleteButtonPosition, "Delete Cue", DeleteButtonStyle))
    //     {

    //     }
    // }
}