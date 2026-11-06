using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 position = target.position;
        position.z = transform.position.z;
        transform.position = position;
    }
}