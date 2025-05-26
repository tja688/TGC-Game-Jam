using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItem : ItemBase
{
    
    public static GameObject testItem {get;set;}

    private void Awake()
    {
        testItem =  this.gameObject;
    }
}
