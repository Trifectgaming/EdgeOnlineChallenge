using System;
using UnityEngine;

namespace Assets.Scripts
{
    public static class GUIEx
    {
        public static IDisposable BeginGroup(Rect position)
        {
            GUI.BeginGroup(position);
            return new Disposable(GUI.EndGroup);
        }
    }
}