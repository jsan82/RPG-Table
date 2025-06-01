using System;
using UnityEngine;

[Serializable]
public class SFXElement
{
    public SFXType Type => _type;
    public AudioClip Clip => _clip;

    [SerializeField] private SFXType _type;
    [SerializeField] private AudioClip _clip;
}
