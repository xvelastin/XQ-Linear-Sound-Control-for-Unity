using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// All functions that get sent to XQASCs are routed through this singleton, which must be in the hierarchy.
/// </summary>
[ExecuteInEditMode]
public class XQManager : MonoBehaviour
{
    #region initialisation
    private static XQManager _instance;
    public static XQManager Instance { get { return _instance; } }

    private void Reset()
    {
        init();
    }
    private void Awake()
    {
        init();
    }

    void init()
    {
        if (_instance != null && _instance != this)
        {
            DestroyImmediate(this);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    #region Timer
    public void StartTimer(Q targetQ, float waitTime, XQAudioSourceController ASC, CueType cueType, float playSpeed, float targetVol, float fadeTime, float curveShape, bool pause)
    {
        StartCoroutine(PreWait(targetQ, waitTime, ASC, cueType, playSpeed, targetVol, fadeTime, curveShape, pause));
    }

    public IEnumerator PreWait(Q targetQ, float waitTime, XQAudioSourceController ASC, CueType cueType, float playSpeed, float targetVol, float fadeTime, float curveShape, bool pause)
    {
        Debug.Log(name + ": Waiting for " + waitTime + " seconds, then triggering " + cueType + " on " + ASC.gameObject.name);
        yield return new WaitForSeconds(waitTime);

        PlayQ(targetQ, ASC, cueType, playSpeed, targetVol, fadeTime, curveShape, pause);
        yield break;
    }
    #endregion

    public void PlayQ(Q targetQ, XQAudioSourceController ASC, CueType cueType, float playSpeed, float targetVol, float fadeTime, float curveShape, bool pause)
    {
        targetQ.UpdateTargetASCParameters(cueType);
        switch (cueType)
        {
            case CueType.Play:
                ASC.Play(playSpeed);
                break;

            case CueType.Fade:
                ASC.FadeTo(targetVol, fadeTime, curveShape);
                break;

            case CueType.Stop:
                ASC.Stop(pause);
                break;
        }

    }
}


