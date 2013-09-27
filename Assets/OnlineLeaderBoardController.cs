using UnityEngine;
using System.Collections;

public class OnlineLeaderBoardController : MonoBehaviour {
    
    private UIButtonHandler _button;

    void Awake()
    {
        _button = GetComponent<UIButtonHandler>();
    }

    void Start()
    {
        
    }

	void OnEnable()
	{
	    _button.Click += Clicked;
	}

    private void Clicked(object sender, ClickEventArgs e)
    {
        OnlineLeaderBoardManager.Show(OnlineLeaderBoardManager.EndlessBoard);
    }

    void OnDisable () 
    {
        _button.Click -= Clicked;
	}

    void OnDestory()
    {
        _button.Click -= Clicked;        
    }
}
