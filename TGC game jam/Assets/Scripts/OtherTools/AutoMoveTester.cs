using UnityEngine;
using Cysharp.Threading.Tasks;

public class AutoMoveTester : MonoBehaviour
{
    public Transform targetPoint; // 在Inspector中拖拽一个目标点

    void Update()
    {
        // 按下 'T' 键来触发自动移动
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (PlayerMove.CurrentPlayer != null && targetPoint != null)
            {
                var playerMoveScript = PlayerMove.CurrentPlayer.GetComponent<PlayerMove>();
                if (playerMoveScript != null)
                {
                    Debug.Log("开始自动移动到: " + targetPoint.position);
                    // 异步调用，不需要等待它完成（fire and forget）
                    playerMoveScript.AutoMoveToPosition(targetPoint.position).Forget();
                }
            }
        }
    }
}