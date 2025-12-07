
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerEventData
{
    public class EnemyHealthPoint
    {
        public int viewId;
        public int healthPoint;
    }

    public class CreateDiceParams
    {
        public int stage;
        public int kind;
        public Vector2 pos;

        public CreateDiceParams()
        {
            //stage = dice.Stage;
            //kind = (int)dice.KindDice;
            //pos = dice.UsedCell.pos;
        }

        
    }
}
