using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUITexture))]
public class Touchpad : MonoBehaviour {
    
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
    public Vector2 position; 									// [-1, 1] in x,y
    public int tapCount;	                                    // Current tap count

    private int lastFingerId = -1;								// Finger last used for this joystick
    private float tapTimeWindow; 				                // How much time there is left for a tap to occur
    private Vector2 fingerDownPos;
    private float fingerDownTime;
    private float firstDeltaTime = 0.5f;

    private GUITexture gui;           							// Joystick graphic
    private Rect defaultRect;   					            // Default position / extents of the joystick graphic
    private Boundary guiBoundary = new Boundary();			    // Boundary for joystick graphic
    private Vector2 guiTouchOffset;				                // Offset to apply to touch input
    private Vector2 guiCenter;  					            // Center of joystick

    void Awake()
    {
        Touchpads = new Touchpad[0];
        Debug.Log("Touch pad awoken");
    }

	void Start ()
	{
        enumeratedJoysticks = false;
        gui = GetComponent<GUITexture>();
	    defaultRect = gui.pixelInset;

	    defaultRect.x += transform.position.x*Screen.width;
	    defaultRect.y += transform.position.y*Screen.height;

	    transform.position = new Vector3(0,0,0);

        if (touchPad)
        {
            if (gui.texture)
            {
                touchZone = defaultRect;
            }
        }
        else
        {
            guiTouchOffset = new Vector2(defaultRect.width * 0.5f, defaultRect.height * 0.5f);
            guiCenter = new Vector2(defaultRect.x + guiTouchOffset.x, defaultRect.y + guiTouchOffset.y);

            guiBoundary.min.x = defaultRect.x - guiTouchOffset.x;
            guiBoundary.max.x = defaultRect.x + guiTouchOffset.x;
            guiBoundary.min.y = defaultRect.y - guiTouchOffset.y;
            guiBoundary.max.y = defaultRect.y + guiTouchOffset.y;
        }
	}

    public void Disable()
    {
        gameObject.SetActive(false);
        enumeratedJoysticks = false;
    }

    public void Enable()
    {
        gameObject.SetActive(true);
        enumeratedJoysticks = true;
    }

    void ResetJoystick()
    {
        // Release the finger control and set the joystick back to the default position
        gui.pixelInset = defaultRect;
        lastFingerId = -1;
        position = Vector2.zero;
        fingerDownPos = Vector2.zero;

        if (touchPad)
            gui.color = new Color(gui.color.r, gui.color.g, gui.color.b, .025f);
    }

    private bool IsFingerDown()
    {
        return (lastFingerId != -1);
    }

    public void LatchedFinger(int fingerId)
    {
        // If another joystick has latched this finger, then we must release it
        if (IsLatched(fingerId))
            ResetJoystick();
    }

    public bool IsLatched(int fingerId)
    {
        return lastFingerId == fingerId;
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
        if (tapTimeWindow > 0)
            tapTimeWindow -= Time.deltaTime;
        else
            tapCount = 0;

        if (count == 0)
            ResetJoystick();
        else
        {
            for (int i = 0; i < count; i++)
            {
                var touch = Input.GetTouch(i);
                var guiTouchPos = touch.position - guiTouchOffset;

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
                if (shouldLatchFinger && (lastFingerId == -1 || lastFingerId != touch.fingerId))
                {

                    if (touchPad)
                    {
                        gui.color = new Color(gui.color.r, gui.color.g, gui.color.b, 0.15f);

                        lastFingerId = touch.fingerId;
                        fingerDownPos = touch.position;
                        fingerDownTime = Time.time;
                    }

                    lastFingerId = touch.fingerId;

                    // Accumulate taps if it is within the time window
                    if (tapTimeWindow > 0)
                        tapCount++;
                    else
                    {
                        tapCount = 1;
                        tapTimeWindow = tapTimeDelta;
                    }

                    // Tell other Touchpads we've latched this finger
                    foreach (var j in Touchpads )
                    {
                        if (j != this)
                            j.LatchedFinger(touch.fingerId);
                    }
                }

                if (lastFingerId == touch.fingerId)
                {
                    // Override the tap count with what the iPhone SDK reports if it is greater
                    // This is a workaround, since the iPhone SDK does not currently track taps
                    // for multiple touches
                    if (touch.tapCount > tapCount)
                        tapCount = touch.tapCount;

                    if (touchPad)
                    {
                        // For a touchpad, let's just set the position directly based on distance from initial touchdown
                        position.x = Mathf.Clamp((touch.position.x - fingerDownPos.x)/(touchZone.width/2), -1, 1);
                        position.y = Mathf.Clamp((touch.position.y - fingerDownPos.y)/(touchZone.height/2), -1, 1);
                    }
                    else
                    {
                        // Change the location of the joystick graphic to match where the touch is
                        gui.pixelInset = new Rect(
                            Mathf.Clamp(guiTouchPos.x, guiBoundary.min.x, guiBoundary.max.x),
                            Mathf.Clamp(guiTouchPos.y, guiBoundary.min.y, guiBoundary.max.y),
                            gui.pixelInset.width,
                            gui.pixelInset.height);
                    }

                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                        ResetJoystick();
                }
            }
        }

        if (!touchPad)
        {
            // Get a value between -1 and 1 based on the joystick graphic location
            position.x = (gui.pixelInset.x + guiTouchOffset.x - guiCenter.x)/guiTouchOffset.x;
            position.y = (gui.pixelInset.y + guiTouchOffset.y - guiCenter.y)/guiTouchOffset.y;
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
        return gui.HitTest(p);
    }
}
