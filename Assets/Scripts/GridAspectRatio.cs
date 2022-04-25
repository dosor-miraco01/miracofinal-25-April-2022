using Doozy.Engine.UI;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class GridAspectRatio : MonoBehaviour
{
    private GridLayoutGroup grid;
    private RectTransform _rect;
    private RectTransform parentRect;
    private float columnTargetSize = 0;
    private float diff = 0;
    [ReadOnly]
    public int columnsCount;

    public RectTransform labelsParent;

    // Start is called before the first frame update
    void Start()
    {
        grid = GetComponent<GridLayoutGroup>();
        
        _rect = GetComponent<RectTransform>();
        parentRect = _rect.parent.GetComponent<RectTransform>();

        //columnTargetSize = _rect.rect.width;
        float aspectRatio = (float)Screen.width / 1920.0f;
        grid.cellSize = new Vector2(grid.cellSize.x * aspectRatio, grid.cellSize.y * aspectRatio);
        grid.padding = new RectOffset((int)((float)grid.padding.left * aspectRatio),
            (int)((float)grid.padding.right * aspectRatio), 
            (int)((float)grid.padding.top * aspectRatio), 
            (int)((float)grid.padding.bottom * aspectRatio));
        grid.spacing = new Vector2(grid.spacing.x * aspectRatio, grid.spacing.y * aspectRatio);
        columnTargetSize = grid.cellSize.x + grid.spacing.x;
        diff = _rect.rect.width - columnTargetSize;
    }

    private void Update()
    {
        if (_rect.childCount>0)
        {
            columnsCount = Mathf.CeilToInt(((_rect.childCount * (grid.cellSize.y + grid.spacing.y)) + grid.padding.top) / _rect.rect.height); 
        }
    }// method

    public void Shrink()
    {
        if (Mathf.CeilToInt(parentRect.rect.width/ (columnTargetSize + diff)) > 1)
        {
            //parentRect.SetWidth(parentRect.sizeDelta.x - columnTargetSize);
            parentRect.offsetMin = new Vector2(parentRect.offsetMin.x + columnTargetSize, parentRect.offsetMin.y);
            //parentRect.sizeDelta = new Vector2(parentRect.sizeDelta.x - origGridWidth, parentRect.sizeDelta.y);
            //parentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentRect.sizeDelta.x - columnTargetSize);
        }
        else
        {
            parentRect.GetComponent<Doozy.Engine.UI.UIView>().Hide();
        }
    }// method

    public void Expand()
    {
        //float maxWidth = columnsCount * columnTargetSize;
        //if (_rect.rect.width < maxWidth)
        if (Mathf.CeilToInt(parentRect.rect.width/ (columnTargetSize + diff)) < columnsCount)
        {
            parentRect.offsetMin = new Vector2(parentRect.offsetMin.x - columnTargetSize, parentRect.offsetMin.y);
            //            parentRect.SetWidth(parentRect.sizeDelta.x + columnTargetSize);
            //parentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentRect.sizeDelta.x + columnTargetSize);
        }

    }// method

    public void ShrinkToMin()
    {
        for (int i = 0; i < columnsCount-1; ++i)
        {
            Shrink();
        }
    }

    public void ShowLabel(RectTransform objTra)
    {
        int c = GetRow(objTra);
        var lbObj = labelsParent.GetChild(c - 1);
        lbObj.GetComponentInChildren<RTLTMPro.RTLTextMeshPro>().text = objTra.GetComponent<LearningObjectItemView>().Data.data.title;
        lbObj.GetComponent<UIView>().Show();
    }// method

    int GetRow(RectTransform objTra)
    {
        int row = 1;
        for (int i = 0; i < _rect.childCount; ++i)
        {
            if (_rect.GetChild(i).GetComponent<RectTransform>().anchoredPosition.y != objTra.anchoredPosition.y)
            {
                ++row;
                continue;
            }
            //Debug.Log("Row: " + row);
            break;
        }
        return row;
    }

    int GetColumn(RectTransform objTra)
    {
        int col = 1;
        float lastX = float.MaxValue;
        RectTransform cTra = null;
        for (int i = 0; i < _rect.childCount; ++i)
        {
            cTra = _rect.GetChild(i).GetComponent<RectTransform>();
            if (cTra.anchoredPosition.x == lastX) continue;
            if (cTra.anchoredPosition.x != objTra.anchoredPosition.x)
            {
                ++col;
                lastX = cTra.anchoredPosition.x;
                continue;
            }
            //Debug.Log("Row: " + row);
            break;
        }
        return col;
    }

    void GetColumnAndRow(GridLayoutGroup glg, RectTransform t, out int column, out int row)
    {
        column = 0;
        row = 0;

        if (glg.transform.childCount == 0)
            return;

        //Column and row are now 1
        column = 1;
        row = 1;

        //Get the first child GameObject of the GridLayoutGroup
        RectTransform firstChildObj = glg.transform.
            GetChild(0).GetComponent<RectTransform>();

        if (firstChildObj == t) return;
        Vector2 firstChildPos = firstChildObj.anchoredPosition;
        bool stopCountingRow = false;

        //Loop through the rest of the child object
        for (int i = 1; i < glg.transform.childCount; i++)
        {
            //Get the next child
            RectTransform currentChildObj = glg.transform.
           GetChild(i).GetComponent<RectTransform>();

            Vector2 currentChildPos = currentChildObj.anchoredPosition;

            //if first child.x == otherchild.x, it is a column, ele it's a row
            if (firstChildPos.x == currentChildPos.x)
            {
                column++;
                //Stop couting row once we find column
                stopCountingRow = true;
            }
            else
            {
                if (!stopCountingRow)
                    row++;
            }

            if (t == currentChildObj)
                break;
        }
    }

    public void HideLabel(RectTransform objTra)
    {
        int c = GetRow(objTra);
        var lbObj = labelsParent.GetChild(c - 1);
        lbObj.GetComponent<UIView>().Hide();
    }// method

    public void CheckIfSelectedItemNeedToExpand(RectTransform item)
    {
        int col = GetColumn(item);
        for (int i = 1; i < col; i++)
        {
            if (Mathf.CeilToInt(parentRect.rect.width / (columnTargetSize + diff)) < col)
            {
                parentRect.offsetMin = new Vector2(parentRect.offsetMin.x - columnTargetSize, parentRect.offsetMin.y);
                //            parentRect.SetWidth(parentRect.sizeDelta.x + columnTargetSize);
                //parentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentRect.sizeDelta.x + columnTargetSize);
            }
        }
       
    }// method
}
