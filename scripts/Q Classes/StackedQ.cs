using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Called XQ generally but StackedQ in scripting, these classes hold lists of Qs and functions that create and trigger them. They are only intended to be nested within QLists.
/// </summary>
[System.Serializable]
public class StackedQ
{
    public List<Q> qs = new List<Q>();
    public string name;

    // For Property Drawer
    public int qIndex;
    public float propertyHeight;

    public void Trigger()
    {
        for (int i = 0; i < qs.Count; ++i)
        {
            qs[i].Trigger();
        }
    }

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


    public void Clear()
    {
        for (int i = 0; i < qs.Count; ++i)
        {
            qs = new List<Q>();
            Debug.Log(this + ": cue cleared.");
        }
    }


}

