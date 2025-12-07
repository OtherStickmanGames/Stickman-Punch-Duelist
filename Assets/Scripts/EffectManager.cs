using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] GameObject punchPrefab;

    static EffectManager instance;
    static float timeBetweenSpawnPunchEffect;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        timeBetweenSpawnPunchEffect += Time.deltaTime;
    }

    public static void Punch(Vector2 pos)
    {
        if (timeBetweenSpawnPunchEffect < 0.03f)
            return;

        Instantiate(instance.punchPrefab, pos, Quaternion.identity);
        timeBetweenSpawnPunchEffect = 0;
    }
}
