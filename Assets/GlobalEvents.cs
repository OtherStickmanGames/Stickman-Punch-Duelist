using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class GlobalEvents
{
    public static Action<GameObject> OnMinePlayerSpawned;
    public static Action<GameObject> OnNotMinePlayerSpawned;
    public static Action<GameObject> OnAIPlayerSpawned;
}
