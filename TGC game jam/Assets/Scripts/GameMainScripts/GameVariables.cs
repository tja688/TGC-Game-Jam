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


    private void FixedUpdate()
    {
        if (lastKnownDay != Day)
        {
            lastKnownDay =  Day;
            OnDayChanged?.Invoke();
        }

        if (Day1LetterSend == 3)
        {
            OnDay1FinishSend?.Invoke();
            Day1Finish = true;
        }
        
    }
}