using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class UI : MonoBehaviour
{
    [SerializeField] Joystick joystick;

    [SerializeField] Button btnRightArm;
    [SerializeField] Button btnLeftArm;
    [SerializeField] Button btnRightLeg;
    [SerializeField] Button btnLeftLeg;

    [SerializeField] GameObject lockLeftArm;
    [SerializeField] GameObject lockRightLeg;
    [SerializeField] GameObject lockLeftLeg;
    
    [Header("Panel Completed")]

    [SerializeField] GameObject panelCompleted;
    [SerializeField] TextMeshProUGUI labelStatus;
    [SerializeField] TextMeshProUGUI labelExp;
    [SerializeField] TMP_Text labelCountPunches;
    [SerializeField] TMP_Text labelEarnedCoins;
    [SerializeField] Button btnOk;

    [Space]


    Stickman player;
    Stickman enemy;

    int originExpValue;


    public static int countGame;

    void Start()
    {
        GlobalEvents.OnMinePlayerSpawned += MinePlayer_Spawned;
        GlobalEvents.OnAIPlayerSpawned += AIPlayer_Spawned;
        GlobalEvents.OnNotMinePlayerSpawned += NotMinePlayer_Spawned;


        btnRightArm.onClick.AddListener(() => player.Punch(PunchPart.RightArm));
        btnLeftArm.onClick.AddListener(() => player.Punch(PunchPart.LeftArm));
        btnRightLeg.onClick.AddListener(() => player.Punch(PunchPart.RightLeg));
        btnLeftLeg.onClick.AddListener(() => player.Punch(PunchPart.LeftLeg));

        panelCompleted.SetActive(false);

        btnOk.onClick.AddListener(LoadMenu);

        originExpValue = DataSaveLoad.Exp;

        CheckPunchLocks();

        countGame++;
    }

    private void NotMinePlayer_Spawned(GameObject player)
    {
        enemy = player.GetComponent<Stickman>();
        enemy.onVictory += Victory;
    }

    private void AIPlayer_Spawned(GameObject player)
    {
        enemy = player.GetComponent<Stickman>();
        enemy.onVictory += Victory;
    }

    private void MinePlayer_Spawned(GameObject player)
    {
        this.player = player.GetComponent<Stickman>();
        this.player.onDefeat += Defeat;
    }

    private void Victory()
    {
        if (Language.rus)
        {
            labelStatus.text = "Победа";
        }
        else
        {
            labelStatus.text = "Victory";
        }

        ShowPanelComplete();
    }

    private void Defeat()
    {
        if (Language.rus)
        {
            labelStatus.text = "Поражение";
        }
        else
        {
            labelStatus.text = "Defeat";
        }

        ShowPanelComplete();
    }

    private void ShowPanelComplete()
    {
        StartCoroutine(Delay());

        IEnumerator Delay()
        {
            yield return new WaitForSecondsRealtime(1.7f);

            panelCompleted.SetActive(true);
            labelExp.text = $"{DataSaveLoad.Exp - originExpValue}";
            labelCountPunches.text = $"{player.CountInflictedPunch}";
            labelEarnedCoins.text = $"{player.CountInflictedPunch}";
        }
    }

    void LoadMenu()
    {
        //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
        if (countGame >= 3)
        {
            Advertising.ShowAds();
            countGame = 0;
        }
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++        

        if (MultiplayerManager.IsOffline)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        else
        {
            Photon.Pun.PhotonNetwork.LeaveRoom(false);
        }
    }

    void Update()
    {
        if (player)
        {
            if (joystick.Direction != Vector2.zero)
            {
                if (joystick.Horizontal > 0)
                {
                    player.StartWalk(1);
                }
                if (joystick.Horizontal < 0)
                {
                    player.StartWalk(-1);
                }
            }
            else
            {
                player.StopWalk();
            }
        }
        
    }

    void CheckPunchLocks()
    {
        if(DataSaveLoad.Level >= 3)
        {
            lockLeftArm.SetActive(false);
        }

        if (DataSaveLoad.Level >= 5)
        {
            lockRightLeg.SetActive(false);
        }

        if (DataSaveLoad.Level >= 8)
        {
            lockLeftLeg.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (player)
        {
            player.onDefeat -= Defeat;
        }
        if (enemy)
        {
            enemy.onVictory -= Victory;
        }

        GlobalEvents.OnMinePlayerSpawned -= MinePlayer_Spawned;
        GlobalEvents.OnAIPlayerSpawned -= AIPlayer_Spawned;
        GlobalEvents.OnNotMinePlayerSpawned -= NotMinePlayer_Spawned;
    }
}
