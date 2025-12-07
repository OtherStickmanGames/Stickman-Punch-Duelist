using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthSortingFitter : MonoBehaviour
{
    [SerializeField] int depth;
    [SerializeField] Color32 color;
    [SerializeField] bool enableSorting;

    [SerializeField]
    private List<SpriteRenderer> spritesAnimated;

    private void Awake()
    {
        if (enableSorting)
        {
            spritesAnimated.ForEach(s => { s.sortingOrder = depth; s.color = color; });
        }
    }
}
