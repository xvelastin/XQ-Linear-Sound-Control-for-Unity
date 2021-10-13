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
public static class XQAudioUtility
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