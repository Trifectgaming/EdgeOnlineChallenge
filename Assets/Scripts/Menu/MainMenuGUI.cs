using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MainMenuGUI : MonoBehaviour
{
    public AudioClip beep;
    public GUISkin menuSkin;
    public Rect menuArea;
    public string currentMenu;
    public MenuTemplate[] menus;
    private Rect menuAreaNormalized;
    private Dictionary<string, MenuTemplate> _menus;
    private Stack<string> _backStack;

    private void Start()
    {
        _backStack = new Stack<string>();
        _menus = new Dictionary<string, MenuTemplate>(menus.Length);
        if (menus != null)
            foreach (var template in menus.Where(m => m != null))
            {
                template.OnButtonPress = () => audio.PlayOneShot(beep);
                template.OnNavigation = s =>
                                            {
                                                _backStack.Push(currentMenu);
                                                currentMenu = s;
                                            };
                template.OnBack = () =>
                                      {
                                          currentMenu = _backStack.Pop();
                                      };
                _menus.Add(template.GetType().Name, template);
            }
        var left = menuArea.x*Screen.width - (menuArea.width*0.5f);
        var top = menuArea.y*Screen.height - (menuArea.height*0.5f);
        menuAreaNormalized = new Rect(
            left,
            top,
            menuArea.width,
            menuArea.height
            );
    }

    void OnGUI()
	{
	    GUI.skin = menuSkin;
	    using (GUIEx.BeginGroup(menuAreaNormalized))
	    {
	        if (currentMenu != null)
	        {
	            MenuTemplate template;
	            if (_menus.TryGetValue(currentMenu, out template))
                    template.Draw();
	            else
	            {
                    var menu = _menus.First();
                    currentMenu = menu.Key;
	                menu.Value.Draw();
	            }
	        }
	    }
	}

	// Update is called once per frame
	void Update () {
	
	}
}