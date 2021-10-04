using System.Collections;
using UnityEngine;

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
        source.volume = XQ.AudioUtility.ConvertDbtoA(startingVolume);

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

        // This took me SO long to get right.
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
    public void Play(float playbackSpeed)
    {
        CheckAudiosource();
        Debug.Log("XQAudioSourceController on " + gameObject.name + ": Play triggered.");
        source.pitch = playbackSpeed;

        fadeVolume = startingVolume;
        UpdateAudiosourceVolume();

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

    public void Stop(bool pause)
    {
        if (pause)
        {
            Pause();
            return;
        }

        CheckAudiosource();
        Debug.Log("XQAudioSourceController on " + gameObject.name + ": Stop triggered.");

        // Coroutine both stops any ongoing fade and avoids an audible pop on a snap off.
        StartCoroutine(StopAfterFade(minimumFadeTime));

        paused = false;
        playing = false;
    }

    public void Pause()
    {
        CheckAudiosource();
        Debug.Log("XQAudioSourceController on " + gameObject.name + ": Pause triggered.");

        source.Pause();
        playing = false;
        paused = true;
    }

    IEnumerator StopAfterFade(float fadeTime)
    {
        FadeTo(XQ.AudioUtility.minimum, fadeTime);
        yield return new WaitForSeconds(fadeTime);
        source.Stop();
        yield break;
    }
    #endregion

    #region Checks and Updates
    private void CheckAudiosource()
    {
        if (!source) source = GetComponent<AudioSource>();
    }

    private void UpdateFadeVolume()
    {
        CheckAudiosource();
        fadeVolume = XQ.AudioUtility.ConvertAtoDb(source.volume);
    }

    private float GetGain()
    {
        return fadeVolume + outputVolume;
    }

    private void UpdateAudiosourceVolume()
    {
        float currentVol = GetGain();
        source.volume = XQ.AudioUtility.ConvertDbtoA(currentVol);
    }
    #endregion

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
