using System;
using System.Collections;
using UnityEngine;

public abstract class MenuTemplate : MonoBehaviour
{
    public Action OnButtonPress;
    public abstract void Draw();
    public Action<string> OnNavigation;
    public Action OnBack;
    
    protected void DoButtonAction(string levelName)
    {
        StartCoroutine("ButtonAction", levelName);
    }

    protected void DoNavigation(string uri)
    {
        if (OnButtonPress != null) OnButtonPress();
        if (OnNavigation != null) OnNavigation(uri);
    }

    protected void DoBack()
    {
        if (OnButtonPress != null) OnButtonPress();
        if (OnBack != null) OnBack();
    }

    IEnumerator ButtonAction(string levelName)
    {
        if (OnButtonPress != null) OnButtonPress();
        yield return new WaitForSeconds(0.35f);
        if (levelName != "quit")
            Application.LoadLevel(levelName);
        else
        {
            Application.Quit();
            Debug.Log("Application has quit!");
        }
    }

}

public class NavigationEventArgs : EventArgs
{
    public string Uri { get; set; }
}