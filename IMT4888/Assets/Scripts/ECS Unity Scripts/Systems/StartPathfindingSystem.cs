using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class StartPathfindingSystem : ComponentSystem
{
    struct InjectionData
    {
        public readonly int Length;
        public ComponentDataArray<NavJob.Components.NavAgent> navAgents;
        public EntityArray entityArray;
    }

    [Inject] InjectionData entities;
    [Inject] NavJob.Systems.NavAgentSystem navAgentSystem;

    protected override void OnUpdate()
    {
        for (int i = 0; i < entities.Length; i++)
        {
            navAgentSystem.SetDestination(entities.entityArray[i], entities.navAgents[i], BootstrapperPathfinding._goalPosition);
        }
        Enabled = false;
    }
}
