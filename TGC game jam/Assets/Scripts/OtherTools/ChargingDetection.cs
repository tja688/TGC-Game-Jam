using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ChargingDetection : MonoBehaviour
{
    public Transform chargingPosition;
    
    // 添加一个状态锁，防止重复触发
    private bool isCharging = false;
    
    private async void OnTriggerEnter2D(Collider2D other)
    {
        // 如果正在处理充电逻辑，或者碰撞的不是玩家，则直接返回
        if (isCharging || !other.CompareTag("Player"))
        {
            return;
        }

        switch (GameVariables.Day)
        {
            case 1:
                if (GameVariables.Day1Finish)
                {
                    // 立刻锁上状态，防止其他触发进入
                    isCharging = true;
                    
                    PlayerMove.CanPlayerMove = false;
                    
                    if (PlayerMove.CurrentPlayer)
                    {
                        PlayerMove.CurrentPlayer.transform.position = chargingPosition.position;
                    }
                    
                    await WaitForCharging();
                    
                }
                
                break;
        }
    }

    private async UniTask WaitForCharging()
    {
        await UniTask.WaitForSeconds(1f);
        
        // 确保PlayerDialogue实例存在
        if (PlayerDialogue.Instance)
        {
            PlayerDialogue.Instance.Charging();
        }
        
        // 等待对话结束事件
        await GameFlow.WaitForEvent(
            h => DialogueManager.DialogueFinished += h,
            h => DialogueManager.DialogueFinished -= h
        );
        
        // 触发睡眠事件
        EventCenter.TriggerEvent(GameEvents.PlayerSleep);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isCharging = false;
        }
    }
}