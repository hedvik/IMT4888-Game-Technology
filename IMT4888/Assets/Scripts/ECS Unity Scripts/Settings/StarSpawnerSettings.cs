using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class StarSpawnerSettings : MonoBehaviour {
    public int _numberOfStars;
    public int _numberOfOrbits;
    public float _orbitOffset;
    public float _speed;
    public Vector3 _orbitAxis;
    public float _starScale = 0f;
    public bool _noiseOffset = true;
    public float _noiseOffsetAmount = 0.5f;
    public bool _scaleRotationToDistance = true;

    public List<Material> _materials = new List<Material>();
}
