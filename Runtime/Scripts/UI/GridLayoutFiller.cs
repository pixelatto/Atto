using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class GridLayoutFiller : MonoBehaviour
{
    public GridLayoutGroup gridLayout { get { if (gridLayout_ == null) { gridLayout_ = GetComponent<GridLayoutGroup>(); }; return gridLayout_; } }
    private GridLayoutGroup gridLayout_;

    public RectTransform rt { get { if (rt_ == null) { rt_ = GetComponent<RectTransform>(); }; return rt_; } }
    private RectTransform rt_;

    void Update()
    {
        var height = rt.rect.height;
        var width = rt.rect.width;
        int horizontalCount = gridLayout.constraintCount;
        int verticalCount = Mathf.CeilToInt(rt.childCount / gridLayout.constraintCount);

        gridLayout.cellSize = new Vector2(width / horizontalCount, height / verticalCount);
    }
}
