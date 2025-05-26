using UnityEngine;

public interface IChattable 
{

    public void InitiateDialogue();

    public void SendDialogueLine(Vector2 customPosition, string dialogueID);
}
