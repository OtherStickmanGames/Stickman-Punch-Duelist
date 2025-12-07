using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSaveLoad : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    public static int Exp
    {
        get
        {
            if (PlayerPrefs.HasKey("Exp"))
            {
                return PlayerPrefs.GetInt("Exp");
            }
            else
            {
                return 0;
            }
        }

        set
        {
            PlayerPrefs.SetInt("Exp", value);
            PlayerPrefs.Save();
        }
    }

    public static int Coin
    {
        get
        {
            if (PlayerPrefs.HasKey("Coin"))
            {
                return PlayerPrefs.GetInt("Coin");
            }
            else
            {
                return 0;
            }
        }

        set
        {
            PlayerPrefs.SetInt("Coin", value);
            PlayerPrefs.Save();
        }
    }

    public static int Level
    {
        get
        {
            if (PlayerPrefs.HasKey("Level"))
            {
                return PlayerPrefs.GetInt("Level");
            }
            else
            {
                return 1;
            }
        }

        set
        {
            PlayerPrefs.SetInt("Level", value);
            PlayerPrefs.Save();
        }
    }

    public static string NickName
    {
        get
        {
            if (PlayerPrefs.HasKey("Name"))
            {
                return PlayerPrefs.GetString("Name");
            }
            else
            {
                if(Language.rus)
                    return "Игрочело_" + Random.Range(100, 1000);
                else 
                    return "Player_" + Random.Range(100, 1000);
            }
        }

        set
        {
            PlayerPrefs.SetString("Name", value);
            PlayerPrefs.Save();
        }
    }

    public static string ColorsUnlocked
    {
        get
        {
            if (PlayerPrefs.HasKey("ColorsUnlocked"))
            {
                return PlayerPrefs.GetString("ColorsUnlocked");
            }
            else
            {
                return "0&";
            }
        }

        set
        {
            PlayerPrefs.SetString("ColorsUnlocked", value);
            PlayerPrefs.Save();
        }
    }

    public static string ColorsUsed
    {
        get
        {
            if (PlayerPrefs.HasKey("ColorsUsed"))
            {
                return PlayerPrefs.GetString("ColorsUsed");
            }
            else
            {
                return "0&0&0&0&0&0&0&0&0&0&0&";
            }
        }

        set
        {
            PlayerPrefs.SetString("ColorsUsed", value);
            PlayerPrefs.Save();
        }
    }
}
