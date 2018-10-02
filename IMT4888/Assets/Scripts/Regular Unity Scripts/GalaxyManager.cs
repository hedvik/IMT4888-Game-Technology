using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class GalaxyManager : MonoBehaviour
{
    public int _numberOfStars;
    public int _numberOfOrbits;
    public float _orbitOffset;
    public float _speed;
    public Vector3 _orbitAxis;
    public GameObject _starPrefab;
    public bool _noiseOffset = true;
    public float _noiseOffsetAmount = 0.5f;
    public bool _scaleRotationToDistance = true;

    public List<Material> _materials = new List<Material>();

    private List<Transform> _stars = new List<Transform>();
    private List<float> _starSpeeds = new List<float>();
    private NativeArray<float> _nativeStarSpeeds;
    private TransformAccessArray _transformAccessArray;

    private struct OrbitJob : IJobParallelForTransform
    {
        [ReadOnly] public float _deltaTime;
        [ReadOnly] public Vector3 _axis;
        [ReadOnly] public Vector3 _targetPosition;
        [ReadOnly] public NativeArray<float> _speeds;

        public void Execute(int index, TransformAccess transform)
        {
            var currentAngle = math.atan2(transform.position.z - _targetPosition.z, transform.position.x - _targetPosition.x);
            var radius = math.sqrt(math.pow(transform.position.x - _targetPosition.x, 2) + math.pow(transform.position.z - _targetPosition.z, 2));

            transform.position = new Vector3(
                math.cos(currentAngle - _deltaTime * _speeds[index]) * radius + _targetPosition.x,
                transform.position.y,
                math.sin(currentAngle - _deltaTime * _speeds[index]) * radius + _targetPosition.z
                );
        }
    }

    private void Start()
    {
        for (int i = 0; i < _numberOfOrbits; i++)
        {
            for (int j = 0; j < _numberOfStars / _numberOfOrbits; j++)
            {
                var newStar = Instantiate(_starPrefab);
                newStar.GetComponent<MeshRenderer>().material = _materials[UnityEngine.Random.Range(0, _materials.Count)];

                var randomX = Mathf.Cos(UnityEngine.Random.Range(0f, 360f));
                var randomY = Mathf.Sin(UnityEngine.Random.Range(0f, 360f));

                // TODO: Might want to make use of the axis here somehow
                var offsetDirection = new Vector3(randomX, 0, randomY);

                newStar.transform.position =
                    transform.position +
                    (offsetDirection.normalized * (i + 1) * _orbitOffset) *
                    (_noiseOffset ? UnityEngine.Random.Range(0f, _noiseOffsetAmount) : 1);

                _starSpeeds.Add(_scaleRotationToDistance ? (_speed / (newStar.transform.position - transform.position).magnitude) : _speed);
                _stars.Add(newStar.transform);
            }
        }

        _nativeStarSpeeds = new NativeArray<float>(_starSpeeds.ToArray(), Allocator.Persistent);
        _transformAccessArray = new TransformAccessArray(_stars.ToArray());
    }

    private void Update()
    {
        
        var movementJob = new OrbitJob()
        {
            _deltaTime = Time.deltaTime,
            _speeds = _nativeStarSpeeds,
            _axis = _orbitAxis,
            _targetPosition = transform.position
        };

        var handle = movementJob.Schedule(_transformAccessArray);
        handle.Complete();
    }

    private void OnDisable()
    {
        _nativeStarSpeeds.Dispose();
        _transformAccessArray.Dispose();
    }
}
