using System.Collections.Generic;
using UnityEngine;

public class OverlayManager : MonoBehaviour
{
    public float speed;
    public float OverlayWidth;
    public Vector3 end;
    public Vector3 start;
    private Transform _transform;
    private List<Node<Transform>> _overlayQueue;
    private Node<Transform> _head;

    void Start ()
    {
        _transform = transform;
        var sprites = gameObject.GetComponentsInChildren<tk2dSprite>();
        _overlayQueue = new List<Node<Transform>>(sprites.Length);        
        OverlayWidth = sprites[0].GetBounds().size.x;
        end = new Vector3(UIHelper.MinX - OverlayWidth, 0, _transform.position.z);
        start = new Vector3(UIHelper.MaxX, 0, transform.position.z);
        for (int index = 0; index < sprites.Length; index++)
        {
            var sprite = sprites[index];
            var currentTransform = sprite.transform;
            if (index == 0)
            {
                currentTransform.position = new Vector3(UIHelper.MinX, 0, _transform.position.z);
            }
            else
            {
                currentTransform.position = start;
            }
            var newNode = new Node<Transform>(currentTransform);
            if (_overlayQueue.Count > 0)
                _overlayQueue[index - 1].Next = newNode;
            _overlayQueue.Add(newNode);
            if (_overlayQueue.Count == sprites.Length)
            {
                _overlayQueue[index].Next = _overlayQueue[0];
            }
        }
        _head = _overlayQueue[0];
    }


    void Update ()
    {
        var currentTranform = _head.Value;
        if ((currentTranform.position.x + OverlayWidth - 1) <= UIHelper.MinX)
        {
            currentTranform.position = start;
            _head = _head.Next;
        }
        if ((currentTranform.position.x + OverlayWidth) <= UIHelper.MaxX)
        {
            _head.Next.Value.position = Vector3.MoveTowards(_head.Next.Value.position, end, Time.deltaTime * speed);            
        }
        currentTranform.position = Vector3.MoveTowards(currentTranform.position, end, Time.deltaTime * speed);
    }
}

public class Node<T>
{
    public T Value;
    public Node<T> Next;

    public Node(T value)
    {
        Value = value;
    }
}
