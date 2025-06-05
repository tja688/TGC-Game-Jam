using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class GameVariables : MonoBehaviour
{
    public static int Day;
    public static bool DebugNoOpener;
    public static bool Day1Findletter = false;
    public static Transform StartAndFindCameraPosition;
    
    // Inspector可编辑的中间变量
    [SerializeField] private int editorDay;
    [SerializeField] private bool editorDebugNoOpener;
    [SerializeField] private Transform startAndFindCameraPosition;


    private void Awake()
    {
        Day = editorDay;
        DebugNoOpener = editorDebugNoOpener;
        StartAndFindCameraPosition = startAndFindCameraPosition;
    }
}