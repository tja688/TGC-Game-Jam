using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFadeController : MonoBehaviour
{
    public Image fadeImage;
    public float defaultFadeDuration = 1f;
    public static ScreenFadeController Instance { get; private set; }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        if (!fadeImage) Debug.LogError("ScreenFadeController: 'fadeImage' 未指定!");
        else fadeImage.gameObject.SetActive(true);
    }

    // 公共方法，返回一个可以被外部 yield return 的协程
    public Coroutine BeginFadeToBlack(float? duration = null)
    {
        return !fadeImage ? null : StartCoroutine(Fade(1f, duration ?? defaultFadeDuration));
    }

    public Coroutine BeginFadeToClear(float? duration = null)
    {
        return !fadeImage ? null : StartCoroutine(Fade(0f, duration ?? defaultFadeDuration));
    }

    // 核心渐变逻辑，现在不触发事件
    private IEnumerator Fade(float targetAlpha, float duration)
    {
        if (!fadeImage) yield break;

        var timer = 0f;
        var currentColor = fadeImage.color;
        var startAlpha = currentColor.a;

        if (duration <= 0f) {
        }
        else {
            while (timer < duration)
            {
                timer += Time.deltaTime;
                currentColor.a = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
                fadeImage.color = currentColor;
                yield return null;
            }
        }

        currentColor.a = targetAlpha;
        fadeImage.color = currentColor;
    }
}