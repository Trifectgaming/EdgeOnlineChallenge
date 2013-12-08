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
        MaxX = ((float)Screen.width/Screen.height*MaxY);
        MinX = -MaxX;
    }
}