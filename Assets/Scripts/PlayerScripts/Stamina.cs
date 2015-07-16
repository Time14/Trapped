using UnityEngine;
using System.Collections;

public class Stamina : MonoBehaviour
{

	public float maxStamina = 100;

	//Messured in stamina units per second
	private float staminaIncrease = 10;
	private float stamina = 0;
	private float smoothStamina = 0;
	private float staminaCoolDown = 2f;
	private float staminaCoolDownTimer = 0;
	private UnityEngine.UI.Image staminaBar;

	void Start()
	{
		staminaBar = GameObject.FindGameObjectWithTag("Stamina").GetComponent<UnityEngine.UI.Image>();
		staminaBar.type = UnityEngine.UI.Image.Type.Filled;
	}

	void Update()
	{
		smoothStamina += (stamina - smoothStamina) * 10 * Time.deltaTime;
		staminaBar.fillAmount = smoothStamina / maxStamina;
		if (staminaCoolDownTimer <= Time.time && stamina < maxStamina)
		{
			stamina += staminaIncrease * Time.deltaTime;
		}
		else if (stamina > maxStamina)
		{
			stamina = maxStamina;
		}
	}

	public bool DecreaseStamina(float delta)
	{
		if (stamina - delta < 0)
			return false;

		stamina -= delta;
		staminaCoolDownTimer = Time.time + staminaCoolDown;
		return true;
	}

	/*
	 * float staminaMultiplier
	 * float stamina
	 * update()
	 *    If stamina.unchangedForXSeconds
	 *       stamina++
	 *    else
	 *       return
	 * 
	 * public bool decrease(float ammount)
	 *     
	 *    if stamina - ammount < 0
	 *         return false
	 * 
	 *     staminaCoolDown.reset()
	 *     stamina -= ammount
	 *     
	 * 
	 * 
	 * 
	 * 
	 */
}
