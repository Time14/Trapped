using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Light))]
public class LampControl : MonoBehaviour
{

	private Animator anim;


	// Use this for initialization
	void Awake()
	{
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update()
	{
		if (RInput.GetButtonDown("LampToggle"))
			anim.SetBool("isOn", !anim.GetBool("isOn"));
	}
}
