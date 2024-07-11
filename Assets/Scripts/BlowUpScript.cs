using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.Tilemaps;
// TNT and Creeper use this script to blow up
public class BlowUpScript
{

	private float blastRadius = 4f;
	private float blastBaseDamage = 45f; // damage taken when really close to TNT, this diminishes the further you are away from it
	private float blastBaseForce = 20f; // how much force is used to shoot ignited tnt
	private ParticleSystem explosionParticleSystem;
	private Tilemap tilemap;
	private spawnChunkScript scScript;

	public BlowUpScript() {
		explosionParticleSystem = Resources.Load<ParticleSystem>("Particle Systems\\Explosion Particle System");
		tilemap = GameObject.Find("Grid").transform.Find("Tilemap").GetComponent<Tilemap>();
		scScript = GameObject.Find("Main Camera").GetComponent<spawnChunkScript>();
	}

	public void blowUp(GameObject blowingUpObject, Vector3 position)
	{
		spawnGameObjectsInsteadOfTiles(position);

		int explosionSound = Random.Range(1, 5);
		AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>($"Sounds\\Random\\explode{explosionSound}"), position);

		GameObject.Instantiate(explosionParticleSystem, position, Quaternion.identity);
		List<Collider2D> withinRadius = new List<Collider2D>();

		ContactFilter2D filter = new ContactFilter2D();
		filter.SetLayerMask(LayerMask.GetMask("Player") | LayerMask.GetMask("Entity") | LayerMask.GetMask("Default") | LayerMask.GetMask("FrontBackground") | LayerMask.GetMask("BackBackground") | LayerMask.GetMask("Item") | LayerMask.GetMask("Movable") | LayerMask.GetMask("TNT"));

		// Check for overlaps
		Physics2D.OverlapCircle(position, blastRadius, filter, withinRadius);
		foreach (Collider2D collider in withinRadius)
		{
			if (collider.gameObject.layer == LayerMask.NameToLayer("Entity") || collider.gameObject.layer == LayerMask.NameToLayer("Player"))
			{
				if (ReferenceEquals(collider.gameObject, blowingUpObject)) continue; // Dont let this creeper take damage

				float distance = Vector2.Distance(position, collider.transform.position);

				float divisor = Mathf.Max(1f, distance - 0.5f);
				float damageTaken = blastBaseDamage / divisor;

				if (collider.gameObject.layer == LayerMask.NameToLayer("Entity")) collider.GetComponent<Entity>().takeDamage(damageTaken, position.x);
				else GameObject.Find("Canvas").transform.Find("Healthbar").GetComponent<HealthbarScript>().takeDamage((int)damageTaken, position.x);
			}
			else if (collider.gameObject.layer == LayerMask.NameToLayer("Default") || collider.gameObject.layer == LayerMask.NameToLayer("FrontBackground") || collider.gameObject.layer == LayerMask.NameToLayer("BackBackground"))
			{
				if (collider.name.Equals("TNT")) collider.GetComponent<TNTScript>().ignite();
				// if its a block gameobject
				else collider.GetComponent<BlockScript>()?.breakBlock(false);
			}
			else if (collider.gameObject.layer == LayerMask.NameToLayer("Item"))
			{
				GameObject.Destroy(collider.gameObject);
			}
			else if (collider.gameObject.layer == LayerMask.NameToLayer("Movable"))
			{
				collider.GetComponent<BoatScript>().mineBlock(null);
			}
			else if (collider.gameObject.layer == LayerMask.NameToLayer("TNT"))
			{
				// Get the Rigidbody2D component of the colliding GameObject
				Rigidbody2D rb = collider.gameObject.GetComponent<Rigidbody2D>();

				if (rb != null)
				{
					// Calculate the direction from the current GameObject to the collider's GameObject
					Vector2 direction = (collider.transform.position - position).normalized;

					float distance = Vector2.Distance(position, collider.transform.position);

					float divisor = Mathf.Max(1f, distance - 0.5f);
					float blastForce = blastBaseForce / divisor;

					// Apply the velocity in the calculated direction, multiplied by the force multiplier
					rb.velocity = direction * blastForce;
				}
			}
		}

		GameObject.Destroy(blowingUpObject);
	}

	private void spawnGameObjectsInsteadOfTiles(Vector2 center)
	{
		// Convert center position from world to cell coordinates
		Vector3Int centerCell = tilemap.WorldToCell(center);

		// Calculate the bounds of the search area
		int minX = centerCell.x - Mathf.CeilToInt(blastRadius);
		int maxX = centerCell.x + Mathf.CeilToInt(blastRadius);
		int minY = centerCell.y - Mathf.CeilToInt(blastRadius);
		int maxY = centerCell.y + Mathf.CeilToInt(blastRadius);

		// Iterate over each potential tile position
		for (int x = minX; x <= maxX; x++)
		{
			for (int y = minY; y <= maxY; y++)
			{
				Vector3Int tilePos = new Vector3Int(x, y, centerCell.z);
				Vector2 tileWorldPos = tilemap.CellToWorld(tilePos) + tilemap.cellSize / 2;

				// Check if the tile is within the radius
				if (Vector2.Distance(center, tileWorldPos) <= blastRadius)
				{
					TileBase tile = tilemap.GetTile(tilePos);
					if (tile != null)
					{
						scScript.spawnGameObjectInsteadOfTile(tile, tilePos); // place gameObject at tiles position
						tilemap.SetTile(tilePos, null); // remove tile
					}
				}
			}
		}
	}
}
