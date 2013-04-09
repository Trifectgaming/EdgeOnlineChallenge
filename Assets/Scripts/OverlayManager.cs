using UnityEngine;

public class OverlayManager : MonoBehaviour
{
    public tk2dSprite[] sprites;
    public float speed;
    private Transform _transform;
    private Transform[] _overlayTransforms;
    private float _overlayWidth;
    private Vector3 _end;
    private int last;
    private int inView;

    void Start ()
    {
        _transform = transform;
        sprites = gameObject.GetComponentsInChildren<tk2dSprite>();
        _overlayWidth = sprites[0].GetBounds().size.x;
        _overlayTransforms = new Transform[sprites.Length];
        _end = new Vector3(UIHelper.MinX - _overlayWidth, 0, _transform.position.z);
        for (int index = 0; index < sprites.Length; index++)
        {
            var sprite = sprites[index];
            _overlayTransforms[index] = sprite.transform;
            if (index == 0)
            {
                _overlayTransforms[index].position = new Vector3(UIHelper.MinX, 0, _transform.position.z);
            }
            else
            {
                GetBehind(_overlayTransforms[index], _overlayTransforms[index - 1]);                
            }
            last = index;
        }
    }

    private void GetBehind(Transform who, Transform to)
    {
        who.position = new Vector3(to.position.x + _overlayWidth, 0, _transform.position.z);
    }

    void Update ()
    {
        var currentTranform = _overlayTransforms[inView];
        if ((currentTranform.localPosition.x + _overlayWidth) < UIHelper.MinX)
        {
            GetBehind(currentTranform, _overlayTransforms[last]);
            Iterate(ref last);
            Iterate(ref inView);
        }
        foreach (Transform trans in _overlayTransforms)
        {
            trans.position = Vector3.MoveTowards(trans.position, _end, Time.deltaTime*speed);
        }
    }

    private void Iterate(ref int i)
    {
        i++;
        if (i > (_overlayTransforms.Length - 1))
        {
            i = 0;
        }
    }
}
