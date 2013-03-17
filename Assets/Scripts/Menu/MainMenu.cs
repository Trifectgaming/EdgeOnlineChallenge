using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    private void Awake()
    {
    }
    
    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {

    }

    public void Show()
    {
        Debug.Log("PauseMenu.Show");
        enabled = true;
    }
}
