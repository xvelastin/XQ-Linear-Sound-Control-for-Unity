// xq version 0.5 :: alpha
// last edit: 3 October 2021

using UnityEngine;

/// <summary>
/// An example of linear playback that can be used to go through a neighbouring Q List.
/// </summary>
[System.Serializable]
public class QTrigger : MonoBehaviour
{
    public QList targetQList;
    public int current;
    public int next;
    public int total;

    public void NextQ()
    {
        if (!CheckNulls())
        {
            Debug.Log(this + ": NextQ() called, but no Qs found in Q List");
            return;
        }

        var selCue = targetQList.qlist[next];
        selCue.Trigger();
        Debug.Log(this + ":: Triggered Q " + next + ": " + selCue.name);

        current = this.next;
        if (this.next + 1 < targetQList.qlist.Count)
        {
            this.next++;
        }
        else
        {
            this.next = 0;
        }
    }

    private bool CheckNulls()
    {
        if (!targetQList) targetQList = GetComponent<QList>();

        if (total > 0) return true;
        else return false;
    }

    public void UpdateCount()
    {
        if (!targetQList) return;
        total = targetQList.qlist.Count;
    }


}
