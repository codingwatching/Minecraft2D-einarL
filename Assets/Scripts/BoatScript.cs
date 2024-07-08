using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatScript : MonoBehaviour
{

    private Rigidbody2D rb;
    private bool isFloating = false;
    private Transform steve;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = gameObject.name.Replace("(Clone)", "");
        rb = GetComponent<Rigidbody2D>();
        steve = GameObject.Find("SteveContainer").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFloating) checkIfFloating();
        else checkIfStoppedFloating();
    }

    // makes the player a child of the boat so that the player moves with the boat
    public void attatchPlayerToBoat()
    {
        if (steve == null) steve = GameObject.Find("SteveContainer").transform;

        steve.parent = transform;
	}
    // removes the player child so that the player no longer moves with the boat
    public void detachPlayer()
    {
        if (steve == null) steve = transform.Find("SteveContainer").transform;

        steve.parent = null;
	}


	private void checkIfFloating()
    {
        // Check for overlaps
        bool bottomCheck = Physics2D.OverlapCircle(transform.position, 0.01f, LayerMask.GetMask("Water"));
		bool topCheck = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y + 0.1f), 0.01f, LayerMask.GetMask("Water"));
		
        if(bottomCheck && !topCheck)
        {
            isFloating = true;
			rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }
        else if(bottomCheck && topCheck)
        {
            // float up
            rb.velocity = new Vector2(rb.velocity.x, 5);
        }

	}

    private void checkIfStoppedFloating()
    {
        if (!Physics2D.OverlapCircle(transform.position, 0.01f, LayerMask.GetMask("Water")))
        {
            isFloating = false;
			rb.constraints = RigidbodyConstraints2D.FreezeRotation;
		}
	}

}
