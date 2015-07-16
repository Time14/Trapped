using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour
{

	public GameObject target;
	public Vector3 offset;
	public float slide;
	public float maxDistace;

	void Start()
	{
		transform.rotation = Quaternion.LookRotation(-offset);
	}

	void Update()
	{
		Vector3 desierdPosition = (target.transform.position + offset);
		Vector3 dir = desierdPosition - gameObject.transform.position;

		//transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-transform.position + target.transform.position), 1 - Mathf.Clamp(dir.magnitude / maxDistace, 0, 1)); 

		if (dir.magnitude < (0.5f * Time.deltaTime))
		{
			gameObject.transform.position = desierdPosition;
			return;
		} else
		{
			gameObject.transform.position += dir * slide * Time.deltaTime;
		}
	}
}
