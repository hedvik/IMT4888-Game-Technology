using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitAroundTransform : MonoBehaviour {
    public Transform _target;
    public Vector3 _axis;
    public float _speed = 5;

    private void Update()
    {
        transform.RotateAround(_target.position, _axis, Time.deltaTime * _speed);
    }
}
