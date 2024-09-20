using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeResource : MonoBehaviour
{
    public int woodAmount = 100; 
    public int woodPerChop = 1;  

    public void Chop()
    {
        woodAmount -= woodPerChop;
        if (woodAmount <= 0)
        {
            Destroy(gameObject);
        }
    }
}
