using UnityEngine;
using System.Collections;

public struct Sound
{
	private Vector3 position;
	private float volume;

	public Sound (Vector3 position, float volume = 1)
	{
		this.position = position;
		this.volume = volume;
	}

	public Vector3 GetPosition ()
	{
		return position;
	}

	public float GetVolume ()
	{
		return volume;
	}

	public static void MakeSound (Vector3 position, float volume)
	{
		GameObject[] objects = GameObject.FindGameObjectsWithTag ("Enemy");
		foreach (GameObject o in objects) {
			o.BroadcastMessage ("Sound", new Sound (position, volume), SendMessageOptions.DontRequireReceiver);
		}
	}
}
