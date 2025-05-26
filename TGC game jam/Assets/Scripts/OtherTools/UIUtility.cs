using UnityEngine;

public static class UIUtility
{

    public static Vector2 WorldToUILocalPoint(Vector3 worldPosition, RectTransform canvasRectTransform, Camera uiCamera, RectTransform targetRectTransform)
    {
        var screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRectTransform, screenPoint, uiCamera, out var localPoint);
        return localPoint;
    }

    public static Vector2 WorldToScreenSpaceOverlayPosition(Vector3 worldPosition)
    {

        var screenPoint = Camera.main.WorldToScreenPoint(worldPosition);


        return new Vector2(screenPoint.x, screenPoint.y);
    }
}