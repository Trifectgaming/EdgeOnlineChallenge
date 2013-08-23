using System.Linq;
using UnityEngine;
using System.Collections;

public abstract class ShieldControlBase : GameSceneObject
{
    protected ShieldManager ShieldManager;
    protected Transform _transform;

    protected override void Start ()
    {
        base.Start();
        _transform = transform;
        var drone = ((Drone) FindObjectOfType(typeof (Drone)));
        ShieldManager = drone.shieldManager;
    }

    protected virtual void SpinShieldRight()
    {
        ShieldManager.SetCurrent(ShieldManager.GetPreviousShield());
    }

    protected virtual void SpinShieldLeft()
    {
        ShieldManager.SetCurrent(ShieldManager.GetNextShield());
    }
}

public class ShieldTapControl : ShieldControlBase
{
    public float ControlOffset;
    public tk2dSprite SpinLeft;
    public tk2dSprite SpinRight;
    public int LastFinger = -1;
    private GameObject _spinLeftGo;
    private GameObject _spinRightGo;

    // Use this for initialization

    public bool HitTest(Vector3 position, params GameObject[] objects)
    {
        if (objects == null || objects.Length == 0)
            objects = new[]
                          {
                              _spinRightGo, _spinLeftGo
                          };
        RaycastHit hitInfo;
        var rayPosition = Camera.main.ScreenPointToRay(position);
        Debug.Log("Hit Position: " + rayPosition);
        for (int index = 0; index < objects.Length; index++)
        {
            var o = objects[index];
            if (o != null) Debug.Log("Object " + index + " Position: " + o.transform.position);
        }
        if (Physics.Raycast(rayPosition, out hitInfo, 100))
        {
            Debug.Log("Touch Hit: " + hitInfo.transform.gameObject);
            return objects.Contains(hitInfo.transform.gameObject);
            //SpinRight.HitTest(position) || SpinLeft.HitTest(position)
        }
        return false;
    }


    private void PlaceManagerAtEdge()
    {
        if (UIHelper.MaxX != (_transform.position.x - ControlOffset))
        {
            _transform.position = new Vector3(UIHelper.MaxX - ControlOffset, _transform.position.y, _transform.position.z);
        }
    }

    protected override void Start()
    {
        base.Start();
        if (SpinRight)
            _spinRightGo = SpinRight.gameObject;
        if (SpinLeft)
            _spinLeftGo = SpinLeft.gameObject;
        PlaceManagerAtEdge();
    }

    // Update is called once per frame
	void Update () {
        if (Input.touchCount == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (SpinLeft && HitTest(Input.mousePosition, _spinLeftGo))
                {
                    SpinShieldLeft();
                }
                else if (SpinRight && HitTest(Input.mousePosition, _spinRightGo))
                {
                    SpinShieldRight();
                }
                else
                {
                    ShieldManager.SetCurrent(ShieldManager.GetNextShield());
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                ShieldManager.SetCurrent(ShieldManager.GetPreviousShield());
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
                if (SpinLeft && HitTest(touch.position, _spinLeftGo))
                {
                    SpinShieldLeft();
                }
                if (SpinRight && HitTest(touch.position, _spinRightGo))
                {
                    SpinShieldRight();
                }
                LastFinger = touch.fingerId;
            }
        }
	    PlaceManagerAtEdge();
	}

    protected override void SpinShieldLeft()
    {
        base.SpinShieldLeft();
        RestoreAlpha(SpinLeft);
        StartCoroutine(UnphaseSpinLeftButton());
    }

    protected override void SpinShieldRight()
    {
        base.SpinShieldRight();
        RestoreAlpha(SpinRight);
        StartCoroutine(UnphaseSpinRightButton());
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

    private void RestoreAlpha(tk2dSprite texture)
    {
        texture.color = new Color(texture.color.r, texture.color.g, texture.color.b, 1);
    }

    private void ReduceAlpha(tk2dSprite texture, float a)
    {
        texture.color = new Color(texture.color.r, texture.color.g, texture.color.b, a);
    }
}