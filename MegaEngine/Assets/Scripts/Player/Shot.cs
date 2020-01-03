using UnityEngine;
using System.Collections;

public class Shot : MonoBehaviour 
{
	#region Variables

	// Properties
	public Vector3 VelocityDirection 	{ get; set; }
	public float ShotSpeed 				{ get; set; }

    public Shooting parent { get; set; }

	// Protected Instance Variables
	protected float lifeSpan = 3f;
	protected int damage = 10;
	//protected float timeStart;

	#endregion
	
	
	#region MonoBehaviour

	// Use this for initialization
	protected void Start () 
	{
        Destroy(gameObject, lifeSpan);
	}

	// Update is called once per frame
	protected void Update () 
	{
		
		GetComponent<Rigidbody2D>().velocity = VelocityDirection * ShotSpeed;
	}

	// Called when the Collider other enters the trigger.
	protected void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.tag == "shootable")
		{
			InflictDamage(other.gameObject);
		}
		
		else if (other.gameObject.layer == (int)CollisionLayer.EnemyRobot && other.tag == "unshootable")
		{
			Destroy(gameObject);	
		}
		
		else if (other.gameObject.layer == (int)CollisionLayer.Defualt && other.tag == "unshootable")
		{
			Destroy(gameObject);
		}
		
		else if (other.gameObject.layer == (int)CollisionLayer.Defualt && other.tag == "platform")
		{
			Destroy(gameObject);
		}		
	}

	#endregion


	#region Protected Functions

	// 
	protected void IncreaseLifeSpan(float increase)
	{
		//lifeSpan += increase;	
	}
	
	// 
	protected void InflictDamage(GameObject enemy)
	{
		if (enemy.tag == "shootable")
		{
			enemy.SendMessage("TakeDamage", damage);
		}
		
		if (enemy.tag != "Player" && enemy.tag != "shot" && enemy.tag != "unshootable")
		{
			Destroy(gameObject);
		}
	}

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        parent.ShotDestroyed();        
    }

    #endregion
}
