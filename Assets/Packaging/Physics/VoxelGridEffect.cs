using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Effect {
	public VoxelGridForce force;
	[Range(0.0f,1.0f)]
	public float startTime;
	[Range(0.0f,1.0f)]
	public float duration;
}

public class VoxelGridEffect : MonoBehaviour {
	public List < Effect > effects;
	[Range(0.0f,10.0f)]
	public float duration;
	// Use this for initialization
	public bool running;
	public float startTime;
	public float runningTime;
	public void Start () {
		running = false;
		startTime = runningTime = 0.0f;
		foreach (var effect in effects)
			effect.force.active = false;
	}
	public void Run () {
		if (running)
			return;
		startTime = Time.realtimeSinceStartup;
		runningTime = 0.0f;
		running = true;
	}
	void FixedUpdate () {
		if (!running)
			return;
		runningTime += Time.fixedDeltaTime;
		float delta = runningTime / duration;
		foreach (var effect in effects) {
			if (delta > effect.startTime)
				effect.force.active = true;
			if (delta > effect.startTime + effect.duration)
				effect.force.active = false;
		}
		if (delta > 1.0f)
			running = false;
	}
}
