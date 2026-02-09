using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour
{
    private Vector3 offset = new Vector3(0f, 0f, -10f);
    [SerializeField] private Transform target;
    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + offset;
    }
}
