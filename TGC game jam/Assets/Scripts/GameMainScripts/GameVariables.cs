using UnityEngine;
using UnityEngine.Serialization;

public class GameVariables : MonoBehaviour
{
    public static int Day;
    public static bool DebugNoOpener;

    // Inspector可编辑的中间变量
    [SerializeField] private int editorDay;
    [SerializeField] private bool editorDebugNoOpener;

    private void Awake()
    {
        Day = editorDay;
        DebugNoOpener = editorDebugNoOpener;
    }
}