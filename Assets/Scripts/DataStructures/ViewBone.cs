using UnityEngine;
using System;

[Serializable]
public class ViewBone
{
    [SerializeField] public Transform bone;
    [SerializeField] public SpriteRenderer view;

    public byte ColorID { get; set; }
}
