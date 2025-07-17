using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using System;
using PixelCrushers.DialogueSystem;

public class GameVariables : MonoBehaviour
{
    [Tooltip("输入您想要在 Dialogue System 中监听的变量的准确名称")]
    [SerializeField] private string variableToWatch = "Day"; 

    
    public static int Day;
    public static bool DebugNoOpener;
    public static bool Day1Findletter = false;
    public static Transform StartAndFindCameraPosition;
    public static Transform FindLoraPosition;
    public static int Day1LetterSend = 0;
    public static bool Day1Finish = false;
    public static int Day2EventCount = 0;
    public static bool Day2HasTalkToGrandma = false;
    public static bool Day2Finish = false;
    public static int Day3EventCount = 0;
    public static bool Day3Finish = false;
    public static Transform BatteryPos;
    public static bool IsHaveBattery;
    public static int Day4EventCount = 0;
    public static bool Day4Finish = false;
    public static int Day5EventCount = 0;
    public static Transform LetterSender;
    public static Transform BoluNewPosition;
    public static Transform GirlPosition;
    public static bool Day5Finish = false;
    public static bool CanPickPaper = false;

    
    public static Transform BoluSetInStore;
    public static Transform GrandmaInSecondFloor;
    public static Transform Restaurant;

    public static event Action OnDayChanged;
    
    // 用于轮询的私有变量
    private int lastKnownDay;
    
    // Inspector可编辑的中间变量
    [SerializeField] private int editorDay;
    [SerializeField] private bool editorDebugNoOpener;
    [SerializeField] private Transform startAndFindCameraPosition;
    [SerializeField] private Transform findLoraPosition;
    [SerializeField] private Transform boluSetInStore;
    [SerializeField] private Transform grandmaInSecondFloor;
    [SerializeField] private Transform restaurant;
    [SerializeField] private Transform batteryPos;
    [SerializeField] private Transform letterSender;
    [SerializeField] private Transform boluNewPosition;
    [SerializeField] private Transform girlPosition;

    private void Awake()
    {
        Day = editorDay;
        DebugNoOpener = editorDebugNoOpener;
        StartAndFindCameraPosition = startAndFindCameraPosition;
        FindLoraPosition = findLoraPosition;
        lastKnownDay = Day;
        BoluSetInStore = boluSetInStore;
        GrandmaInSecondFloor = grandmaInSecondFloor;
        Restaurant = restaurant;
        BatteryPos = batteryPos;
        LetterSender = letterSender;
        BoluNewPosition = boluNewPosition;
        GirlPosition = girlPosition;
    }
    
    
    private void FixedUpdate()
    {
        Day = DialogueLua.GetVariable(variableToWatch).AsInt;
        
        // 检查天数变化
        if (lastKnownDay != Day)
        {
            lastKnownDay = Day;
            OnDayChanged?.Invoke();
        }
    }
}