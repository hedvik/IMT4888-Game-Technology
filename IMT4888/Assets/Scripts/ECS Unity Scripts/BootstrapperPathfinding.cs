using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapperPathfinding
{
    public static EntityArchetype _agentArchetype;
    public static MeshInstanceRenderer _agentLook;
    public static float3 _goalPosition = new float3();

    private const float SPAWN_OFFSET_Y = 0.5f;
    private static float2x2 _spawnMinMax = new float2x2();
    private static int _numberOfAgents;
    private static float _movementSpeed;

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
        var position = ComponentType.Create<Position>();
        var navAgent = ComponentType.Create<NavJob.Components.NavAgent>();
        var syncPosFromNav = ComponentType.Create<NavJob.Components.SyncPositionFromNavAgent>();
        var syncPosToNav = ComponentType.Create<NavJob.Components.SyncPositionToNavAgent>();

        _agentArchetype = entityManager.CreateArchetype(position, navAgent, syncPosFromNav, syncPosToNav);
    }

    private static void CreateEntities(EntityManager entityManager)
    {
        var agentEntityArray = new NativeArray<Entity>(_numberOfAgents, Allocator.Temp);
        entityManager.CreateEntity(_agentArchetype, agentEntityArray);

        var navAgentComponent = new NavJob.Components.NavAgent();
        navAgentComponent.acceleration = _movementSpeed;
        navAgentComponent.stoppingDistance = 0;
        navAgentComponent.moveSpeed = _movementSpeed;
        navAgentComponent.areaMask = -1;

        for(int i = 0; i < _numberOfAgents; i++)
        {
            var randomSpawnX = UnityEngine.Random.Range(_spawnMinMax.c0.x, _spawnMinMax.c0.y);
            var randomSpawnZ = UnityEngine.Random.Range(_spawnMinMax.c1.x, _spawnMinMax.c1.y);

            entityManager.SetComponentData(agentEntityArray[i], new Position(){ Value = new float3(randomSpawnX, SPAWN_OFFSET_Y, randomSpawnZ) });
            entityManager.SetComponentData(agentEntityArray[i], navAgentComponent);
            entityManager.AddSharedComponentData(agentEntityArray[i], _agentLook);
        }

        agentEntityArray.Dispose();
    }

    private static void GetSettingsFromPrototype(string prototypeName)
    {
        var prototype = GameObject.Find(prototypeName);
        var settings = prototype.GetComponent<EcsAgentSpawnerComponent>();
        var spawnArea = settings._spawnArea.GetComponent<Collider>();

        _agentLook = prototype.GetComponent<MeshInstanceRendererComponent>().Value;
        _numberOfAgents = settings._numberOfAgents;
        _goalPosition = settings._goal.position;
        _movementSpeed = settings.movementSpeed;
        _spawnMinMax = new float2x2(spawnArea.bounds.min.x, spawnArea.bounds.min.z,
                                    spawnArea.bounds.max.x, spawnArea.bounds.max.z);

        Object.Destroy(prototype);
    }
}
