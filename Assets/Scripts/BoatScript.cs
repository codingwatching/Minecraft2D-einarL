using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatScript : MonoBehaviour, Interactable
{

    private Rigidbody2D rb;
    private bool isFloating = false;
    private Transform steve;
    private bool isPlayerAttatched = false;
    private Vector3 previousPosition;
    private float speed = 60f;
    private float maxSpeed = 10f;
    private MessageScript messageScript;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = gameObject.name.Replace("(Clone)", "");
        rb = GetComponent<Rigidbody2D>();
        steve = GameObject.Find("SteveContainer").transform;
        messageScript = GameObject.Find("Canvas").transform.Find("Message").GetComponent<MessageScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFloating) checkIfFloating();
        else checkIfStoppedFloating();
    }

    void FixedUpdate()
    {
		if (isPlayerAttatched)
		{
			movePlayerWithBoat();
			boatControls();
		}
	}

    private void boatControls()
    {
        if (Input.GetKey(KeyCode.D) && rb.velocity.magnitude < maxSpeed) rb.AddForce(Vector2.right * speed);
		if (Input.GetKey(KeyCode.A) && rb.velocity.magnitude < maxSpeed) rb.AddForce(Vector2.left * speed);

        if(Input.GetKey(KeyCode.LeftControl)) detachPlayer();
	}
    

	public void rightClick()
	{
        attatchPlayerToBoat();

	}

    public void mineBlock(ToolInstance heldTool)
    {
        if (isPlayerAttatched) return;

		// destroy boat
		GameObject itemToDrop = Resources.Load<GameObject>("Prefabs\\ItemContainer");
		GameObject itemObject = itemToDrop.transform.Find("Item").gameObject; // get item within itemContainer

		itemObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures\\ItemTextures\\Boat"); // change item texture 


		GameObject itemInstance = GameObject.Instantiate(itemToDrop, transform.position, Quaternion.identity);

		// Generate a random angle between 0 and 180 degrees
		float randomAngle = Random.Range(0f, 180f);

		// Convert the angle to a direction vector
		Vector2 direction = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));

		// Apply the force to the item instance
		itemInstance.GetComponent<Rigidbody2D>().AddForce(direction * Random.Range(1f, 3f), ForceMode2D.Impulse);

		Destroy(gameObject);
    }

	// makes the player a child of the boat so that the player moves with the boat
	public void attatchPlayerToBoat()
    {
        if (isPlayerAttatched) return;
        messageScript.displayMessage("Press Left Control to get off the boat");

		isPlayerAttatched = true;
        previousPosition = transform.position;

		if (steve == null) steve = GameObject.Find("SteveContainer").transform;
		steve.parent = transform;

		steve.GetComponent<PlayerControllerScript>().boatThatThePlayerIsIn = this;

		Rigidbody2D rb = steve.GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), steve.GetComponent<Collider2D>());
        hidePlayerLegs();

        if (steve.position.x < transform.position.x)
        {
            steve.position = new Vector2(transform.position.x - 0.1f, transform.position.y + 0.1f);
        }
        else
        {
            steve.position = new Vector2(transform.position.x + 0.1f, transform.position.y + 0.1f);
		}
	}
    // removes the player child so that the player no longer moves with the boat
    public void detachPlayer()
    {
		isPlayerAttatched = false;
		if (steve == null) steve = transform.Find("SteveContainer").transform;
        steve.parent = null;

		steve.GetComponent<PlayerControllerScript>().boatThatThePlayerIsIn = null;
		Rigidbody2D steveRB = steve.GetComponent<Rigidbody2D>();
		steveRB.constraints = RigidbodyConstraints2D.FreezeRotation;
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), steve.GetComponent<Collider2D>(), false);

        hidePlayerLegs(false);
        steve.position = new Vector2(transform.position.x, transform.position.y + 1);
	}

    private void movePlayerWithBoat()
    {
		Vector3 offset = previousPosition - transform.position;
		steve.transform.position = new Vector2(steve.transform.position.x - offset.x, steve.transform.position.y - offset.y);

		previousPosition = transform.position;
	}

    private void hidePlayerLegs(bool hide = true)
    {
        steve.Find("Steve").Find("Leg Front").gameObject.SetActive(!hide);
		steve.Find("Steve").Find("Leg Back").gameObject.SetActive(!hide);
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
		bool bottomCheck = Physics2D.OverlapCircle(transform.position, 0.01f, LayerMask.GetMask("Water"));
		bool topCheck = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y + 0.1f), 0.01f, LayerMask.GetMask("Water"));

		if (bottomCheck && topCheck || !bottomCheck && !topCheck || !bottomCheck && topCheck)
        {
            isFloating = false;
			rb.constraints = RigidbodyConstraints2D.FreezeRotation;
		}
	}
}
