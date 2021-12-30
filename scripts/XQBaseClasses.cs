/* Contains XQ-LSC specific classes */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* BASE CLASSES  */

/// <summary>
/// The base of the XQ system. Each Q holds data corresponding to an action that can be done on a clip - playing, fading, stopping, pausing. When triggered, the information is passed to the QManager singleton for timing.
/// </summary>
[System.Serializable]
public class Q
{
    // base properties
    public string name;
    [SerializeReference] public XQ parentXQ;
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

    // for property drawer
    [SerializeReference] public int qIndex;

    // type constructors
    public Q(string name, XQ xq, GameObject target, bool loop, float outputVolume, float startingVolume, float playSpeed)
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

    public Q(string name, XQ xq,  GameObject target, float targetVol, float fadeTime, float curveShape)
    {
        // Fade Q
        this.cueType = CueType.Fade;
        this.name = name;
        this.target = target;

        _targetVol = targetVol;
        _fadeTime = fadeTime;
        _curveShape = curveShape;
    }

    public Q(string name, XQ xq,  GameObject target, bool pause)
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
    public static Q Play(string name, XQ xq, GameObject target, bool loop, float outputVolume, float startingVolume, float playSpeed)
    {
        return new Q(name, xq, target, loop, outputVolume, startingVolume, playSpeed);
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
    public static Q Fade(string name, XQ xq, GameObject target, float targetVol, float fadeTime, float curveShape)
    {
        return new Q(name, xq, target, targetVol, fadeTime, curveShape);
    }

    /// <summary>
    /// Create a new Stop Q.
    /// </summary>
    /// <param name="name">The display name of the Q.</param>
    /// <param name="target">The target - must be a Game Object in the hierarchy with an attached audio source 
    /// <param name="pause">If true, triggers Pause() rather than Stop() so that on the next Play() call, meaning playback resumes from the same position.</param>
    /// <returns></returns>
    public static Q Stop(string name, XQ xq, GameObject target, bool pause)
    {
        return new Q(name, xq, target, pause);
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

/// <summary>
/// Holds lists of Qs and functions that create and trigger them. They are only intended to be nested within QLists.
/// </summary>
[System.Serializable]
public class XQ
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
    /// Add a new Q of the given type to this XQ.
    /// </summary>
    /// <param name="qtype">The type of Q to be added.</param>
    public void NewQ(CueType qtype)
    {
        switch (qtype)
        {
            case CueType.Play:
                qs.Add(new Q("Play", this, null, false, 0, 0, 1.0f));
                break;

            case CueType.Fade:
                qs.Add(new Q("Fade", this, null, 0.0f, 3.0f, 0.5f));
                break;

            case CueType.Stop:
                qs.Add(new Q("Stop", this, null, false));
                break;
        }
    }

    /// <summary>
    /// Delete all Qs from this XQ.
    /// </summary>
    public void ClearAll()
    {
        for (int i = 0; i < qs.Count; ++i)
        {
            qs = new List<Q>();
            Debug.Log(this + ": cleared.");
        }
    }

    /// <summary>
    /// Delete a single Q from this XQ.
    /// </summary>
    /// <param name="index"></param>
    public void Delete(int index)
    {
        qs.Remove(qs[index]);
    }


    public void test()
    {
        Debug.Log("bang!");
    }
}

/* MONOBEHAVIOURS */

/// <summary>
/// This component is instantiated at runtime whenever a Game Object with an Audio Source has been selected in a Q List, although it can be attached in the Editor and it will still be used. It takes over all playback and volume functions from the Audio Source.
/// </summary>
/// 
/// For a fuller implementation, visit https://github.com/xvelastin/unityaudioutility
[RequireComponent(typeof(AudioSource))]
[DisallowMultipleComponent]
public class XQAudioSourceController : MonoBehaviour
{
    // Playback params
    AudioSource source;
    [HideInInspector] public AudioClip clip;
    [HideInInspector] public bool playing = false;
    [HideInInspector] public bool paused = false;
    [HideInInspector] public bool loop = false;
    [HideInInspector] public float outputVolume = 0f;
    [HideInInspector] public float startingVolume;

    // Fade params    
    private float fadeVolume;
    private bool isFading;
    public static float minimumFadeTime = 0.01f;
    public static float maximumFadeTime = 60.0f;

    #region Initialisation
    private void Awake()
    {
        // Takes over audiosource functions and initialises to default.
        if (!source) source = GetComponent<AudioSource>();
        if (!clip) clip = source.clip;
        if (source.isPlaying) source.Stop();

        source.loop = loop;
        source.playOnAwake = false;
        source.volume = XQAudioUtility.ConvertDbtoA(startingVolume);

        // ensures fades start from the starting volume.
        fadeVolume = startingVolume;
    }
    #endregion

    #region Fader
    /// <summary>
    /// Begins a fade on the attached Audiosource component.
    /// </summary>
    /// <param name="targetVol">The target volume (in dB)</param>
    /// <param name="fadetime">The time it takes for the fade to go from the current volume to the target volume.</param>
    public void FadeTo(float targetVol, float fadetime)
    {
        // Create a two-point s-curve.
        AnimationCurve animcur = AnimationCurve.EaseInOut(0, 0, 1, 1);

        if (isFading)
        {
            StopAllCoroutines();
            isFading = false;
        }
        StartCoroutine(StartFadeInDb(fadetime, targetVol, animcur));
        isFading = true;
    }

    /// <summary>
    /// Begins a fade on the attached Audiosource component. The optional curve shape argument defines the rate at which the volume values change over the duration of the fade.
    /// </summary>
    /// <param name="targetVol">The target volume (in dB)</param>
    /// <param name="fadetime">The time it takes for the fade to go from the current volume to the target volume.</param>
    /// <param name="curveShape">Applies a bend to the curve from exponential (0) to logarithmic (1).</param>
    public void FadeTo(float targetVol, float fadetime, float curveShape)
    {
        curveShape = Mathf.Clamp(curveShape, 0.0f, 1.0f);

        Keyframe[] keys = new Keyframe[2];
        keys[0] = new Keyframe(0, 0, 0, Mathf.Sin(curveShape), 0, 1.0f - curveShape);
        keys[1] = new Keyframe(1, 1, 1 - curveShape, 0, curveShape, 0);
        AnimationCurve animcur = new AnimationCurve(keys);

        if (isFading)
        {
            StopAllCoroutines();
            isFading = false;
        }
        StartCoroutine(StartFadeInDb(fadetime, targetVol, animcur));
        isFading = true;
    }

    /// <summary>
    /// Begins a fade on the attached Audiosource component with a custom curve shape passed as an Animation Curve.
    /// </summary>
    /// <param name="targetVol">The target volume (in dB)</param>
    /// <param name="fadetime">The time it takes for the fade to go from the current volume to the target volume.</param>
    /// <param name="animcur">A custom Animation Curve. Values on keyframes of both axes should be constrained from 0-1.</param>
    public void FadeTo(float targetVol, float fadetime, AnimationCurve animcur)
    {
        if (isFading)
        {
            StopAllCoroutines();
            isFading = false;
        }
        StartCoroutine(StartFadeInDb(fadetime, targetVol, animcur));
        isFading = true;
    }

    private IEnumerator StartFadeInDb(float fadetime, float targetVol, AnimationCurve animcur)
    {
        UpdateFadeVolume();
        if (fadetime > maximumFadeTime)
        {
            Debug.Log("Fade time is clamped to " + minimumFadeTime + "s to " + maximumFadeTime + "s");
        }
        fadetime = Mathf.Clamp(fadetime, minimumFadeTime, maximumFadeTime);

        float currentTime = 0f;
        float startVol = fadeVolume;

        if (startVol == targetVol)
        {
            // aborts fade if target volume is the same as starting volume
            isFading = false;
            yield break;
        }

        Debug.Log("XQAudioSourceController on " + gameObject.name + ": Fading from " + startVol + " to " + targetVol + " over " + fadetime);

        while (currentTime < fadetime)
        {
            currentTime += Time.deltaTime;
            fadeVolume = Mathf.Lerp(startVol, targetVol, animcur.Evaluate(currentTime / fadetime));
            UpdateAudiosourceVolume();
            yield return null;
        }

        isFading = false;
        yield break;
    }
    #endregion

    #region Playback
    /// <summary>
    /// Play this Audio Source's clip.
    /// </summary>
    /// <param name="playbackSpeed">The speed, or pitch, of the clip.</param>
    public void Play(float playbackSpeed)
    {
        CheckAudiosource();
        Debug.Log("XQAudioSourceController on " + gameObject.name + ": Play triggered.");
        source.pitch = playbackSpeed;

        // Initialises fadeVolume ready for future fades.
        fadeVolume = startingVolume;
        UpdateAudiosourceVolume();

        // Passes the loop argument through in case it's changed.
        source.loop = loop;

        if (paused)
        {
            source.UnPause();
            paused = false;
            playing = true;
        }
        else
        {
            source.Play();
            playing = true;
        }
    }

    /// <summary>
    /// Stops the Audio Source after a very short fade to minimise audible pops.
    /// </summary>
    /// <param name="pause">If true, the clip will restart at the same point if Play is called again on this Audio Source.</param>
    public void Stop(bool pause)
    {
        if (pause)
        {
            Pause();
            return;
        }

        CheckAudiosource();
        Debug.Log("XQAudioSourceController on " + gameObject.name + ": Stop triggered.");

        // Using a Coroutine both stops any ongoing fade and avoids an audible pop on a snap off.
        StartCoroutine(StopAfterFade(minimumFadeTime));

        paused = false;
        playing = false;
    }

    private void Pause()
    {
        CheckAudiosource();
        Debug.Log("XQAudioSourceController on " + gameObject.name + ": Pause triggered.");

        source.Pause();
        playing = false;
        paused = true;
    }

    private IEnumerator StopAfterFade(float fadeTime)
    {
        FadeTo(XQAudioUtility.minimum, fadeTime);
        yield return new WaitForSeconds(fadeTime);
        source.Stop();
        yield break;
    }
    #endregion

    #region Checks and Updates
    private void CheckAudiosource()
    {
        if (!source)
        {
            source = GetComponent<AudioSource>();
        }
    }

    private void UpdateFadeVolume()
    {
        CheckAudiosource();
        fadeVolume = XQAudioUtility.ConvertAtoDb(source.volume);
    }

    private float GetGain()
    {
        return fadeVolume + outputVolume;
    }

    private void UpdateAudiosourceVolume()
    {
        float currentVol = GetGain();
        source.volume = XQAudioUtility.ConvertDbtoA(currentVol);
    }
    #endregion

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}

/* GLOBAL VARS AND FUNCS  */

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