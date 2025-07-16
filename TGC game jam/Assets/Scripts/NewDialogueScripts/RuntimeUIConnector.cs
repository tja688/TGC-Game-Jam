using UnityEngine;
using UnityEngine.UI; // Required for Button
using PixelCrushers.DialogueSystem; // Required for Dialogue System classes

// This script ensures it's on the same GameObject as StandardDialogueUI
[RequireComponent(typeof(StandardDialogueUI))]
public class RuntimeUIConnector : MonoBehaviour
{
    private void Start()
    {
        // On start, find the global button and assign it to all relevant subtitle panels.
        AssignContinueButton();
    }

    private void AssignContinueButton()
    {
        // 1. Check if our central manager exists in the scene.
        if (SceneReferenceManager.Instance == null)
        {
            Debug.LogError("SceneReferenceManager not found! Can't assign the continue button.", this);
            return;
        }

        // 2. Get the global continue button from our manager.
        Button continueButton = SceneReferenceManager.Instance.globalContinueButton;

        if (continueButton == null)
        {
            Debug.LogWarning("The globalContinueButton is not set in the SceneReferenceManager!", this);
            return;
        }

        // 3. Get the StandardDialogueUI component on this GameObject.
        StandardDialogueUI dialogueUI = GetComponent<StandardDialogueUI>();

        // 4. Access its conversation controls, which hold the panel arrays.
        StandardUIDialogueControls dialogueControls = dialogueUI.conversationUIElements;

        // 5. THE FIX: Iterate through all assigned subtitle panels and set their continueButton.
        if (dialogueControls.subtitlePanels != null)
        {
            foreach (var panel in dialogueControls.subtitlePanels)
            {
                if (panel != null)
                {
                    panel.continueButton = continueButton;
                }
            }
        }

        // 6. As a safeguard, also set the default panels.
        if (dialogueControls.defaultNPCSubtitlePanel != null)
        {
            dialogueControls.defaultNPCSubtitlePanel.continueButton = continueButton;
        }
        if (dialogueControls.defaultPCSubtitlePanel != null)
        {
            dialogueControls.defaultPCSubtitlePanel.continueButton = continueButton;
        }
        
        Debug.Log($"RuntimeUIConnector: Successfully assigned '{continueButton.name}' to all subtitle panels.", this);
    }
}