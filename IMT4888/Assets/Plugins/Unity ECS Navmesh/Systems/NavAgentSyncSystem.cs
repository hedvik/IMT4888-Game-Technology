using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using NavJob.Components;

namespace NavJob.Systems
{
    /// <summary>
    /// Sets the NavAgent position to the Position component
    /// </summary>
    [UpdateBefore (typeof (NavAgentSystem))]
    public class NavAgentFromPositionSyncSystem : JobComponentSystem
    {
        [BurstCompile]
        [RequireComponentTag (typeof (SyncPositionToNavAgent))]
        private struct NavAgentFromPositionSyncSystemJob : IJobProcessComponentData<NavAgent, Position>
        {
            public void Execute (ref NavAgent NavAgent, [ReadOnly] ref Position Position)
            {
                NavAgent.position = Position.Value;
            }
        }

        protected override JobHandle OnUpdate (JobHandle inputDeps)
        {
            return new NavAgentFromPositionSyncSystemJob ().Schedule (this, inputDeps);
        }
    }

    /// <summary>
    /// Sets the Position component to the NavAgent position
    /// </summary>
    [UpdateAfter (typeof (NavAgentSystem))]
    public class NavAgentToPositionSyncSystem : JobComponentSystem
    {
        [BurstCompile]
        [RequireComponentTag (typeof (SyncPositionFromNavAgent))]
        private struct NavAgentToPositionSyncSystemJob : IJobProcessComponentData<NavAgent, Position>
        {
            public void Execute ([ReadOnly] ref NavAgent NavAgent, ref Position Position)
            {
                Position.Value = NavAgent.position;
            }
        }

        protected override JobHandle OnUpdate (JobHandle inputDeps)
        {
            return new NavAgentToPositionSyncSystemJob ().Schedule (this, inputDeps);
        }
    }

    /// <summary>
    /// Sets the NavAgent rotation to the Rotation component
    /// </summary>
    [UpdateBefore (typeof (NavAgentSystem))]
    public class NavAgentFromRotationSyncSystem : JobComponentSystem
    {
        [BurstCompile]
        [RequireComponentTag (typeof (SyncRotationToNavAgent))]
        private struct NavAgentFromRotationSyncSystemJob : IJobProcessComponentData<NavAgent, Rotation>
        {
            public void Execute (ref NavAgent NavAgent, [ReadOnly] ref Rotation Rotation)
            {
                NavAgent.rotation = Rotation.Value;
            }
        }

        protected override JobHandle OnUpdate (JobHandle inputDeps)
        {
            return new NavAgentFromRotationSyncSystemJob ().Schedule (this, inputDeps);
        }
    }

    /// <summary>
    /// Sets the Rotation component to the NavAgent rotation
    /// </summary>
    [UpdateAfter (typeof (NavAgentSystem))]
    public class NavAgentToRotationSyncSystem : JobComponentSystem
    {
        [BurstCompile]
        [RequireComponentTag (typeof (SyncRotationFromNavAgent))]
        private struct NavAgentToRotationSyncSystemJob : IJobProcessComponentData<NavAgent, Rotation>
        {
            public void Execute ([ReadOnly] ref NavAgent NavAgent, ref Rotation Rotation)
            {
                Rotation.Value = NavAgent.rotation;
            }
        }

        protected override JobHandle OnUpdate (JobHandle inputDeps)
        {
            return new NavAgentToRotationSyncSystemJob ().Schedule (this, inputDeps);
        }
    }
}