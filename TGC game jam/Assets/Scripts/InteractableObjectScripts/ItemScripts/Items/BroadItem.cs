using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BroadItem : ItemBase
{
    public override void Interact(GameObject player)
    {
        PlayerDialogue.Instance.Roadsigns();
    }
}
