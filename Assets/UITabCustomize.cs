using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UITabCustomize : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] GameObject outline;
    [SerializeField] TMP_Text title;
    [SerializeField] Button button;

    [Space]

    [SerializeField] Color colorDeselect;
    [SerializeField] Color colorSelect;

    public System.Action<UITabCustomize> onClick;

    private void Start()
    {
        button.onClick.AddListener(() => onClick?.Invoke(this));
    }

    public void Deselect()
    {
        var c = image.color;
        c.a = 0.3f;
        image.color = c;

        outline.SetActive(false);

        title.color = colorDeselect;
        title.fontStyle = FontStyles.Normal;
    }

    public void Select()
    {
        var c = image.color;
        c.a = 1f;
        image.color = c;

        outline.SetActive(true);

        title.color = colorSelect;
        title.fontStyle = FontStyles.Bold;
    }
}
