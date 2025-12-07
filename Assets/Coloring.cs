using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Coloring : MonoBehaviour
{
    Stickman stickman;

    List<byte> colorsUsed;

    public static ColorKey[] ColorKeys { get; set; }

    readonly string[] botColorsSet = new string[] 
    {
        "0&0&0&0&0&0&0&0&0&0&0&",
        "0&0&0&0&0&4&4&0&0&7&0&",
        "0&0&14&0&0&0&0&0&0&0&0&",
        "14&14&0&0&0&0&0&0&0&0&0&",
        "7&12&0&14&8&14&0&11&0&4&8&",
        "14&14&14&14&14&14&14&14&14&14&14&",
    };

    private void Awake()
    {
        colorsUsed = new List<byte>();

        stickman = GetComponent<Stickman>();

        LoadColorsSelected();

        for (int i = 0; i < colorsUsed.Count; i++)
        {
            var ck = ColorKeys.ToList().Find(ck => ck.index == colorsUsed[i]);

            stickman.ViewsBones[i].view.color = ck.color;
            stickman.ViewsBones[i].ColorID = ck.index;
        }
    }

    private void LoadColorsSelected()
    {
        var loadedColors = DataSaveLoad.ColorsUsed;

        if (stickman.GetComponent<AIInput>())
        {
            loadedColors = botColorsSet[Random.Range(0, botColorsSet.Length)];
        }

        var colorsIDs = loadedColors.Split(new char[] { '&' }, System.StringSplitOptions.RemoveEmptyEntries);

        colorsUsed.Clear();

        foreach (var id in colorsIDs)
        {
            colorsUsed.Add(byte.Parse(id));
        }
    }

    public void ApplyReceiveColors(byte[] colorsID)
    {
        for (int i = 0; i < colorsID.Length; i++)
        {
            var ck = ColorKeys.ToList().Find(ck => ck.index == colorsID[i]);

            stickman.ViewsBones[i].ColorID = colorsID[i];
            stickman.ViewsBones[i].view.color = ck.color;
        }
    }

    public void SaveColorsUsed()
    {
        string saveData = string.Empty;

        foreach (var viewBone in stickman.ViewsBones)
        {
            saveData += $"{viewBone.ColorID}&";
        }

        DataSaveLoad.ColorsUsed = saveData;

    }

}
