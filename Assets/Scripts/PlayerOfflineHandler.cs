using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOfflineHandler : MonoBehaviour
{
    ObstacleCollisionDetect collisionDetect;
    Stickman stickman;

    private void Start()
    {
        collisionDetect = GetComponent<ObstacleCollisionDetect>();
        stickman = GetComponent<Stickman>();

        stickman.OnDamage += Damage;

        if (stickman.IsPlayer)
        {
            stickman.NickName = DataSaveLoad.NickName;
        }

        if (stickman.IsPlayer)
        {
            GlobalEvents.OnMinePlayerSpawned?.Invoke(gameObject);
        }
        else
        {
            GlobalEvents.OnAIPlayerSpawned?.Invoke(gameObject);
        }
    }

    private void Damage(int value)
    {
        stickman.HP -= value;

        if (!stickman.IsPlayer)
        {
            EarnExp(value);
        }
    }

    private void EarnExp(int value)
    {
        if (!(stickman.HP > 0))
            return;

        foreach (var item in FindObjectsOfType<Stickman>())
        {
            if (item != stickman)
            {
                item.TakenExp(value);
            }
        }
    }

}
