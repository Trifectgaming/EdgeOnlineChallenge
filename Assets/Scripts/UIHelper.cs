using UnityEngine;

public static class UIHelper
{
    public static float MaxY;
    public static float MinY;
    public static float MaxX;
    public static float MinX;

    static UIHelper()
    {
        MaxY = Camera.main.orthographicSize;
        MinY = -MaxY;
        MaxX = (Camera.main.GetScreenWidth()/Camera.main.GetScreenHeight()*MaxY);
        MinX = -MaxX;
    }
}