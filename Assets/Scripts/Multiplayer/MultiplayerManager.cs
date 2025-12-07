using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using MultiplayerEventData;

public class MultiplayerManager : MonoBehaviourPunCallbacks, IOnEventCallback
{

    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject playerAIPrefab;
    [SerializeField] GameObject rigidbodyController;

    bool applicationIsQuit;

    public static MultiplayerManager Instance;

    public static bool IsOffline { get; set; }
    public static bool IsMaster => PhotonNetwork.IsMasterClient;
    public static List<Player> Players { get; } = new List<Player>();

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Больше 1-го MultiplayerManager ");
        }
        Instance = this;

        if (!PhotonNetwork.IsConnected && !IsOffline)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            //if(PhotonNetwork.IsMasterClient)
            //PhotonNetwork.Instantiate(rigidbodyController.name, Vector3.zero, Quaternion.identity);
        }


    }

    void Start()
    {
        PhotonPeer.RegisterType(typeof(EventHpContent), 012, SerializeEventHpContent, DeserializeEventHpContent);
        PhotonPeer.RegisterType(typeof(FirePhotonData), 011, SerializeEventTurnContent, DeserializeEventTurnContent);
        PhotonPeer.RegisterType(typeof(EnemyHealthPoint), 015, SerializeHealthPoint, DeserializeEnemyHealthPoint);
        PhotonPeer.RegisterType(typeof(IncreasePhotonData), 088, SerializeIncreaseData, DeserializeIncreaseData);
        PhotonPeer.RegisterType(typeof(DirPhotonData), 078, SerializeDirectionData, DeserializeDirectionData);
        PhotonPeer.RegisterType(typeof(PunchEventPhotonData), 075, SerializePunchEventData, DeserializePunchEventData);
        PhotonPeer.RegisterType(typeof(WalkEventPhotonData), 076, SerializeWalkEventData, DeserializeWalkEventData);
        PhotonPeer.RegisterType(typeof(DamagePhotonData), 074, SerializeDamageData, DeserializeDamageData);

        if (playerPrefab)
            SpawnPlayer();

        if (PhotonNetwork.PlayerList.Length < 2)
        {
            var ai = Instantiate(playerAIPrefab);
            if (IsOffline)
            {
                ai.AddComponent<PlayerOfflineHandler>();
                Destroy(ai.GetComponent<PlayerNetworkHandler>());
            }
        }

        if (!IsOffline)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
    }

    public GameObject SpawnPlayer()
    {
        Vector3 pos;
        if(PhotonNetwork.IsMasterClient || IsOffline)
        {
            pos = new Vector3(-5, -3.21f, 0);
        }
        else
        {
            pos = new Vector3(5, -3.21f, 0);
        }

        if (IsOffline)
        {
            var player = Instantiate(playerPrefab, pos, Quaternion.identity);
            player.AddComponent<PlayerOfflineHandler>();
            Destroy(player.GetComponent<PlayerNetworkHandler>());
            return player;
        }
        else
        {
            return PhotonNetwork.Instantiate(playerPrefab.name, pos, Quaternion.identity);
        }
    }

    public static void AddPlayer(Player player) => Players.Add(player);
    

    public static T Spawn<T>(T prefab, Vector3 position)
    {
        var prefabName = (prefab as MonoBehaviour).name;
        var spawned = PhotonNetwork.Instantiate(prefabName, position, Quaternion.identity);

        return spawned.GetComponent<T>();
    }

    public static GameObject Spawn(GameObject prefab, Vector3 position)
    {
        return PhotonNetwork.Instantiate(prefab.name, position, Quaternion.identity);
    }

    public static void DestroyByPhoton(GameObject target)
    {
        PhotonNetwork.Destroy(target);
    }

    public static void RaiseEvent<T>(EventCode eventCode, T data, int targetActor)
    {
        RaiseEventOptions option = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        option.TargetActors = new int[] { targetActor };
        PhotonNetwork.RaiseEvent((byte)eventCode, data, option, sendOptions);
    }

    public static void RaiseEvent<T>(PhotonView actor, EventCode eventCode, T data)
    {
        RaiseEventOptions option = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        option.TargetActors = new int[] { actor.OwnerActorNr };
        PhotonNetwork.RaiseEvent((byte)eventCode, data, option, sendOptions);
    }

    public static void RaiseEvent<T>(EventCode eventCode, T data)
    {
        RaiseEventOptions option = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent((byte)eventCode, data, option, sendOptions);
    }

    public static void RaiseEvent(PhotonView actor, EventCode eventCode)
    {
        RaiseEventOptions option = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent((byte)eventCode, actor.Owner.ActorNumber, option, sendOptions);
    }

    //Dictionary<PhotonView, Dictionary<EventCode, Action>> ActorMethods = new Dictionary<PhotonView, Dictionary<EventCode, Action>>();

    Dictionary<byte, List<ActorMethods>> actorMethods = new Dictionary<byte, List<ActorMethods>>();

    public class ActorMethods
    {
        public PhotonView actor;
        public Action method;
        public bool allActors;
        //public Action<object> objectMethod;
    }

    public class ParamMethods<T> : ActorMethods
    {
        public Action<T> paramMethod;
    }

    //public static void RegisterMethod(PhotonView actor, EventCode eventCode, Action<object> method)
    //{
    //    var actorMethods = Instance.actorMethods;
    //    byte code = (byte)eventCode;
    //    if (actorMethods.ContainsKey(code))
    //    {
    //        var actors = actorMethods[code];
    //        ActorMethods am = null;
    //        foreach (var item in actors)
    //        {
    //            if (item.actor == actor) am = item as ActorMethods;
    //        }
    //        if (am == null)
    //        {
    //            var newActor = new ActorMethods { actor = actor, objectMethod = method };
    //            actors.Add(newActor);
    //        }
    //    }
    //    else
    //    {
    //        var newActor = new ActorMethods { actor = actor, objectMethod = method };
    //        actorMethods.Add(code, new List<ActorMethods> { newActor });
    //    }
    //}

    public static void RegisterMethod<T>(PhotonView actor, EventCode eventCode, Action<T> method, bool allActros)
    {
        var actorMethods = Instance.actorMethods;
        byte code = (byte)eventCode;
        if (actorMethods.ContainsKey(code))
        {
            var actors = actorMethods[code];
            ParamMethods<T> am = null;
            foreach (var item in actors)
            {
                if (item.actor == actor) am = item as ParamMethods<T>;
            }
            if (am == null)
            {
                var newActor = new ParamMethods<T> { actor = actor, paramMethod = method, allActors = allActros };
                actors.Add(newActor);
            }
        }
        else
        {
            var newActor = new ParamMethods<T> { actor = actor, paramMethod = method, allActors = allActros };
            actorMethods.Add(code, new List<ActorMethods> { newActor });
        }
    }

    public static void RegisterMethod(PhotonView actor, EventCode eventCode, Action method)
    {
        var actorMethods = Instance.actorMethods;
        byte code = (byte)eventCode;
        if (actorMethods.ContainsKey(code))
        {
            var actors = actorMethods[code];
            ActorMethods am = null;
            foreach (var item in actors)
            {
                if (item.actor == actor) am = item;
            }
            if (am == null)
            {
                var newActor = new ActorMethods { actor = actor, method = method };
                actors.Add(newActor);
            }
        }
        else
        {
            var newActor = new ActorMethods { actor = actor, method = method };
            actorMethods.Add(code, new List<ActorMethods> { newActor });
        }
    }

    //public static void RaiseEvent(EventCode code, object content)
    //{
    //    RaiseEventOptions option = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
    //    SendOptions sendOptions = new SendOptions { Reliability = true };
    //    PhotonNetwork.RaiseEvent((byte)code, content, option, sendOptions);
    //}

    public static void RaiseEvent(EventCode code, object content, ReceiverGroup receiverGroup)
    {
        RaiseEventOptions option = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent((byte)code, content, option, sendOptions);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //Player player = GameManager.instance.players.First(p => p.GetPhoton().Owner == null);
        //GameManager.instance.players.Remove(player);
       
        //if (player.GetComponent<PhotonView>().IsMine == true && PhotonNetwork.IsConnected == true)//возможно проверка лишняя но все работает норм
        //{
        //    PhotonNetwork.Destroy(player.gameObject);
        //}

        //    EventSystem.TriggerEvent(EventKey.PlayerLeave);
    }

    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        if (!applicationIsQuit)
        {
            SceneManager.LoadScene(0);
        }
    }

    private void OnApplicationQuit()
    {
        applicationIsQuit = true;
    }

    public void OnEvent(EventData photonEvent)
    {
        if (actorMethods.ContainsKey(photonEvent.Code))
        {
            var actors = actorMethods[photonEvent.Code];

            foreach (var actor in actors)
            {
                if (actor.actor == null)
                {
                    continue;
                }

                if (actor.actor.Owner.ActorNumber == photonEvent.Sender || actor.allActors)
                {
                    actor.method?.Invoke();

                    var data = photonEvent.CustomData;

                    if (data is byte[])
                    {
                        (actor as ParamMethods<byte[]>).paramMethod?.Invoke((byte[])photonEvent.CustomData);
                    }
                    if (data is float)
                    {
                        (actor as ParamMethods<float>).paramMethod?.Invoke((float)photonEvent.CustomData);
                    }
                    if (data is int)
                    {
                        (actor as ParamMethods<int>).paramMethod?.Invoke((int)photonEvent.CustomData);
                    }
                    if (data is bool)
                    {
                        (actor as ParamMethods<bool>).paramMethod?.Invoke((bool)photonEvent.CustomData);
                    }
                    if (data is string)
                    {
                        (actor as ParamMethods<string>).paramMethod?.Invoke((string)photonEvent.CustomData);
                    }
                    if (data is Vector2)
                    {
                        (actor as ParamMethods<Vector2>).paramMethod?.Invoke((Vector2)photonEvent.CustomData);
                    }
                    if (data is EnemyHealthPoint)
                    {
                        (actor as ParamMethods<EnemyHealthPoint>).paramMethod?.Invoke((EnemyHealthPoint)photonEvent.CustomData);
                    }
                    if (data is CreateDiceParams)
                    {
                        (actor as ParamMethods<CreateDiceParams>).paramMethod?.Invoke((CreateDiceParams)photonEvent.CustomData);
                    }
                    if (data is FirePhotonData)
                    {
                        (actor as ParamMethods<FirePhotonData>).paramMethod?.Invoke((FirePhotonData)photonEvent.CustomData);
                    }
                    if (data is IncreasePhotonData)
                    {
                        (actor as ParamMethods<IncreasePhotonData>).paramMethod?.Invoke((IncreasePhotonData)photonEvent.CustomData);
                    }
                    if (data is DirPhotonData)
                    {
                        (actor as ParamMethods<DirPhotonData>).paramMethod?.Invoke((DirPhotonData)photonEvent.CustomData);
                    }
                    if (data is PunchEventPhotonData)
                    {
                        (actor as ParamMethods<PunchEventPhotonData>).paramMethod?.Invoke((PunchEventPhotonData)photonEvent.CustomData);
                    }
                    if (data is WalkEventPhotonData)
                    {
                        (actor as ParamMethods<WalkEventPhotonData>).paramMethod?.Invoke((WalkEventPhotonData)photonEvent.CustomData);
                    }
                    if (data is DamagePhotonData)
                    {
                        (actor as ParamMethods<DamagePhotonData>).paramMethod?.Invoke((DamagePhotonData)photonEvent.CustomData);
                    }
                }

            }
        }
      
    }


    #region --== Serialize / Deserialize Photon Data ==--
    public static object DeserializeEventHpContent(byte[] data)
    {
        EventHpContent result = new EventHpContent();
        result.viewId = BitConverter.ToInt32(data, 0);
        result.hp = BitConverter.ToInt32(data, 4);

        return result;
    }

    public static byte[] SerializeEventHpContent(object obj)
    {
        EventHpContent ehc = (EventHpContent)obj;
        byte[] result = new byte[8];

        BitConverter.GetBytes(ehc.viewId).CopyTo(result, 0);
        BitConverter.GetBytes(ehc.hp).CopyTo(result, 4);

        return result;
    }

    public static object DeserializeEventTurnContent(byte[] data)
    {
        FirePhotonData result = new FirePhotonData
        {
            viewID = BitConverter.ToInt32(data, 0),
            yAngle = BitConverter.ToSingle(data, 4)
        };
        return result;
    }

    public static byte[] SerializeEventTurnContent(object obj)
    {
        FirePhotonData ehc = (FirePhotonData)obj;
        byte[] result = new byte[8];
        BitConverter.GetBytes(ehc.viewID).CopyTo(result, 0);
        BitConverter.GetBytes(ehc.yAngle).CopyTo(result, 4);
        return result;
    }

    public static object DeserializeEnemyHealthPoint(byte[] data)
    {
        EnemyHealthPoint result = new EnemyHealthPoint
        {
            viewId = BitConverter.ToInt32(data, 0),
            healthPoint = BitConverter.ToInt32(data, 4)
        };
        return result;
    }

    public static byte[] SerializeHealthPoint(object obj)
    {
        EnemyHealthPoint ehc = (EnemyHealthPoint)obj;
        byte[] result = new byte[8];
        BitConverter.GetBytes(ehc.viewId).CopyTo(result, 0);
        BitConverter.GetBytes(ehc.healthPoint).CopyTo(result, 4);
        return result;
    }
    
    public static object DeserializeCreateDiceParams(byte[] data)
    {
        CreateDiceParams result = new CreateDiceParams
        {
            stage = BitConverter.ToInt32(data, 0),
            kind = BitConverter.ToInt32(data, 4),
            pos = new Vector2
            {
                x = BitConverter.ToSingle(data, 8),
                y = BitConverter.ToSingle(data, 12),
            }
        };
        return result;
    }

    public static byte[] SerializeCreateDiceParams(object obj)
    {
        CreateDiceParams cdp = (CreateDiceParams)obj;
        byte[] result = new byte[16];
        BitConverter.GetBytes(cdp.stage).CopyTo(result, 0);
        BitConverter.GetBytes(cdp.kind).CopyTo(result, 4);
        BitConverter.GetBytes(cdp.pos.x).CopyTo(result, 8);
        BitConverter.GetBytes(cdp.pos.y).CopyTo(result, 12);
        return result;
    }

    public static object DeserializeIncreaseData(byte[] data)
    {
        IncreasePhotonData result = new IncreasePhotonData
        {
            viewID = BitConverter.ToInt32(data, 0),
            Value = data[4]
        };
        return result;
    }

    public static byte[] SerializeIncreaseData(object obj)
    {
        IncreasePhotonData ehc = (IncreasePhotonData)obj;
        byte[] result = new byte[5];
        BitConverter.GetBytes(ehc.viewID).CopyTo(result, 0);
        result[4] = ehc.Value;
        return result;
    }

    public static object DeserializeDirectionData(byte[] data)
    {
        DirPhotonData result = new DirPhotonData
        {
            viewID = BitConverter.ToInt32(data, 0),
            Value  = BitConverter.ToInt32(data, 4)
        };
        return result;
    }

    public static byte[] SerializeDirectionData(object obj)
    {
        DirPhotonData ehc = (DirPhotonData)obj;
        byte[] result = new byte[8];
        BitConverter.GetBytes(ehc.viewID).CopyTo(result, 0);
        BitConverter.GetBytes(ehc.Value).CopyTo(result, 4);
        return result;
    }

    public static object DeserializePunchEventData(byte[] data)
    {
        PunchEventPhotonData result = new PunchEventPhotonData
        {
            viewID = BitConverter.ToInt32(data, 0),
            Value = BitConverter.ToInt32(data, 4)
        };
        return result;
    }

    public static byte[] SerializePunchEventData(object obj)
    {
        PunchEventPhotonData ehc = (PunchEventPhotonData)obj;
        byte[] result = new byte[8];
        BitConverter.GetBytes(ehc.viewID).CopyTo(result, 0);
        BitConverter.GetBytes(ehc.Value).CopyTo(result, 4);
        return result;
    }

    public static object DeserializeWalkEventData(byte[] data)
    {
        WalkEventPhotonData result = new WalkEventPhotonData
        {
            viewID = BitConverter.ToInt32(data, 0),
            Value = BitConverter.ToInt32(data, 4)
        };
        return result;
    }

    public static byte[] SerializeWalkEventData(object obj)
    {
        WalkEventPhotonData ehc = (WalkEventPhotonData)obj;
        byte[] result = new byte[8];
        BitConverter.GetBytes(ehc.viewID).CopyTo(result, 0);
        BitConverter.GetBytes(ehc.Value).CopyTo(result, 4);
        return result;
    }

    public static object DeserializeDamageData(byte[] data)
    {
        DamagePhotonData result = new DamagePhotonData
        {
            viewID = BitConverter.ToInt32(data, 0),
            Value = BitConverter.ToInt32(data, 4)
        };
        return result;
    }

    public static byte[] SerializeDamageData(object obj)
    {
        DamagePhotonData ehc = (DamagePhotonData)obj;
        byte[] result = new byte[8];
        BitConverter.GetBytes(ehc.viewID).CopyTo(result, 0);
        BitConverter.GetBytes(ehc.Value).CopyTo(result, 4);
        return result;
    }
    #endregion

}

public class FirePhotonData
{
    public float yAngle;
    public int viewID;
}

public class DirPhotonData
{
    public int viewID;
    public int Value;
}

public class PunchEventPhotonData
{
    public int viewID;
    public int Value;
}

public class WalkEventPhotonData
{
    public int viewID;
    public int Value;
}

public class EventHpContent
{
    public int viewId;
    public int hp;
}

public class IncreasePhotonData
{
    public int viewID;
    public byte Value;
}

public class DamagePhotonData
{
    public int viewID;
    public int Value;
}

public enum EventCode
{
    PlayerStartAttack = 1,
    PlayerStopAttack = 2,
    ChangeDirection = 5,
    PunchEvent = 6,
    WalkEvent = 7,
    Damage = 27,
    Colors = 16,
    WeaponSpellActivation = 17,
    WeaponSpellDeactivation = 18,
    EnemySetHealthPoint = 11,
    EnemyDestroyed = 69,
    SlowMo = 80,
    
    Increase = 88,
    // PERKS
    Perk1 = 30,
    Perk2 = 31,
    Perk3 = 32,
    Perk4 = 32,
    Perk5 = 32,
    Perk6 = 32,
    Perk7 = 32,
    Perk8 = 32,
    Perk9 = 32
}



