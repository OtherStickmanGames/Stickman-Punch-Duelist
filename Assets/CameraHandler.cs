using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera followCam;
    [SerializeField] CinemachineVirtualCamera staticCam;

    [Space]

    [SerializeField] float limitY = 5.7f;
    [SerializeField] float limitX = 7f;

    Transform player;

    public static bool isSlowMoFollow;

    private void Start()
    {
        GlobalEvents.OnMinePlayerSpawned += PLayer_Spawned;

    }

    private void PLayer_Spawned(GameObject player)
    {
        this.player = player.GetComponent<Stickman>().Hip;

        followCam.Follow = this.player;
    }

    public void SlowMoFollow(Transform target)
    {
        followCam.Follow = target;
        isSlowMoFollow = true;
        followCam.Priority = 11;
    }

    public void PlayerFollowBack()
    {
        followCam.Follow = player;
        isSlowMoFollow = false;
    }

    private void Update()
    {
        if (!player)
            return;

        if (player.position.y > limitY || Mathf.Abs(player.position.x) > limitX)
        {
            if (followCam.Priority < 10)
                followCam.Priority = 11;
        }
        else if (followCam.Priority > 10 && !isSlowMoFollow)
        {
            followCam.Priority = 5;
        }
        
        if(Mathf.Abs(player.position.x) < limitX 
            && player.position.y < limitY
            && followCam.Priority > 11)
        {
            followCam.Priority = 5;
            print("Урурур");
        }
        
        if(timerito > 1)
        {
            //print(player.position.x + " || " + followCam.Priority + " || " + player.position.y);
            timerito = 0;
        }

        timerito += Time.deltaTime;

    }

    float timerito = 0;
}
