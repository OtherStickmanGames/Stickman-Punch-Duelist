using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject mainGroup;
    [SerializeField] GameObject customizeGroup;

    [Space]

    [SerializeField] TextMeshProUGUI expLabel;
    [SerializeField] TMP_Text labelValueCoin;
    [SerializeField] GameObject panelLockInfo;
    [SerializeField] TextMeshProUGUI lockInfo;
    [SerializeField] GameObject panelImprove;
    [SerializeField] GameObject loading;
    [SerializeField] GameObject panelChangeNickName;
    [SerializeField] GameObject panelUpToDate;
    [SerializeField] Button btnUpdate;
    [SerializeField] Button btnCustomize;
    [SerializeField] Button btnBack;

    [SerializeField] BtnPunch[] btnsPunch;

    [Space]

    [SerializeField] Button btnDiscord;
    [SerializeField] Button btnPivacyPolicy;
    [SerializeField] TMP_Text textVersion;


    int expStep = 3000;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        if (DataSaveLoad.Exp >= expStep * DataSaveLoad.Level)
        {
            DataSaveLoad.Exp -= expStep * DataSaveLoad.Level;
            DataSaveLoad.Level++;
        }

        btnDiscord.onClick.AddListener(OpenDiscord);
        btnPivacyPolicy.onClick.AddListener(OpenPrivacyPolicy);
        btnUpdate.onClick.AddListener(OpenPlayMarket);
        btnCustomize.onClick.AddListener(Customize_Clicked);
        btnBack.onClick.AddListener(Back_Clicked);

        textVersion.text = $"Version {Application.version}";
        

        foreach (var btn in btnsPunch)
        {
            btn.onLockClick += Lock_Clicked;
        }

        panelLockInfo.SetActive(false);
        panelImprove.SetActive(false);
        loading.SetActive(false);
        panelChangeNickName.SetActive(false);
        panelUpToDate.SetActive(false);

        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;

        //StartCoroutine(Pisko());
    }

    private void Back_Clicked()
    {
        // Главная панель возвращается
        StartCoroutine(ChangeValue(1920, 0, MainAnim, 70f));

        void MainAnim(float x)
        {
            mainGroup.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0);
        }

        StartCoroutine(ChangeValue(0, 1, MainTransparent, 0.03f));

        void MainTransparent(float a)
        {
            mainGroup.GetComponent<CanvasGroup>().alpha = a;
        }

        // Камера улетает от стикмена
        StartCoroutine(ChangeValue(-12.5f, 0, CameraMove, 0.5f));

        void CameraMove(float x)
        {
            Camera.main.transform.position = new Vector3(x, 0, -10);
        }

        // Панель кастомизации пропадает
        StartCoroutine(ChangeValue(0, -1920, CustomizeMove, 70f));

        void CustomizeMove(float x)
        {
            customizeGroup.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0);
        }

        StartCoroutine(ChangeValue(1, 0, CustomizeTransparent, 0.03f));

        void CustomizeTransparent(float a)
        {
            customizeGroup.GetComponent<CanvasGroup>().alpha = a;
        }
    }

    private void Customize_Clicked()
    {
        // Главная панель уезжает
        StartCoroutine(ChangeValue(0, 1920, MainAnim, 70f));

        void MainAnim(float x)
        {
            mainGroup.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0);
        }

        StartCoroutine(ChangeValue(1, 0, MainTransparent, 0.03f));

        void MainTransparent(float a)
        {
            mainGroup.GetComponent<CanvasGroup>().alpha = a;
        }
        // Камера перемещается на стикмена
        StartCoroutine(ChangeValue(0, -12.5f, CameraMove, 0.5f));

        void CameraMove(float x)
        {
            Camera.main.transform.position = new Vector3(x, 0, -10);
        }

        // Панель кастомизации появляется
        StartCoroutine(ChangeValue(-1920, 0, CustomizeMove, 70f));

        void CustomizeMove(float x)
        {
            customizeGroup.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0);
        }

        StartCoroutine(ChangeValue(0, 1, CustomizeTransparent, 0.03f));

        void CustomizeTransparent(float a)
        {
            customizeGroup.GetComponent<CanvasGroup>().alpha = a;
        }
    }

    private void Start()
    {
        var rectCustomize = customizeGroup.GetComponent<RectTransform>();
        rectCustomize.anchoredPosition = new Vector2(-1920, 0);

        var rectMain = mainGroup.GetComponent<RectTransform>();
        rectMain.anchoredPosition = Vector2.zero;
    }

    private void OpenPlayMarket()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.OtherStickmanGames.StickmanPunch");
    }

    private void Lock_Clicked(int level)
    {
        panelLockInfo.SetActive(true);

        if (Language.rus)
        {
            lockInfo.text = $"Данная способность заблокирована," +
                $" чтобы её разблокировать вам необходимо достигнуть {level} уровня.";
        }
        else
        {
            lockInfo.text = $"This ability is blocked, to unlock it, you need to reach level {level}.";
        }
    }

    private void Update()
    {
        var level = DataSaveLoad.Level;

        if (!Language.rus)
        {
            expLabel.text = $"Level: {level} Exp: {DataSaveLoad.Exp}/{level * expStep}";
        }
        else
        {
            expLabel.text = $"Уровень: {level} Опыт: {DataSaveLoad.Exp}/{level * expStep}";
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            panelUpToDate.SetActive(false);
        }

        labelValueCoin.text = $"{DataSaveLoad.Coin}";

        if (Input.GetKeyDown(KeyCode.P))
        {
            DataSaveLoad.Coin += 1000;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            PlayerPrefs.DeleteKey("Colors");
        }
    }

    private void OpenDiscord()
    {
        Application.OpenURL("https://discord.gg/WVFxHg29sG");
    }

    private void OpenPrivacyPolicy()
    {
        Application.OpenURL("https://wogergames.github.io/StickmanPunch/");
    }

    private IEnumerator Pisko()
    {
        var request = UnityWebRequest.Get("https://play.google.com/store/apps/details?id=com.OtherStickmanGames.StickmanPunch");

        var op = request.SendWebRequest();

        yield return op;
                        
        var searchStr = "Yq7oyNIvAAkuc69fG51sbAQS4otJxbObbt3xdr8tXxXyUdq4tVGtfgeKuptveGdP1srxaHVrNPzOYcfaEQ\"]]],\"";
        var index = request.downloadHandler.text.IndexOf(searchStr);
        if (index > 0)
        {
            var text = request.downloadHandler.text.Substring(index + searchStr.Length);
            var version = text.Substring(0, text.IndexOf("\""));
            print($"Текущая версия {Application.version} === Найденная версия {version}");
            if (Application.version != version)
            {
                panelUpToDate.SetActive(true);
            }
        }
    }

    IEnumerator ChangeValue(float from, float to, System.Action<float> onComplete, float delta = 10f)
    {
        float value = from;
        while(!Mathf.Approximately(value, to))
        {
            yield return null;
            value = Mathf.MoveTowards(value, to, delta);
            onComplete?.Invoke(value);
        }
    }
}
