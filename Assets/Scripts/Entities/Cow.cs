using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : Animal
{

	private AudioClip[] hurtSounds = new AudioClip[3];

	new void Start()
	{
		saySounds = new AudioClip[4];
		health = 15;
		base.Start();
		//jumpPower = 11f;
		initializeEntity();
		initializeAudio();
		StartCoroutine(decideIfMakeNoise());
	}

	public override void takeDamage(float damage, float playerXPos)
	{
		if (anim.GetBool("isDead")) return;
		base.takeDamage(damage, playerXPos);
		makeHurtNoise();
		if (health > 0)
		{
			// code to start running:
			if (transform.position.x < playerXPos) // if the animal is on the left side of the player
			{
				direction = direction = new Vector3(-1, 0, 0); // run left
			}
			else direction = new Vector3(1, 0, 0); // run right
			faceDirection();

			isRunning = true;
			isWalking = true;
			anim.SetFloat("movementSpeed", 1.4f); // make animation faster
			anim.SetBool("isWalking", true);
			if (runningChangeDirectionCoroutine == null) runningChangeDirectionCoroutine = StartCoroutine(decideIfChangeDirectionWhenRunning());
			speed = runSpeed;
		}
		else
		{
			isRunning = false; // if dead

			GetComponent<Collider2D>().offset = deadColliderSize;
		}
	}

	private void makeHurtNoise()
	{
		var random = new System.Random();
		int randIndex = random.Next(hurtSounds.Length);
		AudioClip randClip = hurtSounds[randIndex];
		sayAudioSource.clip = randClip;
		sayAudioSource.Play();
	}

	protected override void dropLoot()
	{
		dropItem("Leather");
		dropItem("SteakRaw");
	}


	/**
	 * checks if there is are two blocks in front of the animal, which the animal can't jump over
	 */
	public override bool isPathBlocked()
	{
		return Physics2D.OverlapCircle(higherBlockCheck.position, 0.05f, LayerMask.GetMask("Default") | LayerMask.GetMask("Tilemap")) && Physics2D.OverlapCircle(lowerBlockCheck.position, 0.05f, LayerMask.GetMask("Default") | LayerMask.GetMask("Tilemap"));
	}

	public override void initializeAudio()
	{
		gameObject.name = gameObject.name.Replace("(Clone)", "").Trim(); // remove (Clone) from object name

		for (int i = 0; i < saySounds.Length; i++)
		{
			saySounds[i] = Resources.Load<AudioClip>($"Sounds\\Entities\\{gameObject.name}\\say{i + 1}");
		}

		for (int i = 0; i < hurtSounds.Length; i++)
		{
			hurtSounds[i] = Resources.Load<AudioClip>($"Sounds\\Entities\\{gameObject.name}\\hurt{i + 1}");
		}
	}
}
