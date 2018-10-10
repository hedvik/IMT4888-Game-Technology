using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct OrbitingStar : IComponentData
{
    public float3 _target;
    public float3 _axis;
    public float _speed;
    public float _distanceFromTarget;
}