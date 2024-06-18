using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.FilePathAttribute;

public class Skeleton : Mob
{

	private float canShootPlayerWithin = 15f;
	private Coroutine damageCoroutine = null;
	private Animator bowAnim;
	private Transform frontArm;
	private Transform backArm;
	private Transform head;
	private GameObject arrowPrefab;
	private Transform groundCheck;
	private float arrowForce = 20f;

	private new void Start()
	{
		base.Start();
		bowAnim = transform.Find("Skeleton Child").Find("Arm Front Parent").Find("Arm Front").Find("Bow").GetComponent<Animator>();
		frontArm = transform.Find("Skeleton Child").Find("Arm Front Parent");
		backArm = transform.Find("Skeleton Child").Find("Arm Back Parent");
		head = transform.Find("Skeleton Child").Find("Head");
		groundCheck = transform.Find("Skeleton Child").Find("GroundCheck");
		arrowPrefab = Resources.Load<GameObject>("Prefabs\\Throwables\\Arrow");
	}


	public override void initializeEntity()
	{
		higherBlockCheck = transform.Find("Skeleton Child").transform.Find("HigherBlockCheck");
		lowerBlockCheck = transform.Find("Skeleton Child").transform.Find("LowerBlockCheck");
		rb = GetComponent<Rigidbody2D>();
		anim = transform.Find("Skeleton Child").GetComponent<Animator>();
		sayAudioSource = transform.Find("Skeleton Child").GetComponent<AudioSource>();
		playerTransform = GameObject.Find("SteveContainer").transform;

		walkingCoroutine = StartCoroutine(decideIfWalk());
	}

	protected override void huntPlayer()
	{
		if (isDamageCoroutineRunning == false && canHurtPlayer() && isGrounded())
		{
			damageCoroutine = StartCoroutine(damagePlayer());
			anim.SetBool("isWalking", false);

		}
		if (isDamageCoroutineRunning)
		{
			facePlayer();
			pointBowTowardsPlayer();
			return;
		}

		// if we reach this point then we want to move to the player
		facePlayer(); // turn towards player
		bool isPlayerOnRightSide = playerPos.position.x > transform.position.x;
		if (isPlayerOnRightSide) makeDirectionRight(); // make the direction variable be to the right
		else makeDirectionLeft();

		if (!isPathBlocked())
		{
			rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
			anim.SetBool("isWalking", true);
		}
		else // if path is blocked
		{
			anim.SetBool("isWalking", false);
		}

		if (isBlockInPath()) jump();
	}

	private void pointBowTowardsPlayer()
	{
		Vector3 target = playerPos.position;

		bool facingRight = target.x > transform.position.x;
		Transform arm = facingRight ? frontArm : backArm;

		// now rotate arm to point towards mouse position
		Vector3 diff = target - arm.position;
		diff.Normalize();

		float zRotation = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

		if (!facingRight)
		{
			backArm.rotation = Quaternion.Euler(-180f, 0f, -zRotation + 90f);
			frontArm.rotation = Quaternion.Euler(-180f, 0f, -zRotation + 80f);
			head.rotation = Quaternion.Euler(-180f, 0f, -zRotation);
		}
		else
		{
			frontArm.rotation = Quaternion.Euler(0f, 0f, zRotation + 90f);
			backArm.rotation = Quaternion.Euler(0f, 0f, zRotation + 80f);
			head.rotation = Quaternion.Euler(0f, 0f, zRotation);
		}
	}

	private void stopPointingBowTowardsPlayer()
	{
		frontArm.rotation = Quaternion.Euler(0f, 0f, 0f);
		backArm.rotation = Quaternion.Euler(0f, 0f, 0f);
		head.rotation = Quaternion.Euler(0f, 180f, 0f);
	}

	float calculateShootingAngle(Vector2 targetDir, float force)
	{
		float g = Mathf.Abs(Physics2D.gravity.y);
		float distance = targetDir.x;
		float yOffset = targetDir.y;

		float discriminant = (force * force * force * force) - (g * (g * distance * distance + 2 * yOffset * force * force));
		if (discriminant < 0)
		{
			Debug.Log("Target is out of range");
			return 0;
		}

		float sqrtDiscriminant = Mathf.Sqrt(discriminant);

		//float angle1 = Mathf.Atan2(force * force + sqrtDiscriminant, g * distance) * Mathf.Rad2Deg;
		float angle2 = Mathf.Atan2(force * force - sqrtDiscriminant, g * distance) * Mathf.Rad2Deg;
		//if (angle1 < angle2) Debug.Log("angle1 chosen");
		//else Debug.Log("angle2 chosen");
		// Choose the lower angle for a more direct shot
		//return Mathf.Min(angle1, angle2);
		return angle2;
	}


	private void shootArrow()
	{
		// Calculate the required angle
		Vector2 direction = playerPos.position - transform.position;
		float angle = calculateShootingAngle(direction, arrowForce);

		// Instantiate the arrow at the skeleton's position
		GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
		arrow.GetComponent<ArrowScript>().EnableCollider(GetComponent<CapsuleCollider2D>());

		Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();

		float radians = angle * Mathf.Deg2Rad;
		Vector2 force = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * arrowForce;
		rb.AddForce(force, ForceMode2D.Impulse);
	}

	// checks if arrows can reach player
	protected override bool canHurtPlayer()
	{
		return (raycast(transform.position, playerPos.position) || raycast(new Vector2(transform.position.x, transform.position.y + 0.4f), new Vector2(playerPos.position.x, playerPos.position.y + 0.4f))) && !anim.GetBool("isDead");
	}

	protected override IEnumerator damagePlayer()
	{
		StartCoroutine(checkIfStopDamageCoroutine());
		isDamageCoroutineRunning = true;
		while (true)
		{
			bowAnim.SetBool("isDrawingBackBow", true);
			yield return new WaitForSeconds(2f);
			if (healthbarScript.getHealth() > 0)
			{
				shootArrow();
				bowAnim.SetBool("isDrawingBackBow", false);
				yield return new WaitForSeconds(0.5f);
			}
			else break;
		}
		isDamageCoroutineRunning = false;
	}

	private IEnumerator checkIfStopDamageCoroutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.3f);
			if(!canHurtPlayer() && isDamageCoroutineRunning)
			{
				StopCoroutine(damageCoroutine);
				isDamageCoroutineRunning = false;
				damageCoroutine = null;
				stopPointingBowTowardsPlayer();
				break;
			}
			else if (!isDamageCoroutineRunning)
			{
				damageCoroutine = null;
				stopPointingBowTowardsPlayer();
				break;
			}
		}
	}

	/**
	 * returns true if the raycast can get from start to end without hitting anything
	 */
	private bool raycast(Vector2 start, Vector2 end)
	{
		// Calculate direction and distance
		Vector2 direction = end - start;
		float distance = Vector2.Distance(start, end);
		if (distance > canShootPlayerWithin) return false; // if player is out of range
		distance = Mathf.Min(distance, canShootPlayerWithin);
		// Cast the ray
		RaycastHit2D hit = Physics2D.Raycast(start, direction, distance, LayerMask.GetMask("Default") | LayerMask.GetMask("Tilemap"));

		return !hit;
	}

	protected override void dropLoot()
	{
		//dropItem("RottenFlesh");
	}

	public override void die()
	{
		base.die();
		CapsuleCollider2D collider = GetComponent<CapsuleCollider2D>();
		collider.size = new Vector2 (0.0979299f, 0.12f);
	}

	private bool isGrounded()
	{
		Collider2D[] results = new Collider2D[1];

		ContactFilter2D contactFilter = new ContactFilter2D();
		contactFilter.layerMask = LayerMask.GetMask("Default") | LayerMask.GetMask("Tilemap");
		contactFilter.useLayerMask = true;

		int count = Physics2D.OverlapCircle(groundCheck.position, 0.05f, contactFilter, results);

		return count > 0;
	}
}
