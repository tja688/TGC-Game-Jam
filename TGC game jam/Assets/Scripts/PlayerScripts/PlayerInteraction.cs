using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerInteraction : MonoBehaviour
{
    [Header("光标设置")]
    [Tooltip("默认状态的光标贴图")]
    [SerializeField] private Texture2D defaultCursor;
    [Tooltip("光标的有效点击点（Hotspot）")]
    [SerializeField] private Vector2 cursorHotspot = Vector2.zero;
    private void Awake()
    {
        ChangeCursor(defaultCursor);
    }
    
    private void ChangeCursor(Texture2D cursorTexture)
    {
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }
}