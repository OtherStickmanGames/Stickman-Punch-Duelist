using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    Stickman player;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Stickman>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.IsMine())
            return;

        if (Input.GetKey(KeyCode.D))
        {
            player.StartWalk(1);
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            player.StopWalk();
        }

        if (Input.GetKey(KeyCode.A))
        {
            player.StartWalk(-1);
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            player.StopWalk();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            player.Punch(PunchPart.RightArm);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            player.Punch(PunchPart.LeftArm);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            player.Punch(PunchPart.LeftLeg);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            player.Punch(PunchPart.RightLeg);
        }
    }
}
