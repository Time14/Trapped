using UnityEngine;
using System;
using System.Collections;

public class GrappleHook : MonoBehaviour
{
	[Range(0.9f, 1)]
	public float
		energyMultiplier = 0.98f;

	[HideInInspector]
	public Vector3
		hookPoint;
	[HideInInspector]
	public bool
		grapple = false;

	private float distance;
	private Vector2 angVelocity = Vector2.zero;

	//Refferences
	private Rigidbody rigidbody;
	private PlayerMovement pm;

	void Awake()
	{
		rigidbody = gameObject.GetComponent<Rigidbody>();
		pm = gameObject.GetComponent<PlayerMovement>();
	}

	void Update()
	{
		if (RInput.GetButtonDown("Grapple"))
		{
			//Preliminary
			hookPoint = transform.position;
			hookPoint.y += 4;
			hookPoint += transform.forward * 2;

			grapple = true;
			distance = (transform.position - hookPoint).magnitude;
			angVelocity = ConvertVelocityToAngularVelocity(rigidbody.velocity);
			rigidbody.useGravity = false;
			rigidbody.velocity = Vector3.zero;
		}
		if (RInput.GetButton("Grapple"))
		{
			Vector2 gravity = ConvertVelocityToAngularVelocity(Physics.gravity);
			angVelocity += gravity * Time.deltaTime;
			Move();
		}
		else if (RInput.GetButtonUp("Grapple"))
		{
			grapple = false;
			rigidbody.useGravity = true;
			rigidbody.velocity = ConvertAngularVelocityToVelocity(angVelocity);
		}
	}

	private Vector2 ConvertVelocityToAngularVelocity(Vector3 v)
	{
		Vector3 direction = hookPoint - transform.position;
		//direction.Normalize();
		//vel -= Vector3.Dot(direction, vel) * direction;

		Vector3 tan1 = Vector3.Cross(direction, Vector3.right).normalized;
		Vector3 tan2 = Vector3.Cross(direction, Vector3.forward).normalized;

		Vector2 av = Vector3.zero;

		av.x = Vector3.Dot(tan1, v) / distance;
		av.y = Vector3.Dot(tan2, v) / distance;
	
		return av;
	}

	private Vector3 ConvertAngularVelocityToVelocity(Vector2 av)
	{
		Vector3 v = Vector3.zero;
		Vector3 direction = hookPoint - transform.position;
		Vector3 tan1 = Vector3.Cross(direction, Vector3.right).normalized;
		Vector3 tan2 = Vector3.Cross(direction, Vector3.forward).normalized;

		v += tan1 * av.x * distance;
		v += tan2 * av.y * distance;

		angVelocity = Vector2.zero;

		return v;
	}

	private Vector3 Rotate(Vector3 v, float x, float z)
	{
		v.x = v.x;
		v.y = v.y * Mathf.Cos(x) - v.z * Mathf.Sin(x);
		v.z = v.y * Mathf.Sin(x) + v.z * Mathf.Cos(x);

		v.x = v.x * Mathf.Cos(z) - v.y * Mathf.Sin(z);
		v.y = v.x * Mathf.Sin(z) + v.y * Mathf.Cos(z);
		v.z = v.z;

		return v;
	}

	public void AddAngularVelocity(Vector3 v)
	{
		angVelocity += ConvertVelocityToAngularVelocity(v) * Time.deltaTime * 2.5f;
	}

	public void ClampVelocity()
	{
		float max = 20.0f;
		angVelocity.x = Mathf.Clamp(angVelocity.x, -max, max);
		angVelocity.y = Mathf.Clamp(angVelocity.y, -max, max);
		angVelocity *= 0.95f;
		Debug.Log(angVelocity);
	}

	public void Move()
	{
		Vector3 nextPos = Rotate(transform.position - hookPoint, angVelocity.x * Time.deltaTime, angVelocity.y * Time.deltaTime);
		transform.position = hookPoint + (nextPos.normalized * distance);
		angVelocity *= energyMultiplier;
	}
}

