using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioClip[] punches;

    static SoundManager instance;

    private void Awake()
    {
        instance = this;
    }

    public static void Punch(Vector3 pos)
    {
        var sound = instance.punches[Random.Range(0, instance.punches.Length)];
        AudioSource.PlayClipAtPoint(sound, pos);
    }
}
