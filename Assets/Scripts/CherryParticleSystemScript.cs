using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryParticleSystemScript : MonoBehaviour
{
	ParticleSystem cherryParticleSystem;

    // Start is called before the first frame update
    void Start()
    {
		IEnumerator startAfterSomeTime()
		{
			yield return new WaitForSeconds(0.5f);
			destroyIfBelowIsCherryLeaf();
		}
		StartCoroutine(startAfterSomeTime());
    }

    private void destroyIfBelowIsCherryLeaf()
    {
		ContactFilter2D filter = new ContactFilter2D();
		filter.SetLayerMask(LayerMask.GetMask("Default") | LayerMask.GetMask("FrontBackground") | LayerMask.GetMask("BackBackground"));
		// Create a list to store the results
		List<Collider2D> results = new List<Collider2D>();

		// Check for overlaps
		Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y - 1f), 0.1f, filter, results);

		foreach (Collider2D collider in results)
		{
			if (collider.gameObject.name.StartsWith("LeavesCherry"))
			{
				Destroy(gameObject); // destroy this particle system
				return;
			}
		}
	}

	public void stopParticleSystem()
	{
		cherryParticleSystem = GetComponent<ParticleSystem>();
		cherryParticleSystem.Stop();
		StartCoroutine(CheckIfAliveAndDestroy());
	}

	private IEnumerator CheckIfAliveAndDestroy()
	{
		// Wait until all particles are dead
		while (cherryParticleSystem.IsAlive(true))
		{
			yield return new WaitForSeconds(5f);
		}
		// Now safe to destroy the GameObject
		Destroy(gameObject);
	}
}
