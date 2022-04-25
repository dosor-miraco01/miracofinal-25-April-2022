using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseActive : MonoBehaviour
{
    public void Action()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
