using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapperPathfinding
{
    public static EntityArchetype _agentArchetype;
    public static MeshInstanceRenderer _agentLook;

    private static float3 _goalPosition = new float3();
    private static float2x2 _spawnMinMax = new float2x2();
    private static int _numberOfAgents = new int();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitializeBeforeScene()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        CreateArchetypes(entityManager);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void IntitializeAfterScene()
    {
        if (SceneManager.GetActiveScene().name.Contains("PathfindingECS"))
        {
            var entityManager = World.Active.GetOrCreateManager<EntityManager>();
            GetSettingsFromPrototype("AgentSpawnerSettings");
            CreateEntities(entityManager);
        }
    }

    private static void CreateArchetypes(EntityManager entityManager)
    {

    }

    private static void CreateEntities(EntityManager entityManager)
    {

    }

    private static void GetSettingsFromPrototype(string prototypeName)
    {
        var prototype = GameObject.Find(prototypeName);
        var settings = prototype.GetComponent<EcsAgentSpawnerComponent>();
        var spawnArea = settings.GetComponent<Collider>();

        _agentLook = prototype.GetComponent<MeshInstanceRenderer>();
        _numberOfAgents = settings._numberOfAgents;
        _goalPosition = settings._goal.position;
        _spawnMinMax = new float2x2(spawnArea.bounds.min.x, spawnArea.bounds.min.z,
                                    spawnArea.bounds.max.x, spawnArea.bounds.max.z);

        Object.Destroy(prototype);
    }
}
