using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;

public class PhotonStickmanController : MonoBehaviourPun, IPunObservable
{

    StickmanController player_1st;
    StickmanController player_2nd;

    List<Vector2> positions = new List<Vector2>();
    List<float> rotations = new List<float>();
    List<float> distances = new List<float>();
    List<float> angles = new List<float>();

    
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GlobalEvents.OnMinePlayerSpawned += Player1st_Spawned;
            GlobalEvents.OnNotMinePlayerSpawned += Player2nd_Spawned;
        }
        else
        {
            GlobalEvents.OnMinePlayerSpawned += Player2nd_Spawned;
            GlobalEvents.OnNotMinePlayerSpawned += Player1st_Spawned;
        }
    }

    private void Player2nd_Spawned(GameObject stickman)
    {
        player_2nd = stickman.GetComponentInChildren<StickmanController>();
    }

    private void Player1st_Spawned(GameObject stickman)
    {
        player_1st = stickman.GetComponentInChildren<StickmanController>();
    }

    void FixedUpdate()
    {
        if (positions.Count > 0 && player_1st && player_2nd)
        {
            int idx = 0;
            
            player_1st.MainBody.MovePosition(positions[idx]);
            player_1st.MainBody.MoveRotation(rotations[idx]);

            idx++;

            foreach (var muscle in player_1st.muscles)
            {
                muscle.bone.MovePosition(positions[idx]);
                muscle.bone.MoveRotation(rotations[idx]);

                idx++;
            }

            player_2nd.MainBody.MovePosition(positions[idx]);
            player_2nd.MainBody.MoveRotation(rotations[idx]);

            idx++;

            foreach (var muscle in player_2nd.muscles)
            {
                muscle.bone.MovePosition(positions[idx]);
                muscle.bone.MoveRotation(rotations[idx]);

                idx++;
            }
        }

        //if (positions.Count > 0)
        //{
        //    int idx = 0;
        //    foreach (var muscle in player_1st.muscles)
        //    {
        //        var bonePos = muscle.boneTransform.position;
        //        var maxDistanceDelta = speed * distances[idx] * Time.deltaTime;// distances[idx] * (1.0f / PhotonNetwork.SerializationRate);
        //        muscle.boneTransform.position = Vector2.MoveTowards(bonePos, positions[idx], maxDistanceDelta);

        //        var maxDisDelta = speed * angles[idx] * Time.deltaTime;//angles[idx] * (1.0f / PhotonNetwork.SerializationRate);
        //        var z = Mathf.MoveTowards(muscle.boneTransform.rotation.eulerAngles.z, rotations[idx], maxDisDelta);
        //        muscle.boneTransform.rotation = Quaternion.Euler(0, 0, z);

        //        idx++;
        //    }
        //}
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (player_1st && player_2nd)
        {
            if (stream.IsWriting)
            {
                Vector2 mainPos = player_1st.transform.position;
                stream.SendNext(mainPos);

                var mainRot = player_1st.transform.rotation.eulerAngles.z;
                stream.SendNext(mainRot);

                foreach (var muscle in player_1st.muscles)
                {
                    Vector2 pos = muscle.bone.transform.position;
                    stream.SendNext(pos);

                    var rot = muscle.bone.transform.rotation.eulerAngles.z;
                    
                    stream.SendNext(rot);
                }

                Vector2 main2Pos = player_2nd.transform.position;
                stream.SendNext(main2Pos);

                var main2Rot = player_2nd.transform.rotation.eulerAngles.z;
                stream.SendNext(main2Rot);

                foreach (var muscle in player_2nd.muscles)
                {
                    Vector2 pos = muscle.bone.transform.position;
                    stream.SendNext(pos);

                    var rot = muscle.bone.transform.rotation.eulerAngles.z;

                    stream.SendNext(rot);
                }
            }
            else
            {
                positions.Clear();
                rotations.Clear();
                distances.Clear();
                angles.Clear();

                var mainPos = (Vector2)stream.ReceiveNext();
                var mainRot = (float)stream.ReceiveNext();

                positions.Add(mainPos);
                rotations.Add(mainRot);

                foreach (var muscle in player_1st.muscles)
                {
                    var pos = (Vector2)stream.ReceiveNext();
                    var rot = (float)stream.ReceiveNext();

                    var dis = Vector2.Distance(pos, muscle.boneTransform.position);
                    var agl = Mathf.Abs(muscle.boneTransform.rotation.eulerAngles.z - rot);

                    positions.Add(pos);
                    rotations.Add(rot);
                    distances.Add(dis);
                    angles.Add(agl);

                }

                var main2Pos = (Vector2)stream.ReceiveNext();
                var main2Rot = (float)stream.ReceiveNext();

                positions.Add(main2Pos);
                rotations.Add(main2Rot);

                foreach (var muscle in player_2nd.muscles)
                {
                    var pos = (Vector2)stream.ReceiveNext();
                    var rot = (float)stream.ReceiveNext();

                    var dis = Vector2.Distance(pos, muscle.boneTransform.position);
                    var agl = Mathf.Abs(muscle.boneTransform.rotation.eulerAngles.z - rot);

                    positions.Add(pos);
                    rotations.Add(rot);
                    distances.Add(dis);
                    angles.Add(agl);

                }
            }
        }
    }

}
