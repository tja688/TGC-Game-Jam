using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingDetection : MonoBehaviour
{
    public Transform chargingPosition;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查碰撞对象是否有"Player"标签
        if (other.CompareTag("Player"))
        {
            switch (GameVariables.Day)
            {
                case 1:
                    if (GameVariables.Day1Finish)
                    {
                        PlayerMove.CanPlayerMove = false;
                        
                        PlayerMove.CurrentPlayer.transform.position = chargingPosition.position;
                        
                        EventCenter.TriggerEvent(GameEvents.PlayerSleep);
                        
                    }
                    
                    break;
            }
            
        }
    }
}
