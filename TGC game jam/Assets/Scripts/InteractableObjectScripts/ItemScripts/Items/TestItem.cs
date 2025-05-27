using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItem : ItemBase
{
    
    public static Transform ShowPointTrans {get;set;}

    private void Awake()
    {
        ShowPointTrans =  promptAnchorTransform;
    }
}
