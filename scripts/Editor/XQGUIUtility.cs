using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class XQGUIUtility
{
    private static float singleLineHeight = EditorGUIUtility.singleLineHeight;

    //Returns the number of lines that the property drawer of each type of Q takes up in the inspector window.
    public static int QTypeToLines(CueType cueType)
    {
        switch (cueType)
        {
            case CueType.Play:
                return 6;

            case CueType.Fade:
                return 9;

            case CueType.Stop:
                return 3;

            default:
                Debug.LogWarning("Unknown Cue Type read for QTypeToLines. Cuetype is: " + cueType);
                return 0;
        }
    }

    // holds the colours for each cue type.
    public static Color QTypeToColor(CueType qtype)
    {
        switch (qtype)
        {
            case CueType.Play:
                return new Color(0, 0.95f, 0.45f);

            case CueType.Fade:
                return new Color(0, 0.75f, 0.95f);

            case CueType.Stop:
                return new Color(0.98f, 0.75f, 0.34f);

            default:
                Debug.LogWarning("Unknown Cue Type read for QTypeToColour.");
                return Color.white;
        }
    }

    public static void ColorAllTextInStyle(GUIStyle style, Color colour)
    {
        style.normal.textColor = colour;
        style.focused.textColor = colour;
        style.active.textColor = colour;
        style.hover.textColor = colour;

        style.onNormal.textColor = colour;
        style.onFocused.textColor = colour;
        style.onActive.textColor = colour;
        style.onHover.textColor = colour;
    }

    public static class MoveRect
    {
        private static float indent = 100;

        public static Rect Shift(Rect position, float x, float y, float h)
        {
            return new Rect(position.x + x, position.y + y, position.width - x, h);
        }

        public static Rect LineBreak(Rect position)
        {
            return new Rect(position.x, position.y + (singleLineHeight), position.width, singleLineHeight);
        }

        public static Rect ShiftLeft(Rect position)
        {
            return new Rect(position.x - indent, position.y, position.width + indent, singleLineHeight);
        }

        public static Rect ShiftRight(Rect position)
        {
            return new Rect(position.x + indent, position.y, position.width - indent, singleLineHeight);
        }
    }
}
