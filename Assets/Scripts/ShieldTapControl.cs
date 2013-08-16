using System.Linq;
using UnityEngine;
using System.Collections;

public class ShieldTapControl : GameSceneObject
{
    public GUITexture SpinLeft;
    public GUITexture SpinRight;
    public int LastFinger = -1;

    private ShieldManager _shieldManager;

	// Use this for initialization
    protected override void Start ()
	{
        base.Start();
	    var drone = ((Drone) FindObjectOfType(typeof (Drone)));
	    _shieldManager = drone.shieldManager;
	}

    public bool HitTest(Vector3 position)
    {
        return SpinRight.HitTest(position) || SpinLeft.HitTest(position);
    }

    // Update is called once per frame
	void Update () {
        if (Input.touchCount == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (SpinLeft && SpinLeft.HitTest(Input.mousePosition))
                {
                    _shieldManager.SetCurrent(_shieldManager.GetNextShield());
                    RestoreAlpha(SpinLeft);
                    StartCoroutine(UnphaseSpinLeftButton());

                }
                else if (SpinRight && SpinRight.HitTest(Input.mousePosition))
                {
                    _shieldManager.SetCurrent(_shieldManager.GetPreviousShield());
                    RestoreAlpha(SpinRight);
                    StartCoroutine(UnphaseSpinRightButton());
                }
                else
                {
                    _shieldManager.SetCurrent(_shieldManager.GetNextShield());
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                _shieldManager.SetCurrent(_shieldManager.GetPreviousShield());
            }
        }
        else
        {
            var count = Input.touchCount;
            for (int i = 0; i < count; i++)
            {
                var touch = Input.touches[i];
                if (touch.fingerId == LastFinger)
                {
                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                        LastFinger = -1;
                    Debug.Log("Finger " + touch.fingerId + " was last finger.");
                    continue;
                }
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    continue;
                if (!HitTest(touch.position))
                    continue;
                if (SpinLeft && SpinLeft.HitTest(touch.position))
                {
                    _shieldManager.SetCurrent(_shieldManager.GetNextShield());
                    RestoreAlpha(SpinLeft);
                    StartCoroutine(UnphaseSpinLeftButton());
                }
                if (SpinRight && SpinRight.HitTest(touch.position))
                {
                    _shieldManager.SetCurrent(_shieldManager.GetPreviousShield());
                    RestoreAlpha(SpinRight);
                    StartCoroutine(UnphaseSpinRightButton());
                }
                LastFinger = touch.fingerId;
            }
        }
	}

    IEnumerator UnphaseSpinLeftButton()
    {
        yield return new WaitForSeconds(1);
        ReduceAlpha(SpinLeft, .2f);
    }

    IEnumerator UnphaseSpinRightButton()
    {
        yield return new WaitForSeconds(1);
        ReduceAlpha(SpinRight, .2f);
    }

    private void RestoreAlpha(GUITexture texture)
    {
        texture.color = new Color(texture.color.r, texture.color.g, texture.color.b, 1);
    }

    private void ReduceAlpha(GUITexture texture, float a)
    {
        texture.color = new Color(texture.color.r, texture.color.g, texture.color.b, a);
    }
}
