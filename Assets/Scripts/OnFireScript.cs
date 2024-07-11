using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFireScript : MonoBehaviour
{

    private bool isPlayer; // is this the player or another entity
    private HealthbarScript healthbarScript; // this is only used if this is the player
    private Entity entityScript; // this is only used if this is an entity
    private Collider2D col;

    private bool hasStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        if (hasStarted) return;

        isPlayer = transform.parent.name.Equals("Steve");
        if (isPlayer) healthbarScript = GameObject.Find("Canvas").transform.Find("Healthbar").GetComponent<HealthbarScript>();
        else entityScript = GetComponentInParent<Entity>();
        col = GetComponentInParent<Collider2D>();
        hasStarted = true;
	}

	private void OnEnable()
	{
        Start();

        StartCoroutine(takeFireDamage());
		StartCoroutine(stopFire(Random.Range(8f, 15f)));
	}

	private IEnumerator takeFireDamage()
    {
        while (true)
        {
            if (isPlayer) healthbarScript.takeDamage(1, 0, false, false);
            else entityScript.takeDamage(1, transform.position.x, false);
            yield return new WaitForSeconds(1.7f);
        }
    }

    private IEnumerator stopFire(float seconds)
    {
        while (isTouchingFire())
        {
			yield return new WaitForSeconds(seconds);
		}
        gameObject.SetActive(false);
    }

    private bool isTouchingFire()
    {
		if (col == null)
		{
            Debug.Log("OnFireScript: collider is null");
			return false;
		}

		// IsTouchingLayers expects a bitmask, so convert the layer to a bitmask
		int fireLayerMask = 1 << LayerMask.NameToLayer("Fire");
		return col.IsTouchingLayers(fireLayerMask);
	}
}
