using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapperTransformOrbit
{
    public static StarSpawnerSettings _settings;
    public static EntityArchetype _starArchetype;
    public static float3 _galaxyPosition;
    public static List<MeshInstanceRenderer> _starLooks = new List<MeshInstanceRenderer>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitializeBeforeScene()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        CreateArchetypes(entityManager);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void IntitializeAfterScene()
    {
        if (SceneManager.GetActiveScene().name.Contains("TransformOrbit-ECS"))
        {
            var entityManager = World.Active.GetOrCreateManager<EntityManager>();
            GetSettingsFromPrototype("StarSpawnerSettings");
            CreateEntities(entityManager);
        }
    }

    private static void CreateArchetypes(EntityManager entityManager)
    {
        var position = ComponentType.Create<Position>();
        var scale = ComponentType.Create<Scale>();
        var orbitingStar = ComponentType.Create<OrbitingStar>();

        _starArchetype = entityManager.CreateArchetype(position, scale, orbitingStar);
    }

    private static void CreateEntities(EntityManager entityManager)
    {
        var starEntityArray = new NativeArray<Entity>(_settings._numberOfStars, Allocator.Temp);
        entityManager.CreateEntity(_starArchetype, starEntityArray);

        for(int i = 0; i < _settings._numberOfStars; i++)
        {
            var starData = new OrbitingStar();
            starData._axis = _settings._orbitAxis;
            starData._target = _galaxyPosition;

            var randomX = Mathf.Cos(UnityEngine.Random.Range(0f, 360f));
            var randomY = Mathf.Sin(UnityEngine.Random.Range(0f, 360f));
            var orbitIndex = i % _settings._numberOfOrbits;

            // TODO: Might want to make use of the axis here somehow
            var offsetDirection = new float3(randomX, 0, randomY);

            // The position of each star is determined by three components:
            // 1: The origin of the galaxy
            // 2: a normalized+random vector within a unit circle which is multiplied by orbitIndex and the offset between orbits
            // 3: If noiseOffset is enabled, we multiply the result with a random value so that each star is not perfectly aligned with their orbit circle
            float3 starPosition = 
                _galaxyPosition +
                (math.normalize(offsetDirection) * (orbitIndex + 1) * _settings._orbitOffset) *
                (_settings._noiseOffset ? UnityEngine.Random.Range(0f, _settings._noiseOffsetAmount) : 1);

            starData._speed = _settings._scaleRotationToDistance ? (_settings._speed / math.length(starPosition - _galaxyPosition)) : _settings._speed;

            entityManager.SetComponentData(starEntityArray[i], new Position { Value = starPosition });
            entityManager.SetComponentData(starEntityArray[i], new Scale { Value = _settings._starScale });
            entityManager.SetComponentData(starEntityArray[i], starData);
            entityManager.AddSharedComponentData(starEntityArray[i], _starLooks[UnityEngine.Random.Range(0, _starLooks.Count)]);
        }
        starEntityArray.Dispose();
    }

    private static void GetSettingsFromPrototype(string prototypeName)
    {
        var prototype = GameObject.Find(prototypeName);
        _settings = prototype.GetComponent<StarSpawnerSettings>();
        _galaxyPosition = prototype.transform.position;

        var starLook = prototype.GetComponent<MeshInstanceRendererComponent>().Value;
        foreach(var material in _settings._materials)
        {
            starLook.material = material;
            _starLooks.Add(starLook);
        }

        Object.Destroy(prototype);
    }
}
