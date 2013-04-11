using System;
using UnityEngine;

public class UIButtonHandler : MonoBehaviour
{
    public event EventHandler<ClickEventArgs> Click;
    public bool IsEnabled
    {
        get { return gameObject.GetComponent<UIButton>().isEnabled; }
        set { gameObject.GetComponent<UIButton>().isEnabled = value; }
    }

    protected virtual void RaiseClick(ClickEventArgs e)
    {
        EventHandler<ClickEventArgs> handler = Click;
        if (handler != null) handler(this, e);
    }

    // Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnClick()
    {
        RaiseClick(new ClickEventArgs(this, EventArgs.Empty));
    }
}

public class ClickEventArgs : EventArgs
{
    public MonoBehaviour Sender { get; set; }
    public EventArgs Args { get; set; }

    public ClickEventArgs(MonoBehaviour sender, EventArgs args)
    {
        Sender = sender;
        Args = args;
    }
}
