using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class UIPanelColors : MonoBehaviour
{
    [SerializeField] ColorKey[] colorKeys;

    [SerializeField] BtnColor[] btnsColor;

    [SerializeField] TMPro.TMP_Text labelBtnUnlockColor;
    [SerializeField] Button btnUnlockColor;
    [SerializeField] ColoringStickmanHandler coloring;

    List<byte> unlockedColorsID;
    List<byte> selectedColorsID;

    ViewBone selectedBone;
    BtnColor btnAccordanceSelectedBone;

    private void Awake()
    {
        unlockedColorsID = new List<byte>();
        selectedColorsID = new List<byte>();

        Coloring.ColorKeys = colorKeys;
    }

    private void Start()
    {
        btnUnlockColor.onClick.AddListener(Unlock_Cliked);

        foreach (var btn in btnsColor)
        {
            btn.Lock();
            btn.onClick += Color_Clicked;
            btn.onUnlock += Color_Unlocked;
        }

        coloring.onBoneSelect += Bone_Selected;
        coloring.onColoring += Bone_Coloring;
        coloring.onColored += Bone_Colored;

        LoadColorsUnlocked();

        foreach (var colorID in unlockedColorsID)
        {
            var colorKey = colorKeys.ToList().Find(c => c.index == colorID);
            btnsColor[colorID].Unlock(colorKey.color, colorKey.index);
        }

        coloring.ActivateUsedColors();

        CheckCountCoins();

        SelectUsedColors();
    }

    private void CheckCountCoins()
    { 
        btnUnlockColor.interactable = DataSaveLoad.Coin >= 150;
    }

    private void Bone_Colored(byte id)
    {
        selectedColorsID.Add(id);
    }

    private void Bone_Coloring(ViewBone previousColorBone)
    {
        if (previousColorBone != null)
        {
            selectedColorsID.Remove(previousColorBone.ColorID);
        }
    }

    private void Bone_Selected(ViewBone viewBone)
    {
        selectedBone = viewBone;

        StopButtonSelectableAnimation();

        if (viewBone != null)
        {
            foreach (var btn in btnsColor)
            {
                if (btn.ID == viewBone.ColorID)
                {
                    btnAccordanceSelectedBone = btn;
                    btn.StartAnimationBySelectedBone();
                    break;
                }
            }
        }
        
    }

    private void Color_Unlocked(BtnColor obj)
    {
        SelectUsedColors();
    }

    private void Unlock_Cliked()
    {
        int fixedStep = btnsColor.Length * 2;
        
        if (unlockedColorsID.Count < colorKeys.Length)
        {
            int randomIndex = UnityEngine.Random.Range(0, colorKeys.Length);
            byte id = colorKeys[randomIndex].index;

            while (unlockedColorsID.Contains(id))
            {
                randomIndex = UnityEngine.Random.Range(0, colorKeys.Length);
                id = colorKeys[randomIndex].index;
            }

            unlockedColorsID.Add(id);
            DataSaveLoad.Coin -= 150;
            CheckCountCoins();
            SaveColors();

            StartCoroutine(Switcher());

            IEnumerator Switcher()
            {
                float delta = 0.001f;
                float curvel = 0;

                for (int i = 0; i < fixedStep + id + 1; i++)
                {
                    yield return new WaitForSeconds(delta);

                    int idx = i;
                    while (idx > btnsColor.Length - 1)
                    {
                        idx -= btnsColor.Length;
                    }

                    DeselectAll();
                    SelectUsedColors();
                    btnsColor[idx].Select();

                    delta = Mathf.SmoothDamp(delta, 0.1f, ref curvel, 0.38f);
                }

                btnsColor[id].Unlock(colorKeys[randomIndex].color, colorKeys[randomIndex].index);
            }
        }
        else
        {
            if (Language.rus)
            {
                labelBtnUnlockColor.text = "ћл€, сор€н, мы еще не добавили новые цвета";
            }
            else
            {
                labelBtnUnlockColor.text = "Sorry, We haven't added any new colors yet";
            }
        }
    }

    private void Color_Clicked(BtnColor btn)
    {
        coloring.ColoringBone(new ColorKey() { color = btn.Color, index = btn.ID });

        DeselectAll();
        SelectUsedColors();

        btn.Select();

        StopButtonSelectableAnimation();
    }

    void DeselectAll()
    {
        foreach (var item in btnsColor)
        {
            item.Deselect();
        }
    }

    void SelectUsedColors()
    {
        foreach (var idx in selectedColorsID)
        {
            btnsColor[idx].Select();
        }
    }

    void StopButtonSelectableAnimation()
    {
        if (btnAccordanceSelectedBone)
        {
            btnAccordanceSelectedBone.StopAnimationBySelectedBone();
            btnAccordanceSelectedBone = null;
        }
    }

    private void SaveColors()
    {
        string saveData = string.Empty;

        foreach (var colorID in unlockedColorsID)
        {
            saveData += $"{colorID}&";
        }

        DataSaveLoad.ColorsUnlocked = saveData;
    }


    private void LoadColorsUnlocked()
    {
        var loadedColors = DataSaveLoad.ColorsUnlocked;
        
        var colorsIDs = loadedColors.Split(new char[] { '&' }, System.StringSplitOptions.RemoveEmptyEntries);

        unlockedColorsID.Clear();

        foreach (var id in colorsIDs)
        {
            unlockedColorsID.Add(byte.Parse(id));
        }
    }

    
    //private void LoadColorsSelected()
    //{
    //    var loadedColors = DataSaveLoad.ColorsUsed;

    //    var colorsIDs = loadedColors.Split(new char[] { '&' }, System.StringSplitOptions.RemoveEmptyEntries);

    //    selectedColorsID.Clear();

    //    foreach (var id in colorsIDs)
    //    {
    //        selectedColorsID.Add(byte.Parse(id));
    //    }
    //}

}
