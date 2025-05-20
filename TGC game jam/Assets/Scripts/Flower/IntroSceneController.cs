using Flower;
using UnityEngine;

public class IntroSceneController : MonoBehaviour
{
    private FlowerSystem flowerSystem;

    private void Start()
    {
        flowerSystem = FlowerManager.Instance.CreateFlowerSystem("default", false);
        flowerSystem.SetupDialog();
        flowerSystem.ReadTextFromResource("Test");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            flowerSystem.Next();
        }
    }
}
