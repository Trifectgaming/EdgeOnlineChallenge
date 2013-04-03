using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MainMenuBtn : MonoBehaviour
{
    public string levelToLoad;

    // Update is called once per frame
	void Update () {
	    
	}

    void OnMouseUp()
    {
        Application.LoadLevel(levelToLoad);
    }
}
