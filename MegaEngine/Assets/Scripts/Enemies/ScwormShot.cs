using UnityEngine;
using System.Collections;
using Prime31;

public class ScwormShot : MonoBehaviour 
{
	#region Variables

	// private Instance Variables
	private int texDir = 1;
	private bool keepAttacking = false;
	private float texChangeDelay = 0.1f;
	private float texTimer;
	[SerializeField] private float gravity = 118f;
	[SerializeField] private float jumpAmount = 10.0f;
	private float verticalVelocity;
	private float lifeSpan = 3.0f;
	private float lifeTimer;
	private Vector3 attackPos;
	private Vector3 moveVector;
    private float damage = 2f;
    CharacterController2D characterController;
    #endregion


    #region MonoBehaviour
    private void Awake()
    {
        characterController = GetComponent<CharacterController2D>();
    }
    // Use this for initialization
    private void Start ()
	{
		lifeTimer = texTimer = Time.time;
	}
	
	// Update is called once per frame
	private void Update () 
	{
		if (keepAttacking == true)
		{
			verticalVelocity = moveVector.y;
			moveVector = (attackPos - transform.position);
			moveVector.y = verticalVelocity;
			
			ApplyGravity();

            characterController.move(moveVector * Time.deltaTime);

        }

        if (Time.time - texTimer >= texChangeDelay)
		{
			texTimer = Time.time;
			texDir *= -1;
            var rend = GetComponent<SpriteRenderer>();
            rend.flipX = !rend.flipX;
		}
		
		// Time to kill the shot?
		if (Time.time - lifeTimer >= lifeSpan)
		{
            Destroy(gameObject);
		}
	}

	// Called when the Collider other enters the trigger.
	private void OnTriggerEnter2D(Collider2D collider) 
	{
		if (collider.tag == "Player")
		{
			collider.gameObject.GetComponent<Player>().TakeDamage(damage);
			Destroy(gameObject);
		}
	}

	// 
	private void OnCollisionEnter2D(Collision2D collision) 
	{
		if (collision.gameObject.tag == "Player")
		{
			collision.gameObject.GetComponent<Player>().TakeDamage(damage);
			Destroy(gameObject);
		}
		
		if (collision.transform.position.y < transform.position.y)
		{
			keepAttacking = false;
		}
	}

    private void OnDestroy()
    {
        transform.parent.GetComponent<Scworm>().DecrementShotCount();
    }

    #endregion


    #region private Functions

    // 
    private void ApplyGravity()
	{
		moveVector = new Vector3(moveVector.x, (moveVector.y - gravity * Time.deltaTime), moveVector.z);
	}

	#endregion
	
	
	#region Public Functions

	// 
	public void Attack(Vector3 playerPos)
	{
		attackPos = playerPos;
		moveVector = (attackPos - transform.position);
		moveVector.y = jumpAmount;
		verticalVelocity = jumpAmount;
		keepAttacking = true;
	}

	#endregion
}
