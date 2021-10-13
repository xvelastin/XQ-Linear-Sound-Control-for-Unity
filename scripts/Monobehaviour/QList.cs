using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A QList contains a list of XQs - grouped actions (Qs) - which can be triggered by an external script, such as Q Trigger. 
/// Multiple QLists are possible.
/// </summary>
public class QList : MonoBehaviour
{
    public List<StackedQ> qlist = new List<StackedQ>();
    public string Name;

    // change to false to allow Audio Source's "Play On Awake" function.
    private bool overridePlayOnAwake = true;

    private void Start()
    {
        // Stops any pesky Play On Awakes in the q list.
        if (overridePlayOnAwake)
        {
            for (int x = 0; x < qlist.Count; x++)
            {
                List<Q> qs = qlist[x].qs;
                for (int y = 0; y < qs.Count; y++)
                {
                    Q selQ = qs[y];
                    AudioSource source = selQ.target.GetComponent<AudioSource>(); // If this is throwing a NullReferenceException, check the Target fields in the Q list.
                    source.playOnAwake = false;
                    if (source.isPlaying) source.Stop();
                }
            }
        }
    }


    public void PlayXQ(int index)
    {
        qlist[index].Trigger();
    }
}