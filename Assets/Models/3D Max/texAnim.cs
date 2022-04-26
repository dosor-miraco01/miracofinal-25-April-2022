using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class texAnim : MonoBehaviour
{

    public float speed;
    Renderer[] allobj;
public bool x=false;
public bool y=true;
   
	// Use this for initialization
	void Start ()
    {
        allobj = gameObject.GetComponentsInChildren<Renderer>();

    }
	
	// Update is called once per frame
	void Update ()
    {
        StartCoroutine(moveMaterial());

    }
    
    IEnumerator moveMaterial()
    {
if(x)
{
	y=false;
 	foreach (var item in allobj)
        {
            item.material.mainTextureOffset = new Vector2(speed * Time.time, 0);
        }
}
else
{
	x=false;
  	foreach (var item in allobj)
        {
            item.material.mainTextureOffset = new Vector2(0, speed * Time.time);
        }
}

     
        yield return 0;
    }
}
