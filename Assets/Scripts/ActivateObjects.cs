using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateObjects : MonoBehaviour
{
    public List<ActivateObjectItem> items;

    public void Activate(int index)
    {   
        bool isActive = false;
        for (int i = 0; i < items.Count; ++i)
        {   
            isActive = i < index;
            if (items[i].objects != null && items[i].objects.Count > 0)
            {
                foreach (var gb in items[i].objects)
                {
                    gb.SetActive(isActive);
                }
            }
        }
    }// method
}


[System.Serializable]
public class ActivateObjectItem
{
    public int orderNumber;
    public List<GameObject> objects;
}