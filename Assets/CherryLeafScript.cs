using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryLeafScript : MonoBehaviour
{

    /**
     * this runs before this leaf is destroyed, this sets the particle system parent to be null so that
     * the particle system wont be destroyed with the block but instead finishes the particles and then destroys
     * 
     * i cant use OnDestroy() because the particle system will be null
     */
	public void runBeforeDestroyed()
	{
        GameObject cherryParticleSystemObject = transform.Find("Cherry Particle System")?.gameObject;
		if(cherryParticleSystemObject != null) {
			cherryParticleSystemObject.transform.parent = null;
			cherryParticleSystemObject.GetComponent<CherryParticleSystemScript>().stopParticleSystem();
		}

		// check if above is leaf and then put a ps on it
		addParticleSystemOnAboveCherryLeaf();
	}

	private void addParticleSystemOnAboveCherryLeaf()
	{
		ContactFilter2D filter = new ContactFilter2D();
		filter.SetLayerMask(LayerMask.GetMask("Default") | LayerMask.GetMask("FrontBackground") | LayerMask.GetMask("BackBackground"));
		// Create a list to store the results
		List<Collider2D> results = new List<Collider2D>();

		// Check for overlaps
		Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y + 1f), 0.1f, filter, results);

		foreach (Collider2D collider in results)
		{
			if (collider.gameObject.name.StartsWith("LeavesCherry"))
			{
				ParticleSystem cherryPS = Instantiate(Resources.Load<ParticleSystem>("Particle Systems\\Cherry Particle System"));
				cherryPS.name = "Cherry Particle System";
				cherryPS.transform.parent = collider.gameObject.transform;
				cherryPS.transform.localPosition = new Vector2(0.22f, -0.22f);
				return;
			}
		}
	}
}
