using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{

	public void Awake()
	{
		RInput.LoadKeys();
		//RInput.AddButton("Interact", KeyCode.R);
		RInput.SaveKeys();
	}

	public void Update()
	{
		if (RInput.GetButtonUp("Quit"))
		{
			Application.Quit();
		}
	}
}
