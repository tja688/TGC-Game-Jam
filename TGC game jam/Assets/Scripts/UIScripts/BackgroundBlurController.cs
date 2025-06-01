using UnityEngine;
using UnityEngine.Rendering.PostProcessing; 

public class BackgroundBlurController : MonoBehaviour
{
    public PostProcessVolume blurVolume; 

    private DepthOfField depthOfField; 
    
    private void Start()
    {
        if (blurVolume && blurVolume.profile.TryGetSettings(out depthOfField))
        {
            SetBlurActive(false);
        }
        else
        {
            Debug.LogError("模糊效果或Volume未正确配置!");
        }

        PlayerInterfaceOpenerButton.OpenPlayerPanelUI += OpenUI;
        PlayerInterfaceOpenerButton.ClosePlayerPanelUI += CloseUI;
        UIStartSettingShowHideButton.OpenBeginSettingUI += OpenUI;
        UIStartSettingShowHideButton.CloseBeginSettingUI += CloseUI;


    }

    private void CloseUI()
    {
        HideUIPanel();
    }

    private void OpenUI()
    {
        ShowUIPanel();
    }

    private void ShowUIPanel()
    {
        SetBlurActive(true);
    }

    private void HideUIPanel()
    {
        SetBlurActive(false);
    }

    private void SetBlurActive(bool isActive)
    {
        if (depthOfField)
        {
            depthOfField.active = isActive; 
        }
    }
}