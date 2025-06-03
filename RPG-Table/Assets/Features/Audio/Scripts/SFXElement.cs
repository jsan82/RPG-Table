using System;
using UnityEngine;

/// <summary>
/// Represents a sound effect element that pairs an SFXType with its corresponding AudioClip.
/// </summary>
[Serializable]
public class SFXElement
{
    /// <summary>
    /// Gets the type identifier for this sound effect.
    /// </summary>
    public SFXType Type => _type;

    /// <summary>
    /// Gets the AudioClip associated with this sound effect.
    /// </summary>
    public AudioClip Clip => _clip;

    /// <summary>
    /// The sound effect type identifier (serialized field).
    /// </summary>
    [SerializeField] private SFXType _type;

    /// <summary>
    /// The audio clip asset for this sound effect (serialized field).
    /// </summary>
    [SerializeField] private AudioClip _clip;
}