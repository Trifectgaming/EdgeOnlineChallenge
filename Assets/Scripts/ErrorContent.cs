using UnityEngine;

public class ErrorContent : MonoBehaviour
{
    public UILabel Message;
    public UILabel Date;
    private Transform _transform;

    public void SetContent(ExceptionMessage message)
    {
        Message.text = message.Message;
        Date.text = message.Occurance.ToString();
    }

    private void Start()
    {
        _transform = transform;
    }

    private void Update()
    {
        if (_transform.parent != null)
        {
            if (_transform.localPosition.z != _transform.parent.localPosition.z)
            {
                Debug.Log("Parent z is " + _transform.parent.localPosition.z + " " + _transform.parent.gameObject.name);
                _transform.localPosition = new Vector3(_transform.localPosition.x, _transform.localPosition.y, -2);
            }
        }
    }
}