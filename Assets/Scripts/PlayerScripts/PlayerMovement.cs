using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{

	//Tweekables
	public float gravity = 9.82f;
	[Range(0, 200)]
	public float
		rollSpeed = 10;
	[Range(0, 200)]
	public float
		runSpeed = 12;
	[Range(0, 200)]
	public float
		walkSpeed = 5;
	[Range(0, 200)]
	public float
		acceleration;
	[SerializeField, Range(0, 1)]
	private float
		shortestMouseDistance;

	//Variables
	private float jumpSpeed = 700f;
	private float maxAngle = 720;
	private float maxSpeed;
	private float speed;
	private float currentAcceleration = 0;

	private bool rolling = false;
	private float rollCoolDownTimer = 0; 
	private float rollCoolDown = 0.3f;
	private float rollTimer = 0;
	private float rollTime = 0.4f;
	private Vector3 direction = Vector3.zero;
	private Vector3 rollDirection = Vector3.forward;

	//Refferences
	private Rigidbody rigidbody;
	private Transform transform;
	private Collider collider;
	private Stamina stamina;
	private GrappleHook grapple;

	//Precalculated numbers
	private float distToGround;

	void Awake()
	{
		//References
		rigidbody = GetComponent<Rigidbody>();
		transform = GetComponent<Transform>();
		collider = GetComponent<Collider>();
		stamina = GetComponent<Stamina>();
		grapple = GetComponent<GrappleHook>();

		//Pre-Calculations
		distToGround = collider.bounds.extents.y;

		maxSpeed = walkSpeed;
	}

	void FixedUpdate()
	{
		if (!rolling)
		{
			Vector3 lookDir = LookDirection();



			Turn(lookDir);

			Vector3 dir = MoveDirection().normalized;
			if (dir == Vector3.zero)
				dir = lookDir.normalized;

			if (grapple.grapple)
			{
				if (RInput.GetAxisRaw("Horizontal") != 0 || RInput.GetAxisRaw("Vertical") != 0 || RInput.GetButton("Walk"))
					grapple.AddAngularVelocity(dir * maxSpeed);
				return;
			}

			PhysicalMove(dir);
			Debug.Log(rigidbody.velocity);
			CounterSlopes();

			if (RInput.GetButtonDown("Roll") && stamina.DecreaseStamina(20))
				Roll(dir);

			if (RInput.GetButtonDown("Jump"))
				Jump();

			if (RInput.GetButtonDown("Sound"))
				Sound.MakeSound(transform.position, 1);

		} 
		else
		{
			Roll();
			Turn(rollDirection);
			PhysicalMove(rollDirection);
		}
		//if (!grapple.grapple)
		//rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
	}

	//Returns the current look directino
	Vector3 LookDirection()
	{
		RaycastHit hit;
		Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100000f, LayerMask.GetMask("Floor"));
		Vector3 lookDir = hit.point - gameObject.transform.position;
		lookDir.y = 0;
		return lookDir;
	}

	//Returns the current move direction based on the key inputs
	Vector3 MoveDirection()
	{
		return new Vector3(RInput.GetAxisRaw("Horizontal") * 0.5f + RInput.GetAxisRaw("Vertical") * 0.5f, 
		                   0, 
		                   RInput.GetAxisRaw("Vertical") * 0.5f - RInput.GetAxisRaw("Horizontal") * 0.5f);
	}

	//Calculates gravity
	void Gravity()
	{
		direction.y += gravity * Time.deltaTime;
	}

	/*//Moves the body
	void Move(Vector3 dir)
	{
		if (RInput.GetAxisRaw("Horizontal") != 0 || RInput.GetAxisRaw("Vertical") != 0 || (RInput.GetButton("Walk") 
			&& shortestMouseDistance < dir.magnitude) && !rolling && isGrounded())
		{
			if (RInput.GetButton("Run") && stamina.DecreaseStamina(20 * Time.deltaTime))
				maxSpeed = runSpeed;
			else
				maxSpeed = walkSpeed;

			speed += acceleration * Time.deltaTime;
			speed = (speed > maxSpeed) ? maxSpeed : speed;
			direction = (RInput.GetButton("Walk") ? transform.forward : dir);
			direction.Normalize();
		} 
		else if (isGrounded())
		{
			speed -= acceleration * Time.deltaTime;
			speed = (speed < 0) ? 0 : speed;	
		}

		if (!grapple.grapple)
		{
			transform.position += (direction * speed * Time.deltaTime);
		}

	}
	*/

	void PhysicalMove(Vector3 dir)
	{
		if ((RInput.GetAxisRaw("Horizontal") != 0 || RInput.GetAxisRaw("Vertical") != 0 || RInput.GetButton("Walk")) 
			&& shortestMouseDistance < dir.magnitude && !rolling && isGrounded())
		{	
			if (RInput.GetButton("Run") && stamina.DecreaseStamina(20 * Time.deltaTime))
				maxSpeed = runSpeed;
			else
				maxSpeed = walkSpeed;

			dir = (RInput.GetButton("Walk") ? transform.forward : dir);
			Vector3 vel = rigidbody.velocity;
			vel.y = 0;
			Debug.Log(vel.magnitude > maxSpeed);

			if (vel.magnitude > maxSpeed)
			{
				/*
				float y = rigidbody.velocity.y;
				rigidbody.velocity = vel;
				rigidbody.velocity += Vector3.up * y;
				*/
			}
			else
			{
				rigidbody.velocity += (dir * acceleration * rigidbody.mass * Time.deltaTime); 
			}
		} 
	}
    
	//Rotates the body to face the direction you are moving
	void Turn(Vector3 dir)
	{
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), maxAngle * Time.deltaTime); 
	}

	//Does a jump check and jumps
	void Jump()
	{
		if (isGrounded())
		{
			rigidbody.AddForce(Vector3.up * jumpSpeed); 
		}
	}

	//Wrapper so it can be called without a value for dir 
	void Roll()
	{
		Roll(Vector3.down);
	}

	//The class which handels the roll
	void Roll(Vector3 dir)
	{
		if (rollCoolDownTimer <= Time.time)
		{
			if (dir != Vector3.down)
				rollDirection = dir.normalized;

			rolling = true;

			if (rolling && rollTimer <= rollTime)
			{
				maxSpeed = rollSpeed;
				rollTimer += Time.deltaTime;
				transform.position += rollDirection * rollSpeed * Time.deltaTime;
			} 
			else
			{
				rolling = false;
				rollTimer = 0;
				rollCoolDownTimer = Time.time + rollCoolDown;
			}
		}
	}

	//Makes sure you don't slip away on slopes
	void CounterSlopes()
	{
		RaycastHit hit;
		Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, ~LayerMask.GetMask("Floor"));
		if (hit.normal != Vector3.up && hit.normal != Vector3.zero)
		{
			Vector3 tangent = Vector3.Cross(hit.normal, Vector3.forward);
			if (rigidbody.velocity.magnitude != 0)
			{
				rigidbody.AddForce(Vector3.Dot(rigidbody.velocity, tangent) * tangent * rigidbody.mass * 1);
			}
		
		}
	}

	//Checks if the body is grounded
	bool isGrounded()
	{
		return Physics.Raycast(transform.position, Vector3.down, distToGround + 0.2f, ~LayerMask.GetMask("Floor")) && !rolling;
	}

	public Vector3 GetVelocity()
	{
		return direction * speed;
	}

	public void SetVelocity(Vector3 vel)
	{
		speed = vel.magnitude;
		direction = vel.normalized;
	}
}





















