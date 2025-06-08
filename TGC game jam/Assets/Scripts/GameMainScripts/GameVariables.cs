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
    }

    private void Update()
    {
        // 按1增加Day1完成Day1任务
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Day1LetterSend = 3;
            Debug.Log("Day1LetterSend : " + Day1LetterSend);
        }
    }

    private void FixedUpdate()
    {
        if (lastKnownDay != Day)
        {
            lastKnownDay =  Day;
            OnDayChanged?.Invoke();
        }

        if (Day1LetterSend == 3)
        {
            if (Day1Finish) return;
            QuestTipManager.Instance.CompleteTask("SendLetterDay1");
            OnDay1FinishSend?.Invoke();
            Day1Finish = true;
        }
        
        if (Day2EventCount == 4)
        {
            if (Day2Finish) return;
            QuestTipManager.Instance.CompleteTask("SendLetterDay2");

            OnDay1FinishSend?.Invoke();
            Day2Finish = true;
        }
    }
}