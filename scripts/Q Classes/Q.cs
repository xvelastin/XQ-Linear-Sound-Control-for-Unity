using UnityEngine;

/// <summary>
/// The base of the XQ system. Each Q holds data corresponding to an action that can be done on a clip - playing, fading, stopping, pausing. When triggered, the information is passed to the QManager singleton for timing.
/// </summary>

[System.Serializable]
public class Q
{
    // base properties
    public string name;
    public GameObject target;
    public XQAudioSourceController targetASC;
    public CueType cueType;
    [Min(0.0f)] public float prewaitTime;

    // play q props
    [SerializeReference] protected bool _loop;
    [SerializeReference] [Range(-70, 24)] protected float _outputVolume = 0.0f;
    [SerializeReference] [Range(-70, 24)] protected float _startingVolume = 0.0f;
    [SerializeReference] [Range(-3, 3)] protected float _playSpeed = 1.0f;

    // fade q props
    [SerializeReference] [Range(-70, 0)] protected float _targetVol = 0.0f;
    [SerializeReference] [Range(0, 30)] protected float _fadeTime = 3.0f;
    [SerializeReference] [Range(0, 1)] protected float _curveShape = 0.5f;

    // stop q props
    [SerializeReference] protected bool _pause;

    // type constructors
    public Q(string name, GameObject target, bool loop, float outputVolume, float startingVolume, float playSpeed)
    {
        // Play Q
        this.cueType = CueType.Play;
        this.name = name;
        this.target = target;

        _loop = loop;
        _outputVolume = outputVolume;
        _startingVolume = startingVolume;
        _playSpeed = playSpeed;
    }

    public Q(string name, GameObject target, float targetVol, float fadeTime, float curveShape)
    {
        // Fade Q
        this.cueType = CueType.Fade;
        this.name = name;
        this.target = target;

        _targetVol = targetVol;
        _fadeTime = fadeTime;
        _curveShape = curveShape;
    }

    public Q(string name, GameObject target, bool pause)
    {
        // Stop Q
        this.cueType = CueType.Stop;
        this.name = name;
        this.target = target;

        _pause = pause;
    }

    /// <summary>
    /// Create a new Play Q.
    /// </summary>
    /// <param name="name">The display name of the Q.</param>
    /// <param name="target">The target - must be a Game Object in the hierarchy with an attached audio source component.</param>
    /// <param name="playSpeed">Playback speed, affecting pitch, when triggered.</param>
    /// <returns></returns>
    public static Q Play(string name, GameObject target, bool loop, float outputVolume, float startingVolume, float playSpeed)
    {
        return new Q(name, target, loop, outputVolume, startingVolume, playSpeed);
    }

    /// <summary>
    /// Create a new Fade Q.
    /// </summary>
    /// <param name="name">The display name of the Q.</param>
    /// <param name="target">The target - must be a Game Object in the hierarchy with an attached audio source component.</param>
    /// <param name="targetVol">The volume, in decibels (between -70 and 24), at the end of the fade.</param>
    /// <param name="fadeTime">The time, in seconds, that the fade will take.</param>
    /// <param name="curveShape">Defines the rate of change over the duration of the fade. Create a Fade Q in a QList to visualise the curve.</param>
    /// <returns></returns>
    public static Q Fade(string name, GameObject target, float targetVol, float fadeTime, float curveShape)
    {
        return new Q(name, target, targetVol, fadeTime, curveShape);
    }

    /// <summary>
    /// Create a new Stop Q.
    /// </summary>
    /// <param name="name">The display name of the Q.</param>
    /// <param name="target">The target - must be a Game Object in the hierarchy with an attached audio source 
    /// <param name="pause">If true, triggers Pause() rather than Stop() so that on the next Play() call, meaning playback resumes from the same position.</param>
    /// <returns></returns>
    public static Q Stop(string name, GameObject target, bool pause)
    {
        return new Q(name, target, pause);
    }

    public void Trigger()
    {
        if (!CheckNulls())
        {
            return;
        }

        // Sends the info from this Q to a "Q Manager" Singleton        
        if (prewaitTime == 0.0f)
        {
            XQManager.Instance.PlayQ(this, targetASC, cueType, _playSpeed, _targetVol, _fadeTime, _curveShape, _pause);
        }
        else
        {
            XQManager.Instance.StartTimer(this, prewaitTime, targetASC, cueType, _playSpeed, _targetVol, _fadeTime, _curveShape, _pause);
        }
    }

    // updating references and idiot proofing. returns false if there's a problem.
    private bool CheckNulls()
    {
        // checks for an instance of the manager singleton.
        if (!XQManager.Instance)
        {
            Debug.LogError("XQ: You must have a XQManager in the scene for XQ to function.");
            return false;
        }

        // check if the game object has an ASC
        if (target.GetComponent<XQAudioSourceController>())
        {
            // if so, use the ASC attached to the game object
            targetASC = target.GetComponent<XQAudioSourceController>();
            return true;
        }
        // if not, create one.
        else
        {
            // check if the game object has an audio source
            if (target.GetComponent<AudioSource>())
            {
                AudioSource targetSource = target.GetComponent<AudioSource>();
                targetASC = target.AddComponent<XQAudioSourceController>();

                // if the attached audio source has a clip, assign it to the ASC.
                if (targetSource.clip)
                {
                    targetASC.clip = targetSource.clip;
                    return true;
                }
                // if it doesn't, we have a problem.
                else
                {
                    Debug.LogError("XQ: Attempted to trigger something on " + target.name + ", but it couldn't find a clip to play. Ensure there's a clip attached to the Game Object.");
                    return false;
                }

            }
            // no point in adding an audio source since there still wouldn't be a clip.
            else
            {
                Debug.LogError("XQ: Attmpted to trigger something on " + target.name + ", but it couldn't find an Audio Source component. Is the target in the Q List correct?");
                return false;
            }
        }
    }

    public void UpdateTargetASCParameters(CueType cueType)
    {
        // Sends new parameters to the target XQASC object depending on cue type.
        // Currently only CueType.Play needs updated properties but I'm keeping the structure here in case it changes in future.
        switch (cueType)
        {
            case CueType.Play:
                targetASC.loop = _loop;
                targetASC.outputVolume = _outputVolume;
                targetASC.startingVolume = _startingVolume;
                break;

            case CueType.Fade:
                break;

            case CueType.Stop:
                break;
        }
    }
}