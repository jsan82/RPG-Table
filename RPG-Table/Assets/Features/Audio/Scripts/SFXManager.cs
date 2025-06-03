using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages and plays sound effects, including one-shot and looped audio.
/// </summary>
public class SFXManager : MonoBehaviour
{
    /// <summary>
    /// The AudioSource component used for playing one-shot sound effects.
    /// </summary>
    [SerializeField] private AudioSource _source;
    
    /// <summary>
    /// The AudioSource component used for playing looped sound effects.
    /// </summary>
    [SerializeField] private AudioSource _loopedSource;
    
    /// <summary>
    /// Array of SFXElement objects that map SFXType enum values to AudioClip assets.
    /// </summary>
    [SerializeField] private SFXElement[] _elements;

    /// <summary>
    /// Dictionary that stores the mapping between SFXType and corresponding AudioClip.
    /// </summary>
    private Dictionary<SFXType, AudioClip> _sfxDict = new();

    /// <summary>
    /// Ensures the SFXManager persists between scene loads.
    /// </summary>
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Initializes the sound effect dictionary by populating it with the configured SFXElements.
    /// </summary>
    private void Start()
    {
        foreach (var element in _elements)
        {
            _sfxDict.Add(element.Type, element.Clip);
        }
    }

    /// <summary>
    /// Plays a one-shot sound effect of the specified type.
    /// </summary>
    /// <param name="type">The type of sound effect to play, as defined in the SFXType enum.</param>
    public void Play(SFXType type)
    {
        var clip = _sfxDict[type];
        if (clip != null)
        {
            _source.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// Starts playing the configured looped sound effect.
    /// The sound will continue playing until explicitly stopped.
    /// </summary>
    public void PlayLooped()
    {
        _loopedSource.Play();
    }

    /// <summary>
    /// Stops the currently playing looped sound effect.
    /// </summary>
    public void StopLooped()
    {
        _loopedSource.Stop();
    }
}