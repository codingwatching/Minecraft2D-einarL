using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FireScript : MonoBehaviour
{
    private bool burnParent; // does this fire burn the block it is attatched to?
	private spawnChunkScript scScript;
	private Tilemap tilemap;

    // Start is called before the first frame update
    void Start()
    {
		scScript = GameObject.Find("Main Camera").GetComponent<spawnChunkScript>();
		tilemap = GameObject.Find("Grid").transform.Find("Tilemap").GetComponent<Tilemap>();
        burnParent = FrontBackgroundBlocks.isBurnable(transform.parent?.name);
        StartCoroutine(destroyFire());
        if (burnParent) StartCoroutine(spread());
		AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds\\Random\\fire"), transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator destroyFire()
    {
        yield return new WaitForSeconds(Random.Range(10f,30f)); // fire destroys after 10 to 30 seconds
		if(transform.parent != null )SpawningChunkData.addOrRemoveFireBlock(transform.position.x, transform.position.y, transform.position.y > transform.parent.position.y ? 1 : 0, false);
		else SpawningChunkData.addOrRemoveFireBlock(transform.position.x, transform.position.y, 0, false);
		if (burnParent)
		{
			transform.parent.GetComponent<BlockScript>().checkSurroundingBlocks();
			SpawningChunkData.updateChunkData(transform.parent.position.x, transform.parent.position.y, 0, LayerMask.LayerToName(transform.parent.gameObject.layer));
			Destroy(transform.parent.gameObject);
		}
		else Destroy(gameObject);
    }

    private IEnumerator spread()
    {
		List<int[]> blocksAround = new List<int[]>() {
				new int[] {0, 1},
				new int[] {1, 1},
				new int[] {1, 0},
				new int[] {1, -1},
				new int[] {0, -1},
				new int[] {-1, -1},
				new int[] {-1, 0},
				new int[] {-1, 1},
		};
		System.Random rand = new System.Random();
		while (true)
        {
			yield return new WaitForSeconds(Random.Range(2f,5f));
			foreach (int[] b in blocksAround)
			{
				if(rand.NextDouble() < 0.1f && !isFireAtPosition(new Vector2(transform.position.x + b[0], transform.position.y + b[1])))
				{
					Vector2 firePos = new Vector2(transform.position.x + b[0], transform.position.y + b[1]);
					GameObject block = getBlock(firePos);
					if(block != null && FrontBackgroundBlocks.isBurnable(block.name))
					{
						GameObject fireInstance = Instantiate(BlockHashtable.getBlockByID(66), firePos, Quaternion.identity);
						fireInstance.transform.parent = block.transform;

						SpawningChunkData.addOrRemoveFireBlock(firePos.x, firePos.y, 0);
					}
				}
			}
        }
    }

	private bool isFireAtPosition(Vector2 blockPos)
	{
		return Physics2D.OverlapCircle(blockPos, 0.0001f, LayerMask.GetMask("Fire"));
	}

	private GameObject getBlock(Vector2 blockPos, bool firstTry = true)
	{
		ContactFilter2D filter = new ContactFilter2D();
		filter.SetLayerMask(LayerMask.GetMask("Default") | LayerMask.GetMask("FrontBackground") | LayerMask.GetMask("BackBackground"));

		// Create a list to store the results
		List<Collider2D> results = new List<Collider2D>();

		// Check for overlaps
		Physics2D.OverlapCircle(blockPos, 0.0001f, filter, results);

		GameObject objToReturn = null;
		foreach (Collider2D collider in results) // return the frontmost block
		{
			if (collider.gameObject.layer == LayerMask.NameToLayer("FrontBackground")) return collider.gameObject;
			if (collider.gameObject.layer == LayerMask.NameToLayer("Default")) objToReturn = collider.gameObject;
			else if (objToReturn == null && collider.gameObject.layer == LayerMask.NameToLayer("BackBackground")) objToReturn = collider.gameObject;
		}
		if (objToReturn != null) return objToReturn;
		if (!firstTry) return null;
		// if we reach here, then there is no gameobject at this position (maybe a tile though)
		Vector3Int tilePos = tilemap.WorldToCell(blockPos);
		TileBase tile = tilemap.GetTile(tilePos);
		if (tile != null)
		{
			scScript.spawnGameObjectInsteadOfTile(tile, tilePos); // place gameObject at tiles position
			tilemap.SetTile(tilePos, null); // remove tile
			return getBlock(blockPos, false);
		}
		return null;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Entity") || collision.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			// Find the child GameObject named "Fire" in the entire hierarchy
			OnFireScript fire = collision.gameObject.GetComponentInChildren<OnFireScript>(true);

			if (fire != null)
			{
				// Enable the "Fire" GameObject
				fire.gameObject.SetActive(true);
			}
			else
			{
				Debug.LogWarning("Fire GameObject not found in children of " + collision.gameObject.name);
			}
		}
	}
}
