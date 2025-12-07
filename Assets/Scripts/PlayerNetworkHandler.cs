using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;

public class PlayerNetworkHandler : MonoBehaviourPunCallbacks, IPunObservable
{
    ObstacleCollisionDetect collisionDetect;
    StickmanController ragdoll;
    Stickman stickman;

    private void Start()
    {
        collisionDetect = GetComponent<ObstacleCollisionDetect>();
        ragdoll = GetComponentInChildren<StickmanController>();
        stickman = GetComponent<Stickman>();

        foreach (var muscle in ragdoll.muscles)
        {
            muscle.InitBoneTransform();
        }

        stickman.OnDamage += Damage;

        if (photonView)
        {
            if (photonView.IsMine)
            {
                GlobalEvents.OnMinePlayerSpawned?.Invoke(gameObject);
            }
            else
            {
                GlobalEvents.OnNotMinePlayerSpawned?.Invoke(gameObject);
            }

            stickman.NickName = photonView.Owner.NickName;
            
            collisionDetect.OnSlowMo += SlowMo;
            stickman.OnDirChange += MineMoveDirection_Changed;
            stickman.OnPunch += Punch;
            stickman.OnWalk += Walk;


            photonView.RegisterMethod<DirPhotonData>(EventCode.ChangeDirection, NotMineDirection_Changed);
            photonView.RegisterMethod<bool>(EventCode.SlowMo, SlowMo_Event);
            photonView.RegisterMethod<byte[]>(EventCode.Colors, Colors_Receive);

            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RegisterMethod<PunchEventPhotonData>(EventCode.PunchEvent, MasterPunch);
                photonView.RegisterMethod<WalkEventPhotonData>(EventCode.WalkEvent, MasterWalk);
            }
            else
            {
                photonView.RegisterMethod<DamagePhotonData>(EventCode.Damage, NotMasterDamage, true);

            }

            StartCoroutine(DirFitter());
            SendColors();

        }
        else
        {
            GlobalEvents.OnAIPlayerSpawned?.Invoke(gameObject);
        }

    }

    private void Colors_Receive(byte[] colorsID)
    {
        stickman.GetComponent<Coloring>().ApplyReceiveColors(colorsID);   
    }

    IEnumerator DirFitter()
    {
        yield return new WaitForSeconds(0.3f);

        if (!PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            stickman.StartWalk(-1);
        }
    }

    private void SendColors()
    {
        StartCoroutine(DelaySend());

        IEnumerator DelaySend()
        {
            yield return new WaitForSeconds(1f);

            byte[] colorsID = new byte[11];
            for (int i = 0; i < colorsID.Length; i++)
            {
                colorsID[i] = stickman.ViewsBones[i].ColorID;
            }

            photonView.RaiseEvent(EventCode.Colors, colorsID);
        }
    }

    private void SlowMo(bool isSlow)
    {
        
        photonView.RaiseEvent(EventCode.SlowMo, isSlow);
    }

    private void SlowMo_Event(bool isSlow)
    {
        collisionDetect.SlowMotion(3);
        //print("Слоу мо, прилетело");
    }

    private void Damage(int value)
    {
        if (PhotonNetwork.IsMasterClient)
            MasterDamage(value);
    }

    private void NotMasterDamage(DamagePhotonData damageData)
    {
        if(damageData.viewID == photonView.ViewID)
        {
            stickman.HP -= damageData.Value;

            bool damageToEnemyPlayer = !photonView.IsMine;

            if (damageToEnemyPlayer && stickman.HP > 0)
            {
                EarnExp(damageData.Value);
                AddInflictedPunch();
            }
        }
    }

    private void MasterDamage(int value)
    {
        stickman.HP -= value;

        bool damageToAI = photonView == null;
        bool damageToEnemyPlayer = false;

        if(!damageToAI)
            damageToEnemyPlayer = !photonView.IsMine;

        if (damageToAI || damageToEnemyPlayer)
        {
            if (stickman.HP > 0)
            {
                EarnExp(value);
                AddInflictedPunch();
            }
        }

        if (photonView)
        {
            var data = new DamagePhotonData { viewID = photonView.ViewID, Value = value };
            photonView.RaiseEventAll(EventCode.Damage, data);
        }
    }

    private void Walk(int value)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            NotMasterWalk(value);
        }
    }

    private void NotMasterWalk(int value)
    {
        var data = new WalkEventPhotonData { viewID = photonView.ViewID, Value = value };
        photonView.RaiseEvent(EventCode.WalkEvent, data);
    }

    private void MasterWalk(WalkEventPhotonData walkData)
    {
        if (photonView.ViewID == walkData.viewID)
        {
            if(walkData.Value != 0)
            {
                stickman.StartWalk(walkData.Value);
            }
            else
            {
                stickman.StopWalk();
            }
        }
    }

    

    private void Punch(PunchPart part)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            NotMasterPunch(part);
        }
    }

    private void NotMasterPunch(PunchPart part)
    {
        var data = new PunchEventPhotonData { viewID = photonView.ViewID, Value = (int)part };
        photonView.RaiseEvent(EventCode.PunchEvent, data);
    }

    private void MasterPunch(PunchEventPhotonData punchData)
    {
        if(photonView.ViewID == punchData.viewID)
        {
            stickman.Punch((PunchPart)punchData.Value);
        }
    }

    private void MineMoveDirection_Changed(int dir)
    {
        var data = new DirPhotonData { viewID = photonView.ViewID, Value = dir };

        photonView.RaiseEvent(EventCode.ChangeDirection, data);
    }


    void NotMineDirection_Changed(DirPhotonData dirData)
    {
        if (photonView.ViewID == dirData.viewID)
            stickman.Direction = dirData.Value;
    }

    private void EarnExp(int value)
    {
        foreach(var item in FindObjectsOfType<PlayerNetworkHandler>())
        {
            if(item != this)
            {
                item.GetComponent<Stickman>().TakenExp(value);
            }
        }
    }

    private void AddInflictedPunch()
    {
        foreach (var item in FindObjectsOfType<PlayerNetworkHandler>())
        {
            if (item != this)
            {
                
                item.GetComponent<Stickman>().AddInflictedPunch();
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
