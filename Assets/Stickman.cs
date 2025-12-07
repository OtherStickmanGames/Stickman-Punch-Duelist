using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class Stickman : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform hip;
    [SerializeField] GameObject[] limbs;
    [SerializeField] StickmanController ragdoll;
    [SerializeField] ViewBone[] viewsBones;
    
    [Space]

    [SerializeField] Transform limbArmRight;
    [SerializeField] Rigidbody2D bodyArmRightLow;

    [Space]

    [SerializeField] Transform limbArmLeft;
    [SerializeField] Rigidbody2D bodyArmLeftLow;

    [Space]

    [SerializeField] Transform limbLegRight;
    [SerializeField] Rigidbody2D bodyLegRightLow;

    [Space]

    [SerializeField] Transform limbLegLeft;
    [SerializeField] Rigidbody2D bodyLegLeftLow;

    [Space]

    [SerializeField] TMPro.TextMeshPro hpLabel;


    Vector3 originPosLimbLegRight;
    Vector3 originPosLimbArmRight;
    Vector3 originPosLimbArmLeft;
    Vector3 originPosLimbLegLeft;

    public bool IsPlayer { get; set; }
    public int CountInflictedPunch { get; private set; }

    int hp = 1000;
    int forceLeg = 900;
    int direction;
   
    public int HP
    {
        get => hp;
        set
        {
            hp = Mathf.Clamp(value, 0, int.MaxValue);

            if(hp == 0)
            {
                if (IsPlayer)
                {
                    onDefeat?.Invoke();
                }
                else
                {
                    onVictory?.Invoke();
                }

                ragdoll.SetMusclesPower(0);
            }
        }
    }

    public int Direction 
    { 
        get => direction;
        set 
        {
            direction = value;
            transform.GetChild(0).localScale = new Vector3(value, 1, 1);
            transform.GetChild(1).localScale = new Vector3(value, 1, 1);
        }
    }
    public bool IsMove { get; set; }
    public string NickName { get; set; }

    public ViewBone[] ViewsBones => viewsBones;

    public Transform Hip => hip;

    public Action onDefeat;
    public Action onVictory;
    public Action<int> OnDirChange;
    public Action<PunchPart> OnPunch;
    public Action<int> OnWalk;
    public Action<int> OnDamage;


    void Start()
    {
        originPosLimbArmRight = limbArmRight.localPosition;
        originPosLimbLegRight = limbLegRight.localPosition;
        originPosLimbArmLeft = limbArmLeft.localPosition;
        originPosLimbLegLeft = limbLegLeft.localPosition;

        SetActiveLimbs(false);

        IsPlayer = GetComponent<PlayerInput>() != null;
        //TODO
        if (GetComponent<PlayerInput>() && !MultiplayerManager.IsOffline) 
        {
            if (GetComponent<PlayerNetworkHandler>())
            {
                IsPlayer = GetComponent<PlayerNetworkHandler>().photonView.IsMine;
            }
        }

        Direction = (int)transform.GetChild(0).localScale.x;

        gameObject.AddComponent<StartVelocityFitter>();
    }

    
    void Update()
    {
        hpLabel.text = $"{NickName}\n{hp}";
        var labelPos = new Vector3(hip.position.x, hip.position.y + 2.58f, 0);

        hpLabel.transform.position = Vector3.MoveTowards(hpLabel.transform.position, labelPos, Time.deltaTime * 10);
    }

    public void Damage(int value)
    {
        OnDamage?.Invoke(value);
    }

    public void TakenExp(int value)
    {
        DataSaveLoad.Exp += value;
    }

    public void AddInflictedPunch()
    {
        CountInflictedPunch++;
        DataSaveLoad.Coin++;
    }

    public void StartWalk(int dir)
    {
        if(dir != Direction)
        {
            Direction = dir;
            OnDirChange?.Invoke(dir);
        }

        if (!IsMove)
        {
            animator.SetBool("Walk", true);
            OnWalk?.Invoke(dir);
            IsMove = true;
        }

    }

    public void StopWalk()
    {
        if (IsMove)
        {
            animator.SetBool("Walk", false);
            IsMove = false;
            OnWalk?.Invoke(0);
        }
    }

    IEnumerator PosBack(Transform limb, Vector3 originPos)
    {
        yield return new WaitForSeconds(0.3f);

        for (float f = 0; f < 1; f += 0.1f)
        {
            limb.localPosition = Vector3.Lerp(limb.localPosition, originPos, f);
            yield return null;
        }

        limb.localPosition = originPos;

        yield return null;

        SetActiveLimbs(false);
    }

    public void Punch(PunchPart part)
    {
        Vector3 pos;
        var forceDir = transform.GetChild(0).lossyScale.x;

        SetActiveLimbs(true);

        OnPunch?.Invoke(part);

        switch (part)
        {
            case PunchPart.RightArm:
                pos = hip.position + new Vector3(5 * forceDir, 1.8f, 0);
                limbArmRight.position = pos;

                bodyArmRightLow.AddForce(new Vector2(forceDir, 0.58f) * 500, ForceMode2D.Impulse);

                StartCoroutine(PosBack(limbArmRight, originPosLimbArmRight));
                break;
            case PunchPart.RightLeg:
                pos = hip.position + new Vector3(5 * forceDir, 3f, 0);
                limbLegRight.position = pos;

                bodyLegRightLow.AddForce(new Vector2(forceDir, 0f) * forceLeg, ForceMode2D.Impulse);

                StartCoroutine(PosBack(limbLegRight, originPosLimbLegRight));
                break;
            case PunchPart.LeftArm:
                pos = hip.position + new Vector3(5 * forceDir, 1.8f, 0);
                limbArmLeft.position = pos;

                bodyArmLeftLow.AddForce(new Vector2(forceDir, 0.58f) * 500, ForceMode2D.Impulse);

                StartCoroutine(PosBack(limbArmLeft, originPosLimbArmLeft));
                break;
            case PunchPart.LeftLeg:
                pos = hip.position + new Vector3(5 * forceDir, 3f, 0);
                limbLegLeft.position = pos;

                bodyLegLeftLow.AddForce(new Vector2(forceDir, 0f) * forceLeg, ForceMode2D.Impulse);

                StartCoroutine(PosBack(limbLegLeft, originPosLimbLegLeft));
                break;

            default:
                break;
        }
    }
    
    void SetActiveLimbs(bool active)
    {
        foreach (var item in limbs)
        {
            item.SetActive(active);
        }
    }
}


