using UnityEngine;
using System.Collections;

public class KaminariGoroShot : MonoBehaviour
{
    #region Variables

    // Public Properties
    public Vector3 TargetDirection
    {
        get { return targetDirection; }
        set
        {
            targetDirection = value - transform.position;
            targetDirection.Normalize();
            //spriteRenderer.flipX = targetDirection == 1 ? true : false;
            //SetTextureScale();
        }
    }
    public float Speed { set { speed = value; } }

    // private Instance Variables
    private Vector3 targetDirection;
    private float lifeSpan = 3f;
    private float damage = 2;
    private float speed = 150f;
    private float timeStart;
    private Vector3 moveVector;
    [SerializeField] private float gravity = 118f;
    [SerializeField] private float jumpAmount = 10.0f;
    public Transform target;
    private float verticalVelocity;
    private SpriteRenderer spriteRenderer;


    #endregion


    #region MonoBehaviour

    /* Use this for initialization */
    private void Start()
    {
        timeStart = Time.time;
        spriteRenderer = GetComponent<SpriteRenderer>();
        moveVector = (target.position - transform.position);
        moveVector.y = jumpAmount;
        verticalVelocity = jumpAmount;
    }

    /* Update is called once per frame */
    private void Update()
    {
        verticalVelocity = moveVector.y;
        moveVector = (target.position - transform.position);
        moveVector.y = verticalVelocity;

        ApplyGravity();

        transform.position += moveVector * Time.deltaTime;
        
        // destroy object if lifespan is 0 or if it is off the screen
        if ((Time.time - timeStart >= lifeSpan)  || !spriteRenderer.isVisible)
		{
			transform.parent.gameObject.SendMessage("SetIsShooting", false);
			Destroy(gameObject);
		}
	}
	
	// Called when the Collider other enters the trigger.
	private void OnTriggerEnter2D(Collider2D other) 
	{
		InflictDamage(other.gameObject);
	}
	
	// 
	private void OnCollisionEnter2D(Collision2D collision) 
	{
		InflictDamage(collision.gameObject);
	}

    #endregion


    #region private Functions

    private void ApplyGravity()
    {
        moveVector = new Vector3(moveVector.x, (moveVector.y - gravity * Time.deltaTime), moveVector.z);
    }

    // 
    private void InflictDamage(GameObject objectHit)
	{
		if (objectHit.tag == "Player")
		{
			GameEngine.Player.TakeDamage(damage);
			transform.parent.gameObject.GetComponent<KaminariGoro>().SetIsShooting(false);
			Destroy(gameObject);
		}
	}

	#endregion
}

