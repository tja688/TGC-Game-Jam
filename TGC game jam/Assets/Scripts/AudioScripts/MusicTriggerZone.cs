using System;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class MusicTriggerZone : MonoBehaviour
{
    [Header("音乐设置")]
    public List<SoundEffect> musicList = new List<SoundEffect>();
    public int defaultMusicIndex = 0;

    private SoundEffect currentMusic;
    private int currentMusicIndex;
    
    public string playerTag = "Player";

    private BoxCollider2D zoneCollider;

    private void Awake()
    {
        zoneCollider = GetComponent<BoxCollider2D>();
        zoneCollider.isTrigger = true; 

        if (musicList.Count <= 0)
        {
            Debug.LogWarning($"MusicTriggerZone '{gameObject.name}' has no music in musicList.", this);
            return;
        }
        currentMusicIndex = Mathf.Clamp(defaultMusicIndex, 0, musicList.Count - 1);
        currentMusic = musicList[currentMusicIndex];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) 
        {
            Debug.Log($"Player entered zone: {gameObject.name}");
            PlayCurrentMusic();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) 
        {
            Debug.Log($"Player exited zone: {gameObject.name}");
            StopCurrentMusic();
        }
    }

    private void PlayCurrentMusic()
    {
        if (currentMusic == null)
        {
            Debug.LogWarning($"MusicTriggerZone '{gameObject.name}': currentMusic is null. Cannot play.", this);
            return;
        }

        AudioManager.Instance.Play(currentMusic);
    }
    
    private void StopCurrentMusic()
    {
        if (currentMusic == null)
        {
            return;
        }

        if (AudioManager.Instance != null)
        {

            AudioManager.Instance.Stop(currentMusic);
        }

    }

    
    public void SetNextMusicByIndex(int index)
    {
        if (musicList.Count == 0) return;

        index = Mathf.Clamp(index, 0, musicList.Count - 1);
        SoundEffect newMusic = musicList[index];

        bool playerIsCurrentlyInThisZone = IsPlayerStillInZone(); 

        if (currentMusic == newMusic && playerIsCurrentlyInThisZone) {
             return;
        }

        SoundEffect oldMusic = currentMusic;
        currentMusicIndex = index;
        currentMusic = newMusic;

        if (playerIsCurrentlyInThisZone)
        {
            if(oldMusic != null) AudioManager.Instance.Stop(oldMusic); 
            PlayCurrentMusic(); 
        }
    }


    private bool IsPlayerStillInZone()
    {

        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag); 
        if (playerObject != null && zoneCollider.bounds.Intersects(playerObject.GetComponent<Collider2D>().bounds)) {
            return true;
        }
        return false; 
    }


    public void SetNextMusic(SoundEffect music)
    {
        if (!music) return;

        var index = musicList.IndexOf(music);
        if (index >= 0)
        {
            SetNextMusicByIndex(index);
        }
        else
        {
            Debug.LogWarning($"MusicTriggerZone '{gameObject.name}': Specified music '{music.name}' is not in the predefined list, but will be set as current.", this);

            bool playerIsCurrentlyInThisZone = IsPlayerStillInZone();
            SoundEffect oldMusic = currentMusic;
            currentMusic = music; 
            currentMusicIndex = -1; 

            if (playerIsCurrentlyInThisZone)
            {
                if(oldMusic != null) AudioManager.Instance.Stop(oldMusic);
                PlayCurrentMusic();
            }
        }
    }

    public int GetCurrentMusicIndex()
    {
        return currentMusicIndex;
    }

    public SoundEffect GetCurrentMusic()
    {
        return currentMusic;
    }

    public void ResetToDefaultMusic()
    {
        if (musicList.Count > 0)
        {
            SetNextMusicByIndex(defaultMusicIndex);
        }
    }
    
}