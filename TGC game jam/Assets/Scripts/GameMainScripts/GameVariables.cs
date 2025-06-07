using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using System;

public class GameVariables : MonoBehaviour
{
    public static int Day;
    public static bool DebugNoOpener;
    public static bool Day1Findletter = false;
    public static Transform StartAndFindCameraPosition;
    public static Transform FindLoraPosition;

    public static event Action OnDayChanged;
    
    // 用于轮询的私有变量
    private int lastKnownDay;
    
    // Inspector可编辑的中间变量
    [SerializeField] private int editorDay;
    [SerializeField] private bool editorDebugNoOpener;
    [SerializeField] private Transform startAndFindCameraPosition;
    [SerializeField] private Transform findLoraPosition;


    private void Awake()
    {
        Day = editorDay;
        DebugNoOpener = editorDebugNoOpener;
        StartAndFindCameraPosition = startAndFindCameraPosition;
        FindLoraPosition = findLoraPosition;
        lastKnownDay = Day;
    }


    private void FixedUpdate()
    {
        if (lastKnownDay != Day)
        {
            lastKnownDay =  Day;
            OnDayChanged?.Invoke();
        }
        
    }
}