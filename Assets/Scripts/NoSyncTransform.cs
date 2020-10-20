using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

[Unity.Burst.BurstCompile]
public struct NoSyncTransformJob : IWeightedAnimationJob
{
	public ReadOnlyTransformHandle source;
	public ReadWriteTransformHandle destination;

	public void ProcessAnimation(AnimationStream stream)
	{
		float w = jobWeight.Get(stream);
		if (w > 0f)
		{
			source.GetLocalTRS(stream, out var position, out var rotation, out var scale);
			destination.GetLocalTRS(stream, out var position2, out var rotation2, out var scale2);
			destination.SetLocalTRS(stream, position, rotation, scale2);
		}
		else
		{
			AnimationRuntimeUtils.PassThrough(stream, destination);
		}
	}

	public void ProcessRootMotion(AnimationStream stream)
	{
	}

	public FloatProperty jobWeight { get; set; }
}

public class NoSyncTransformJobBinder : AnimationJobBinder<NoSyncTransformJob, NoSyncTransformData>
{
	public override NoSyncTransformJob Create(Animator animator, ref NoSyncTransformData data, Component component)
	{
		var job = new NoSyncTransformJob();

		job.source = ReadOnlyTransformHandle.Bind(animator, data.source);
		job.destination = ReadWriteTransformHandle.Bind(animator, data.destination);

		return job;
	}

	public override void Destroy(NoSyncTransformJob job)
	{
	}
}

[Serializable]
public struct NoSyncTransformData : IAnimationJobData
{
	[SerializeField] public Transform source;
	[SerializeField] public Transform destination;

	public bool IsValid()
	{
		return source != null && destination != null;
	}

	public void SetDefaultValues()
	{
		source = null;
		destination = null;
	}
}

public class NoSyncTransform :  RigConstraint<
	NoSyncTransformJob,
	NoSyncTransformData,
	NoSyncTransformJobBinder
>
{
}
