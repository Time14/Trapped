using UnityEngine;
using System.Collections;

public class PlayerInteract : MonoBehaviour
{

	void OnTriggerEnter(Collider col)
	{
		if (col.CompareTag("Interact"))
		{
			col.gameObject.BroadcastMessage("EnterInteract", SendMessageOptions.RequireReceiver);
		}
	}

	void OnTriggerStay(Collider col)
	{
		if (col.CompareTag("Interact"))
		{
			col.gameObject.BroadcastMessage("UpdateUIPosition", SendMessageOptions.RequireReceiver);
			if (RInput.GetButtonDown("Interact"))
			{
				col.gameObject.BroadcastMessage("Interact", SendMessageOptions.RequireReceiver);
			}
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (col.CompareTag("Interact"))
		{
			col.gameObject.BroadcastMessage("ExitInteract", SendMessageOptions.RequireReceiver);
		}
	}

	/*
	 * OnCollision(col) 
	 *     if (col.tag == "Interact" && buttonDown("Interact"))
	 *         col.gameObject.broadcast("Interact", requireReciver);
	 * 
	 * 
	 */
}
