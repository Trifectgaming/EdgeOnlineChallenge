using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {
    public UIInput input;
    public MainMenuBtn storyButton;
    public MainMenuBtn endlessButton;
    public UIButtonHandler controlsButton;
    public UIButtonHandler creditsButton;
    public TutorialContinMenu TutorialContinMenu;
    public CreditMenu CreditMenu;
    private string key = "PlayerName";

    void Awake()
    {
        storyButton.Click += (sender, args) =>
                                 {
                                     SavePlayerName();
                                     GameManager.IsEndless = false;
                                 };
        endlessButton.Click += (sender, args) =>
                                 {
                                     SavePlayerName();
                                     GameManager.IsEndless = true;
                                 };
        controlsButton.Click += (sender, args) =>
        {
            TutorialContinMenu.Show(Tutorial.Controls);
        };
        creditsButton.Click += (sender, args) =>
        {
            CreditMenu.Show();
        };
    }

    private void SavePlayerName()
    {
        GameManager.PlayerName = input.text;
        PlayerPrefs.SetString(key, GameManager.PlayerName);
    }

    // Use this for initialization
	void Start ()
	{
	    if (PlayerPrefs.HasKey(key))
	        input.text = PlayerPrefs.GetString(key);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
