using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Cue types are the base of identification of Qs.
/// </summary>
public enum CueType
{
    Play,
    Fade,
    Stop
}

/// <summary>
/// Collection of global static stuff used by the XQ Sound Control System.
/// </summary>
public static class XQ
{
    public static class AudioUtility
    {
        public static float minimum = -70f;

        public static float ConvertAtoDb(float amp)
        {
            amp = Mathf.Clamp(amp, ConvertDbtoA(minimum), 1f);
            return 20 * Mathf.Log(amp) / Mathf.Log(10);
        }

        public static float ConvertDbtoA(float db)
        {
            return Mathf.Pow(10, db / 20);
        }

        public static AnimationCurve DrawFadeCurve(float curveShape)
        {
            Keyframe[] keys = new Keyframe[2];
            keys[0] = new Keyframe(0, 0, 0, Mathf.Sin(curveShape), 0, 1.0f - curveShape);
            keys[1] = new Keyframe(1, 1, 1 - curveShape, 0, curveShape, 0);
            return new AnimationCurve(keys);
        }
    }

    public static class GUIUtility
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
}