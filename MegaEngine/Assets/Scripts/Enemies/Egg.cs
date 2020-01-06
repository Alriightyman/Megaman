using UnityEngine;
using System.Collections;

// needed for collision with platform
//[RequireComponent(typeof(Rigidbody2D))]
public class Egg : MonoBehaviour 
{
    #region Variables

    // Unity Editor Variables
    public GameObject powerup;
	[SerializeField] private GameObject copipi;
    [SerializeField] private float speed = 7.0f;
    [SerializeField] private float velSlower = 7.0f;
    [SerializeField] private float copipiSpeed = 75.0f;

    // private Instance Variables
    private bool falling = false;
    private float lifeSpan = 5.0f;
	private float lifeTimer;
	private float xVel = 0.0f;
	
	private float damage = 4f;

    private SpriteRenderer spriteRenderer;

    #endregion


    #region MonoBehaviour

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Called when the Collider other enters the trigger.
    private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			GameEngine.Player.TakeDamage(damage);
		}
		
		// If we are crashing into a platform...
		else if (other.tag == "platform")
		{
			float dist = 25f;
			bool goLeft = (GameEngine.Player.transform.position.x < transform.position.x);
			CreateBird(transform.position, goLeft);
			//CreateBird(transform.position + Vector3.up, goLeft);
			//CreateBird(transform.position + Vector3.down, goLeft);
			//CreateBird(transform.position + Vector3.left, goLeft);
			//CreateBird(transform.position + Vector3.right, goLeft);
			
			//CreateBird(transform.position + Vector3.up * dist + Vector3.left, goLeft);
			//CreateBird(transform.position + Vector3.up * dist + Vector3.right, goLeft);
			CreateBird(transform.position + Vector3.down * dist + Vector3.left, goLeft);
			//CreateBird(transform.position + Vector3.down * dist + Vector3.right, goLeft);
			
			//CreateBird(transform.position + Vector3.up * (dist/2.0f) + Vector3.left * (dist/2.0f), goLeft);
			CreateBird(transform.position + Vector3.up * (dist/2.0f) + Vector3.right * (dist/2.0f), goLeft);
			//CreateBird(transform.position + Vector3.down * (dist/2.0f) + Vector3.left * (dist/2.0f), goLeft);
			//CreateBird(transform.position + Vector3.down * (dist/2.0f) + Vector3.right * (dist/2.0f), goLeft);
			
			//CreateBird(transform.position + Vector3.up * (dist/3.0f) + Vector3.left * (dist/3.0f), goLeft);
			//CreateBird(transform.position + Vector3.up * (dist/3.0f) + Vector3.right * (dist/3.0f), goLeft);
			//CreateBird(transform.position + Vector3.down * (dist/3.0f) + Vector3.left * (dist/3.0f), goLeft);
			CreateBird(transform.position + Vector3.down * (dist/3.0f) + Vector3.right * (dist/3.0f), goLeft);
			
			Destroy(gameObject);
		}
	}

	// Update is called once per frame
	private void Update () 
	{
		if (falling == true)
		{
			transform.position += ((Vector3.down * speed) + (Vector3.left * xVel))  * Time.deltaTime;
			if (xVel > 0.0f) xVel -= velSlower * Time.deltaTime;
			if (xVel < 0.0f) xVel = 0.0f;
			
			if (Time.time - lifeTimer >= lifeSpan || spriteRenderer.isVisible == false)
			{
				Destroy(gameObject);	
			}
		}
	}

	#endregion

	#region private Functions

	// 
	private void CreateBird(Vector3 pos, bool goLeft)
	{
        GameObject littleBirdRobot = Instantiate(copipi, pos, transform.rotation);
		littleBirdRobot.GetComponent<Copipi>().Attack(goLeft, copipiSpeed + Random.Range(0.0f, 10.0f));
		Physics2D.IgnoreCollision(littleBirdRobot.GetComponent<Collider2D>(), GetComponent<Collider2D>());
	}

	#endregion
	
	#region Public Functions

	// 
	public void TakeDamage(float dam)
	{
		GameEngine.SoundManager.Play(AirmanLevelSounds.BOSS_HURTING);
		Destroy (gameObject);
	}
	
	// 
	public void ReleaseEgg(float xVelocity)
	{
        if (gameObject != null)
        {
            xVel = xVelocity;
            transform.parent = null;
            falling = true;
            lifeTimer = Time.time;
            tag = "shootable";
            gameObject.layer = 0;
        }
	}

    private void OnDestroy()
    {
        Instantiate(powerup, transform);
    }

    #endregion
}
