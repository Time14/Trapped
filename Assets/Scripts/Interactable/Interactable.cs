using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Collider))]
public abstract class Interactable : MonoBehaviour
{
	public GameObject uiElement;

	public abstract void Interact();

	public void EnterInteract()
	{
		uiElement.SetActive(true);
	}

	public void ExitInteract()
	{
		uiElement.SetActive(false);
	}

	public void UpdateUIPosition()
	{
		Vector3 screenPos;
		screenPos = Camera.main.WorldToScreenPoint(transform.position);
		screenPos.z = 0;
		uiElement.transform.position = screenPos;
	}
}
