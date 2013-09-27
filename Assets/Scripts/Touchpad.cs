using UnityEngine;
using System.Collections;

public abstract class Control : MonoBehaviour
{
    public abstract void Disable();
    public abstract void Enable();
    public Vector2 position;
}

[RequireComponent(typeof(GUITexture))]
public class Touchpad : Control
{
    
    public class Boundary 
    {
	    public Vector2 min = Vector2.zero;
	    public Vector2 max = Vector2.zero;
    }

    public static Touchpad[] Touchpads = new Touchpad[0];		// A static collection of all Touchpads
    private static bool enumeratedJoysticks = false;
    private static float tapTimeDelta = 0;

    public bool touchPad; 									    // Is this a TouchPad?
    public Rect touchZone;
    public Vector2 deadZone = Vector2.zero;						// Control when position is output
    public bool normalize = false; 							    // Normalize output after the dead-zone?
    public int tapCount;	                                    // Current tap count

    private int _lastFingerId = -1;								// Finger last used for this joystick
    private float _tapTimeWindow; 				                // How much time there is left for a tap to occur
    private Vector2 _fingerDownPos;
    
    private GUITexture _gui;           							// Joystick graphic
    private Rect _defaultRect;   					            // Default position / extents of the joystick graphic
    private readonly Boundary _guiBoundary = new Boundary();			    // Boundary for joystick graphic
    private Vector2 _guiTouchOffset;				                // Offset to apply to touch input
    private Vector2 _guiCenter;  					            // Center of joystick

    void Awake()
    {
        Touchpads = new Touchpad[0];
        Debug.Log("Touch pad awoken");
    }

	void Start ()
	{
        enumeratedJoysticks = false;
        _gui = GetComponent<GUITexture>();
	    _defaultRect = _gui.pixelInset;

	    _defaultRect.x += transform.position.x*Screen.width;
	    _defaultRect.y += transform.position.y*Screen.height;

	    transform.position = new Vector3(0,0,0);

        if (touchPad)
        {
            if (_gui.texture)
            {
                touchZone = _defaultRect;
            }
        }
        else
        {
            _guiTouchOffset = new Vector2(_defaultRect.width * 0.5f, _defaultRect.height * 0.5f);
            _guiCenter = new Vector2(_defaultRect.x + _guiTouchOffset.x, _defaultRect.y + _guiTouchOffset.y);

            _guiBoundary.min.x = _defaultRect.x - _guiTouchOffset.x;
            _guiBoundary.max.x = _defaultRect.x + _guiTouchOffset.x;
            _guiBoundary.min.y = _defaultRect.y - _guiTouchOffset.y;
            _guiBoundary.max.y = _defaultRect.y + _guiTouchOffset.y;
        }
	}

    public override void Disable()
    {
        gameObject.SetActive(false);
        enumeratedJoysticks = false;
    }

    public override void Enable()
    {
        gameObject.SetActive(true);
        enumeratedJoysticks = true;
    }

    void ResetJoystick()
    {
        // Release the finger control and set the joystick back to the default position
        _gui.pixelInset = _defaultRect;
        _lastFingerId = -1;
        position = Vector2.zero;
        _fingerDownPos = Vector2.zero;

        if (touchPad)
            _gui.color = new Color(_gui.color.r, _gui.color.g, _gui.color.b, .025f);
    }

    private bool IsFingerDown()
    {
        return (_lastFingerId != -1);
    }

    public void LatchedFinger(int fingerId)
    {
        // If another joystick has latched this finger, then we must release it
        if (IsLatched(fingerId))
            ResetJoystick();
    }

    public bool IsLatched(int fingerId)
    {
        return _lastFingerId == fingerId;
    }

    private void Update()
    {
        if (!enumeratedJoysticks)
        {
            // Collect all Touchpads in the game, so we can relay finger latching messages
            Touchpads = (Touchpad[])FindObjectsOfType(typeof(Touchpad));
            enumeratedJoysticks = true;
        }

        var count = Input.touchCount;

        // Adjust the tap time window while it still available
        if (_tapTimeWindow > 0)
            _tapTimeWindow -= Time.deltaTime;
        else
            tapCount = 0;

        if (count == 0)
            ResetJoystick();
        else
        {
            for (int i = 0; i < count; i++)
            {
                var touch = Input.GetTouch(i);
                var guiTouchPos = touch.position - _guiTouchOffset;

                var shouldLatchFinger = false;
                if (touchPad)
                {
                    if (touchZone.Contains(touch.position))
                        shouldLatchFinger = true;
                }
                else if (HitTest(touch.position))
                {
                    shouldLatchFinger = true;
                }

                // Latch the finger if this is a new touch
                if (shouldLatchFinger && (_lastFingerId == -1 || _lastFingerId != touch.fingerId))
                {

                    if (touchPad)
                    {
                        _gui.color = new Color(_gui.color.r, _gui.color.g, _gui.color.b, 0.15f);

                        _lastFingerId = touch.fingerId;
                        _fingerDownPos = touch.position;
                    }

                    _lastFingerId = touch.fingerId;

                    // Accumulate taps if it is within the time window
                    if (_tapTimeWindow > 0)
                        tapCount++;
                    else
                    {
                        tapCount = 1;
                        _tapTimeWindow = tapTimeDelta;
                    }

                    // Tell other Touchpads we've latched this finger
                    foreach (var j in Touchpads )
                    {
                        if (j != this)
                            j.LatchedFinger(touch.fingerId);
                    }
                }

                if (_lastFingerId == touch.fingerId)
                {
                    // Override the tap count with what the iPhone SDK reports if it is greater
                    // This is a workaround, since the iPhone SDK does not currently track taps
                    // for multiple touches
                    if (touch.tapCount > tapCount)
                        tapCount = touch.tapCount;

                    if (touchPad)
                    {
                        // For a touchpad, let's just set the position directly based on distance from initial touchdown
                        position.x = Mathf.Clamp((touch.position.x - _fingerDownPos.x)/(touchZone.width/2), -1, 1);
                        position.y = Mathf.Clamp((touch.position.y - _fingerDownPos.y)/(touchZone.height/2), -1, 1);
                    }
                    else
                    {
                        // Change the location of the joystick graphic to match where the touch is
                        _gui.pixelInset = new Rect(
                            Mathf.Clamp(guiTouchPos.x, _guiBoundary.min.x, _guiBoundary.max.x),
                            Mathf.Clamp(guiTouchPos.y, _guiBoundary.min.y, _guiBoundary.max.y),
                            _gui.pixelInset.width,
                            _gui.pixelInset.height);
                    }

                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                        ResetJoystick();
                }
            }
        }

        if (!touchPad)
        {
            // Get a value between -1 and 1 based on the joystick graphic location
            position.x = (_gui.pixelInset.x + _guiTouchOffset.x - _guiCenter.x)/_guiTouchOffset.x;
            position.y = (_gui.pixelInset.y + _guiTouchOffset.y - _guiCenter.y)/_guiTouchOffset.y;
        }

        // Adjust for dead zone	
        var absoluteX = Mathf.Abs(position.x);
        var absoluteY = Mathf.Abs(position.y);

        if (absoluteX < deadZone.x)
        {
            // Report the joystick as being at the center if it is within the dead zone
            position.x = 0;
        }
        else if (normalize)
        {
            // Rescale the output after taking the dead zone into account
            position.x = Mathf.Sign(position.x)*(absoluteX - deadZone.x)/(1 - deadZone.x);
        }

        if (absoluteY < deadZone.y)
        {
            // Report the joystick as being at the center if it is within the dead zone
            position.y = 0;
        }
        else if (normalize)
        {
            // Rescale the output after taking the dead zone into account
            position.y = Mathf.Sign(position.y)*(absoluteY - deadZone.y)/(1 - deadZone.y);
        }
    }

    public bool HitTest(Vector2 p)
    {
        return _gui.HitTest(p);
    }
}
