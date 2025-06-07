using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageStoreEmail : ItemBase
{
    public override void Interact(GameObject player)
    {
        switch( GameVariables.Day)
        {
            case 1:
                PlayerDialogue.Instance.Day1ImagingStoreEmail();
                break;
            case 2:
            case 3:
            case 4:
            case 5:
                break;
        }
        
    }
}
