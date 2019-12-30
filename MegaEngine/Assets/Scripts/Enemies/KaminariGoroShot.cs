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

    // Protected Instance Variables
    protected Vector3 targetDirection;
    protected float lifeSpan = 3f;
    protected float damage = 10f;
    protected float speed = 150f;
    protected float timeStart;
    protected Vector3 moveVector;
    [SerializeField] protected float gravity = 118f;
    [SerializeField] protected float jumpAmount = 10.0f;
    public Transform target;
    protected float verticalVelocity;
    private SpriteRenderer spriteRenderer;


    #endregion


    #region MonoBehaviour

    /* Use this for initialization */
    protected void Start()
    {
        timeStart = Time.time;
        spriteRenderer = GetComponent<SpriteRenderer>();
        moveVector = (target.position - transform.position);
        moveVector.y = jumpAmount;
        verticalVelocity = jumpAmount;
    }

    /* Update is called once per frame */
    protected void Update()
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
	protected void OnTriggerEnter2D(Collider2D other) 
	{
		InflictDamage(other.gameObject);
	}
	
	// 
	protected void OnCollisionEnter2D(Collision2D collision) 
	{
		InflictDamage(collision.gameObject);
	}

    #endregion


    #region Protected Functions

    protected void ApplyGravity()
    {
        moveVector = new Vector3(moveVector.x, (moveVector.y - gravity * Time.deltaTime), moveVector.z);
    }

    // 
    protected void InflictDamage(GameObject objectHit)
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

