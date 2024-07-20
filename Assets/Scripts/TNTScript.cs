using UnityEngine;

public class TNTScript : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
    {
	}

    // Update is called once per frame
    void Update()
    {
        
    }
    // ignite the TNT
    public void ignite()
    {
		if (gameObject.layer == LayerMask.NameToLayer("TNT")) return; // if already ignited, then return
		SpawningChunkData.updateChunkData(transform.position.x, transform.position.y, 0, LayerMask.LayerToName(gameObject.layer));
		Destroy(GetComponent<BlockScript>());
        gameObject.AddComponent<Rigidbody2D>();
		AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds\\Random\\fuse"), transform.position);
		GetComponent<Animator>().Play("ignite");
		gameObject.layer = LayerMask.NameToLayer("TNT");
	}

	public void blowUp()
	{
		new BlowUpScript().blowUp(gameObject, transform.position);
	}
}
