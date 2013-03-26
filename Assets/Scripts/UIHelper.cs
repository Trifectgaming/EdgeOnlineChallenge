using UnityEngine;

public static class UIHelper
{
    public static float MaxY;
    public static float MinY;
    public static float MaxX;
    public static float MinX;

    static UIHelper()
    {
        MaxY = Camera.mainCamera.orthographicSize;
        MinY = -MaxY;
        MaxX = (Camera.mainCamera.GetScreenWidth()/Camera.mainCamera.GetScreenHeight()*MaxY);
        MinX = -MaxX;
    }
}