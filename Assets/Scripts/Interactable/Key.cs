using UnityEngine;
using System.Collections;

public class Key : Interactable
{
	void Awake()
	{

	}

	public override void Interact()
	{
		Debug.Log("It works");
	}
}
