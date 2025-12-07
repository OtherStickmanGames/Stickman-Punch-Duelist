using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BtnColor : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] GameObject outline;
    [SerializeField] GameObject iconLock;
    [SerializeField] Button button;
    [SerializeField] Sprite unlock;

    public byte ID { get; set; }
    public Color Color { get; set; }

    public System.Action<BtnColor> onClick;
    public System.Action<BtnColor> onUnlock;

    bool outlineAnimating;

    private void Start()
    {
        button.onClick.AddListener(() => onClick?.Invoke(this));
    }

    public void Lock()
    {
        iconLock.SetActive(true);
        Deselect();
    }

    public void Unlock(Color color, byte index)
    {
        Color = color;
        ID = index;

        // Тряска замочка
        ShakeLock(0.5f);
        ShakeLock(0.75f);
        ShakeLock(1.0f);
        // Замена спрайта замочка
        LeanTween.delayedCall(1.3f, SwapLockSprite);
        // Исчезание замочка
        LeanTween.value
        (
            gameObject,
            (c) => 
            { 
                iconLock.GetComponent<Image>().color = c;
                outline.GetComponent<Image>().color = c;
            },
            Color.white,
            new Color(1, 1, 1, 0),
            0.3f
        ).setDelay(1.8f).setOnComplete(() => 
        { 
            iconLock.SetActive(false); 
            Deselect();
            outline.GetComponent<Image>().color = Color.white;

            onUnlock?.Invoke(this);
        });

        // Анимация отображения цвета
        LeanTween.value
        (
            gameObject,
            (c) => image.color = c,
            image.color,
            color,
            0.3f
        );
    }

    public void Select()
    {
        outline.SetActive(true);
    }

    public void Deselect()
    {
        outline.SetActive(false);
    }

    private void ShakeLock(float delay)
    {
        var locpos = iconLock.GetComponent<RectTransform>().localPosition;

        LeanTween.value
        (
            gameObject,
            (t) =>
            {
                var shakeValue = LeanTween.shake.Evaluate(t) * 5;
                var pos = locpos + new Vector3(shakeValue, shakeValue, 0);
                iconLock.GetComponent<RectTransform>().localPosition = pos;
            },
            0,
            1f,
            0.25f
        ).setDelay(delay);
    }

    private void SwapLockSprite()
    {
        try
        {
            iconLock.GetComponent<Image>().sprite = unlock;
            iconLock.GetComponent<RectTransform>().sizeDelta *= 1.3f;
        }
        catch (System.Exception)
        {

        }
    }

    public void StartAnimationBySelectedBone()
    {
        outlineAnimating = true;

        Anim();

        void Anim()
        {
            LeanTween.value
            (
                gameObject,
                (c) =>
                {
                    if (outlineAnimating)
                        outline.GetComponent<Image>().color = c;
                },
                Color.white,
                new Color(1, 1, 1, 0),
                0.3f
            ).setOnComplete(() =>
            {
                LeanTween.value
                (
                    gameObject,
                    (c) =>
                    {
                        if(outlineAnimating)
                            outline.GetComponent<Image>().color = c;
                    },
                    new Color(1, 1, 1, 0),
                    Color.white,
                    0.3f
                ).setOnComplete(()=> 
                {
                    if (outlineAnimating) Anim();
                    else outline.GetComponent<Image>().color = Color.white;
                });
            });
        }
    }

    public void StopAnimationBySelectedBone()
    {
        outlineAnimating = false;
    }
}
