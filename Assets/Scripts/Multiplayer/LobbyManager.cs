using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] string sceneNameToLoad;

    [Space(3)]
    [SerializeField] byte maxPlayers = 10;

    [Header("UI References")]
    [SerializeField] TextMeshProUGUI logText;
    [SerializeField] Button btnConnect;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TMP_Text nickname;
    [SerializeField] GameObject loading;
    [SerializeField] GameObject txtStart;
    [SerializeField] GameObject btnStart;

    [Space]

    [SerializeField] TMP_InputField changeNickName;
    [SerializeField] Button btnOkName;

    ExitGames.Client.Photon.Hashtable roomProps;

    [Space]
    [SerializeField]
    float maxTimeWaitingPlayer = 30;

    [Space][SerializeField]
    float timeWaitingPlayer = -1;


    void Awake()
    {
        MultiplayerManager.IsOffline = false;

        btnStart.SetActive(false);

        if (PhotonNetwork.CountOfPlayers > 19)
        {
            MultiplayerManager.IsOffline = true;

            btnStart.SetActive(true);

            btnConnect.onClick.RemoveAllListeners();
            btnConnect.onClick.AddListener(LoadArenaOffline);
        }
        else if (!PhotonNetwork.IsConnected)
        {

            if (PhotonNetwork.NickName == string.Empty)
            {
                PhotonNetwork.NickName = DataSaveLoad.NickName;
                nickname.text = PhotonNetwork.NickName;
            }

            PhotonNetwork.LocalPlayer.NickName = PhotonNetwork.NickName;
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = "1";

            PhotonNetwork.ConnectUsingSettings();

            PhotonNetwork.SerializationRate = 38;
            PhotonNetwork.SendRate = 38;
        }
        else
        {
            btnStart.SetActive(true);
        }

        // ________________________________________________________
        roomProps = new ExitGames.Client.Photon.Hashtable();
        
        roomProps["idi_naxyi"] = PhotonNetwork.GameVersion;


        btnConnect.onClick.AddListener(JoinRoom);
        btnOkName.onClick.AddListener(NickName_Changed);


        var ebat = FindObjectOfType<Advertising>();
        ebat.onVideoClosed += () => PhotonNetwork.ConnectUsingSettings();
        ebat.xyeta += () => Log("ну це пизда не понятная");

        PhotonNetwork.KeepAliveInBackground = 180;

        print(PhotonNetwork.NetworkClientState);

        

    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            Log("Ур хур ебур");
            StartCoroutine(Connect());
        }

        IEnumerator Connect()
        {
            yield return new WaitForSeconds(0.3f);

            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void Start()
    {
        nickname.text = PhotonNetwork.NickName;

        
        
    }

    private void Update()
    {
        if(timeWaitingPlayer > 0)
        {
            timeWaitingPlayer += Time.deltaTime;

            if(timeWaitingPlayer > maxTimeWaitingPlayer)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;

                timeWaitingPlayer = -1;

                LoadArena();
            }
        }
    }

    private void NickName_Changed()
    {
        PhotonNetwork.NickName = changeNickName.text;
        nickname.text = PhotonNetwork.NickName;
        btnOkName.transform.parent.gameObject.SetActive(false);
        DataSaveLoad.NickName = changeNickName.text;
    }

    public override void OnConnected()
    {
        Log("шо блять?");
    }

    public override void OnConnectedToMaster()
    {
        Log("Некий хуежуй: " + PhotonNetwork.NickName + " присоеденился к пиздатой игруле");

        btnStart.SetActive(true);
    }

    void CreateRoom()
    {

        string roomName = "Room " + Random.Range(1000, 10000);
        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers, CleanupCacheOnLeave = false };
        PhotonNetwork.CreateRoom(roomName, options, null);

        //string[] props = new string[1];
        //props[0] = "idi_naxyi";

        //PhotonNetwork.CreateRoom(null, new RoomOptions
        //{
        //    MaxPlayers = maxPlayers,
        //    CleanupCacheOnLeave = false,
        //    //CustomRoomProperties = roomProps,
        //    //CustomRoomPropertiesForLobby = props
        //});
    }

    public void JoinRoom()
    {
        print(PhotonNetwork.CountOfRooms);
        Log("нажато");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    public override void OnJoinedRoom()
    {
        print("Припиздяшил, Уебок " + PhotonNetwork.NickName);

        timeWaitingPlayer = Time.deltaTime;
        loading.SetActive(true);
        txtStart.SetActive(false);

        CheckCountPlayers();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        CheckCountPlayers();
    }

    void CheckCountPlayers()
    {
        if (PhotonNetwork.PlayerList.Length == 2) 
        {
            LoadArena();
        }       
    }

    void LoadArena()
    {
        PhotonNetwork.LoadLevel(sceneNameToLoad);
    }

    void LoadArenaOffline()
    {
        SceneManager.LoadScene(sceneNameToLoad);
    }

    void Log(string msg)
    {
        logText.text += "\n";
        logText.text += msg;
    }

    
}
