using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

public class ColoringStickmanHandler : MonoBehaviour
{
    [SerializeField] Stickman stickman;

    ViewBone selectedBone;
    Color colorBone;

    public Action<ViewBone> onBoneSelect;
    public Action<ViewBone> onColoring;
    public Action<byte> onColored;


    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hits   = Physics2D.RaycastAll(origin, Vector2.zero);

            bool selected = false;

            foreach (var hit in hits)
            {
                foreach (var viewBone in stickman.ViewsBones)
                {
                    if(viewBone.bone == hit.collider.transform)
                    {
                        ResetAllColor();
                        selectedBone = viewBone;
                        colorBone = viewBone.view.color;
                        SelectedBoneAnimation();
                        selected = true;
                        onBoneSelect?.Invoke(viewBone);
                        break;
                    }
                }
            }

            if (!selected && !EventSystem.current.currentSelectedGameObject)
            {
                ResetAllColor();
                selectedBone = null;
                onBoneSelect?.Invoke(null);
            }
        }
    }

    public void ColoringBone(ColorKey key)
    {
        ResetAllColor();

        if (selectedBone != null)
        {
            onColoring?.Invoke(selectedBone);

            selectedBone.ColorID = key.index;
            selectedBone.view.color = key.color;
            selectedBone = null;

            onColored?.Invoke(key.index);
        }
        else
        {
            foreach (var viewBone in stickman.ViewsBones)
            {
                onColoring?.Invoke(viewBone);

                viewBone.ColorID = key.index;
                viewBone.view.color = key.color;

                onColored?.Invoke(key.index);
            }
        }

        stickman.GetComponent<Coloring>().SaveColorsUsed();
    }

    public void ActivateUsedColors()
    {
        foreach (var viewBone in stickman.ViewsBones)
        {
            onColored?.Invoke(viewBone.ColorID);
        }
    }

    private void SelectedBoneAnimation()
    {
        if (selectedBone != null)
        {
            var view = selectedBone.view;
            var startColor = colorBone;
            var finalColor = colorBone;

            startColor.a = 0.8f;
            finalColor.a = 0.5f;

            float time = 0.8f;

            Anim();

            void Anim()
            {
                LeanTween.value
                (
                    gameObject,
                    (c) =>
                    {
                        if (selectedBone != null && selectedBone.view == view)
                            view.color = c;
                    },
                    startColor,
                    finalColor,
                    time
                ).setOnComplete
                (
                    () => 
                    {
                        LeanTween.value
                        (
                            gameObject,
                            (c) =>
                            {
                                if (selectedBone != null && selectedBone.view == view)
                                    view.color = c;
                            },
                            finalColor,
                            startColor,
                            time
                        ).setDelay(0.1f).setOnComplete(() =>
                        {
                            if (selectedBone != null && selectedBone.view == view)
                            {
                                Anim();
                            }
                        });
                    }
                );
            }
        }
    }

    private void ResetAllColor()
    {
        foreach (var viewBone in stickman.ViewsBones)
        {
            var c = viewBone.view.color;
            c.a = 1;
            viewBone.view.color = c;
        }
    }
}
