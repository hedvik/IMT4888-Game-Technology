using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using UnityEngine.Jobs;

public class StarOrbitSystem : JobComponentSystem
{
    [BurstCompile]
    public struct OrbitJob : IJobProcessComponentData<Position, OrbitingStar>
    {
        [ReadOnly] public float _deltaTime;

        // This should more or less be equivalent of RotateAround() using the Y axis
        // The primary difference is that we do not change any rotation matrices.
        public void Execute(ref Position position, [ReadOnly] ref OrbitingStar starData)
        {
            // To give some context on the calculations:
            // We know: 
            // 1. The current position of the star which was randomly placed on the edge of a circle
            // 2. The center of this circle is also known
            // 3. The radius of the circle/the distance from the center
            var currentAngle = math.atan2(position.Value.z - starData._target.z, position.Value.x - starData._target.x);

            // To simulate the behaviour of RotateAround() we have to look at the current angle and add/subtract (deltaTime*speed) 
            position.Value = new float3(
                math.cos(currentAngle - _deltaTime * starData._speed) * starData._distanceFromTarget + starData._target.x,
                position.Value.y,
                math.sin(currentAngle - _deltaTime * starData._speed) * starData._distanceFromTarget + starData._target.z
                );
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var systemJob = new OrbitJob { _deltaTime = Time.deltaTime };
        var handle = systemJob.Schedule(this, inputDeps);
        return handle;
    }
}
