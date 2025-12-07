using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class TabsController : MonoBehaviour
{
    [SerializeField] UITabCustomize[] tabs;

    [Space]
    // TODO
    [SerializeField] GameObject panelItems;

    private void Start()
    {
        DeselectAll();

        tabs[0].Select();

        foreach (var tab in tabs)
        {
            tab.onClick += Tab_Clicked;
        }
    }

    private void Tab_Clicked(UITabCustomize tab)
    {
        DeselectAll();

        tab.Select();

        if(tab == tabs[1])
        {
            panelItems.SetActive(true);
        }
    }

    private void DeselectAll()
    {
        foreach (var tab in tabs)
        {
            tab.Deselect();
        }
    }
}
