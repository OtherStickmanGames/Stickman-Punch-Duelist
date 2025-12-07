using UnityEngine.UI;
using UnityEngine;

public class WoMCrossAd : MonoBehaviour
{
    [SerializeField] Button btnAd;

    private void Start()
    {
        if (Application.systemLanguage == SystemLanguage.Russian)
        {
            btnAd.onClick.AddListener(WoM_Clicked);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void WoM_Clicked()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.OtherStickmanGames.NoobOnline");
    }
}
