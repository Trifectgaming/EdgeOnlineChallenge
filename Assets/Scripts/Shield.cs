using UnityEngine;

public class Shield : MonoBehaviour
{
    public ShieldColor color;
    public Transform Transform;

    private void Start()
    {
        Transform = transform;
    }
}