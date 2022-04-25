using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTransformBehaviour : MonoBehaviour
{
    Dictionary<string, List<SavedTransform>> _savedTrans = new Dictionary<string, List<SavedTransform>>();

    public void Save(string id)
    {
        _savedTrans[id] = new List<SavedTransform>();
        SavePPRescursive(transform, _savedTrans[id]);
    }// method

    private void SavePPRescursive(Transform tr, List<SavedTransform> list)
    {   
        list.Add(SavedTransform.Create(tr));
        for (int i = 0; i < tr.childCount; ++i)
            SavePPRescursive(tr.GetChild(i), list);
    }// method

    public void Restore(string id)
    {
        if (_savedTrans != null && _savedTrans.ContainsKey(id))
        {
            foreach (var nop in _savedTrans[id])
            {
                nop.Target.gameObject.SetActive(nop.isActive);
                
                nop.Target.position = nop.position;
                nop.Target.rotation = nop.rotation;
                nop.Target.localScale = nop.localScale;
            }
        }

    }// method
}
