using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSpawner : MonoBehaviour {
    public int _numberOfStars;
    public int _numberOfOrbits;
    public float _orbitOffset;
    public float _speed;
    public Vector3 _orbitAxis;
    public GameObject _starPrefab;
    public bool _noiseOffset = true;
    public float _noiseOffsetAmount = 0.5f;
    public bool _scaleRotationToDistance = true;

    public List<Material> materials = new List<Material>();

    private void Start()
    {
        for(int i = 0; i < _numberOfOrbits; i++)
        {
            for(int j = 0; j < _numberOfStars/_numberOfOrbits; j++)
            {
                var newStar = Instantiate(_starPrefab);
                var newStarComponent = newStar.GetComponent<OrbitAroundTransform>();
                newStarComponent._axis = _orbitAxis;
                newStarComponent._target = transform;
                newStar.GetComponent<MeshRenderer>().material = materials[Random.Range(0, materials.Count)];

                var randomX = Mathf.Cos(Random.Range(0f, 360f));
                var randomY = Mathf.Sin(Random.Range(0f, 360f));

                // TODO: Might want to make use of the axis here somehow
                var offsetDirection = new Vector3(randomX, 0, randomY);

                newStar.transform.position = 
                    transform.position + 
                    (offsetDirection.normalized * (i+1) * _orbitOffset) * 
                    (_noiseOffset ? Random.Range(0f, _noiseOffsetAmount) : 1);

                newStarComponent._speed = _scaleRotationToDistance ? (_speed / (newStar.transform.position - transform.position).magnitude) : _speed;
            }
        }
    }
}
