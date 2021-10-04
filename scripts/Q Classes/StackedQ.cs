using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Called XQ externally but StackedQ in scripting, these classes hold lists of Qs and functions that create and trigger them. They are only intended to be nested within QLists.
/// </summary>
[System.Serializable]
public class StackedQ
{
    public List<Q> qs = new List<Q>();
    public string name;

    // For Property Drawer
    public int qIndex;
    public float propertyHeight;

    /// <summary>
    /// Trigger all the contained Qs' functions according to their type.
    /// </summary>
    public void Trigger()
    {
        for (int i = 0; i < qs.Count; ++i)
        {
            qs[i].Trigger();
        }
    }

    /// <summary>
    /// Add a new Q of the given type to this Stacked Q.
    /// </summary>
    /// <param name="qtype">The type of Q to be added.</param>
    public void NewQ(CueType qtype)
    {
        switch (qtype)
        {
            case CueType.Play:
                qs.Add(new Q("Play", null, false, 0, 0, 1.0f));
                break;

            case CueType.Fade:
                qs.Add(new Q("Fade", null, 0.0f, 3.0f, 0.5f));
                break;

            case CueType.Stop:
                qs.Add(new Q("Stop", null, false));
                break;
        }
    }

    /// <summary>
    /// Delete all Qs from this Stacked Q.
    /// </summary>
    public void Clear()
    {
        for (int i = 0; i < qs.Count; ++i)
        {
            qs = new List<Q>();
            Debug.Log(this + ": cleared.");
        }
    }
}