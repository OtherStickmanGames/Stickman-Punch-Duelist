using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class BtnPunch : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Sprite source;
    [SerializeField] int reuiredLevel;
    //[SerializeField] PunchPart punchPart;
    [SerializeField] GameObject locked;
    [SerializeField] Button btnLock;


    public Action<int> onLockClick;

    private void Start()
    {
        icon.sprite = source;

        btnLock.onClick.AddListener(() => onLockClick?.Invoke(reuiredLevel));

        if(DataSaveLoad.Level >= reuiredLevel)
        {
            locked.SetActive(false);
        }
    }
}
