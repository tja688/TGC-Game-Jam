using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour
{
    public SoundEffect streetMainMusic;

    private void Start()
    {
        if (!streetMainMusic)
            Debug.LogError("streetMainMusic is null");
        
        AudioManager.Instance.Play(streetMainMusic);
    }
    
}
