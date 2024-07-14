using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Creeper : Mob
{
	private AudioClip fuseSound; // sound when the creeper is going to explode

	private new void Start()
	{
		saySounds = new AudioClip[4];
		hurtSounds = new AudioClip[0];
		fuseSound = Resources.Load<AudioClip>("Sounds\\Random\\fuse");
		base.Start();
	}

	protected override void huntPlayer()
	{
		if (playerControllerScript.isInCreativeMode()) stopHunting();
		float playerDistanceX = Mathf.Abs(playerTransform.position.x - transform.position.x);
		if (canHurtPlayer())
		{
			if (isDamageCoroutineRunning == false) StartCoroutine(damagePlayer());
			facePlayer();
			anim.SetBool("isWalking", false);
			return;
		}
		if (isDamageCoroutineRunning) return;
		// if we reach this point then we want to move to the player
		facePlayer(); // turn towards player
		bool isPlayerOnRightSide = playerTransform.position.x > transform.position.x;
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

	protected override bool canHurtPlayer()
	{
		float playerDistanceX = Mathf.Abs(playerTransform.position.x - transform.position.x);
		float playerDistanceY = Mathf.Abs(playerTransform.position.y - transform.position.y);
		return playerDistanceX <= canHurtPlayerWithin && playerDistanceY <= 3f && !anim.GetBool("isDead");
	}

	protected override IEnumerator damagePlayer()
	{
		isDamageCoroutineRunning = true;
		int blowUpCounter = 0;
		playFuseSound();
		anim.SetBool("isBlowingUp", true);
		while (true)
		{
			float playerDistanceX = Mathf.Abs(playerTransform.position.x - transform.position.x);
			if (playerDistanceX > 3f && blowUpCounter < 2) break;
			if(blowUpCounter >= 2)
			{
				blowUp();
				break;
			}
			yield return new WaitForSeconds(0.7f);
			blowUpCounter++;
		}
		isDamageCoroutineRunning = false;
		anim.SetBool("isBlowingUp", false);
	}

	private void blowUp()
	{
		new BlowUpScript().blowUp(gameObject, transform.position);
	}

	protected override void dropLoot()
	{
		int drops = Random.Range(1, 4);

		for(int _ = 0; _ < drops; _++)
		{
			dropItem("Gunpowder");
		}
	}

	private void playFuseSound()
	{
		AudioSource.PlayClipAtPoint(fuseSound, transform.position);
	}

	protected override void makeHurtNoise()
	{
		var random = new System.Random();
		int randIndex = random.Next(saySounds.Length);
		AudioClip randClip = saySounds[randIndex];
		sayAudioSource.clip = randClip;
		sayAudioSource.Play();
	}

	public override void die()
	{
		base.die();
		CapsuleCollider2D collider = GetComponent<CapsuleCollider2D>();
		collider.size = new Vector2 (0.0979299f, 0.12f);
	}
}
