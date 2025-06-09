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
    public static event Action OnDay1FinishSend;

    
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

    private void Update()
    {
        // 按F1增加Day1完成Day1任务
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Day1LetterSend = 3;
        }
        
        // 按F2增加完成Day3任务
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Day = 3;
            OnDay1FinishSend?.Invoke(); 
            Day3Finish = true;
            IsHaveBattery = true;
        }
    }

    private void FixedUpdate()
    {
        // 检查天数变化
        if (lastKnownDay != Day)
        {
            lastKnownDay = Day;
            OnDayChanged?.Invoke();
        }

        // Day1 逻辑（独立）
        if (Day1LetterSend == 3 && !Day1Finish)
        {
            QuestTipManager.Instance.CompleteTask("SendLetterDay1");
            OnDay1FinishSend?.Invoke();
            Day1Finish = true;
        }
    
        // Day2 逻辑（独立）
        if (Day2EventCount == 4 && !Day2Finish)
        {
            QuestTipManager.Instance.CompleteTask("SendLetterDay2");
            OnDay1FinishSend?.Invoke(); // 注意：这里可能应该是 OnDay2FinishSend?
            Day2Finish = true;
        }
        
        // Day3 逻辑（独立）
        if (Day3EventCount == 4 && !Day3Finish)
        {
            QuestTipManager.Instance.CompleteTask("SendLetterDay3");
            OnDay1FinishSend?.Invoke(); 
            Day3Finish = true;
        }
        
        // Day3 逻辑（独立）
        if (Day4EventCount == 2 && !Day4Finish)
        {
            QuestTipManager.Instance.CompleteTask("SendLetterDay4");
            OnDay1FinishSend?.Invoke(); 
            Day4Finish = true;
        }
        
        if (Day5EventCount == 1 && !Day5Finish)
        {
            QuestTipManager.Instance.CompleteTask("SendLetterDay5");
            OnDay1FinishSend?.Invoke(); 
            Day5Finish = true;
        }

    }
}