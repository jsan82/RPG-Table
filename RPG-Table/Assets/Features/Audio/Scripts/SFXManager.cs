using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages and plays sound effects, including one-shot and looped audio.
/// </summary>
public class SFXManager : MonoBehaviour
{
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioSource _loopedSource;
    [SerializeField] private SFXElement[] _elements;

    private Dictionary<SFXType, AudioClip> _sfxDict = new();

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        foreach (var element in _elements)
        {
            _sfxDict.Add(element.Type, element.Clip);
        }
    }

    public void Play(SFXType type)
    {
        var clip = _sfxDict[type];
        if (clip != null)
        {
            _source.PlayOneShot(clip);
        }
    }

    public void PlayLooped()
    {
        _loopedSource.Play();
    }

    public void StopLooped()
    {
        _loopedSource.Stop();
    }
}
