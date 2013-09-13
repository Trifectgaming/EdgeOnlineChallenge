using UnityEngine;
using System.Collections;

public class ShowErrorMenu : MonoBehaviour
{
    public ErrorDisplay ErrorDisplay;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    for (int index = 0; index < Input.touches.Length; index++)
	    {
            if (Input.touches[index].phase == TouchPhase.Began)
            {
                var ray = Camera.main.ScreenPointToRay(Input.touches[index].position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject == gameObject)
                        ErrorDisplay.Show();
                }
            }
	    }
	}

    void OnClick()
    {
        ErrorDisplay.Show();
    }
}
