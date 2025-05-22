using UnityEngine;

public class InteractiveObject : MonoBehaviour, IInteractable
{
    [Header("交互提示 (Prompt)")]
    [Tooltip("交互提示UI的预制体")]
    public GameObject promptPrefab;

    [Tooltip("交互提示的锚点/生成位置。如果未设置，将使用此对象的Transform")]
    public Transform customPromptAnchor;
    
    private GameObject instantiatedPromptInstance;

    public Transform PromptAnchor => customPromptAnchor ? customPromptAnchor : transform;

    public void ShowInteractionPrompt(GameObject instigator)
    {
        if (!promptPrefab ) return;
        
        instantiatedPromptInstance = Instantiate(promptPrefab,PromptAnchor.position, PromptAnchor.rotation,transform);
        
    }

    public void HideInteractionPrompt()
    {
        if (!promptPrefab ) return;

        Destroy(instantiatedPromptInstance);
    }

    public virtual void Interact(GameObject instigator)
    {
        Debug.Log("交互成功！");
    }

    private void OnDisable()
    {
        HideInteractionPrompt();
    }

    private void OnDestroy()
    {
        HideInteractionPrompt();
    }
}