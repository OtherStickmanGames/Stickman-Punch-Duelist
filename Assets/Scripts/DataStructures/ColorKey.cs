using UnityEngine;
using System;

[Serializable]
public struct ColorKey
{
    [Range(0, 14)]
    public byte index;
    public Color color;
}