using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LiftItem : ItemBase // 假设 ItemBase 是你已有的基类
{
    public Transform liftUpTransform;
    public Transform liftDownTransform;
    
    public Transform playerUpTransform;
    public Transform playerDownTransform;

    
    private bool isUp;
    
    private void OnEnable()
    {
        isUp = false;
    }

    public override void Interact(GameObject player)
    {
        ScreenFadeController.Instance.BeginFadeToBlack(0.5f);

        AudioManager.Instance.Play(grabSound);
        
        PlayerArrive();

    }

    private async void PlayerArrive()
    {
        await WaitForPlayerArrive();
    }

    private async UniTask WaitForPlayerArrive()
    {
        await UniTask.WaitForSeconds(0.5f);

        if (!isUp)
        {
            this.transform.position = liftUpTransform.position;
            PlayerMove.CurrentPlayer.transform.position = playerUpTransform.position;
            isUp =  true;
        }
        else
        {
            this.transform.position = liftDownTransform.position;
            PlayerMove.CurrentPlayer.transform.position = playerDownTransform.position;
            isUp =  false;
        }
        
        ScreenFadeController.Instance.BeginFadeToClear(0.5f);
        
    }
}