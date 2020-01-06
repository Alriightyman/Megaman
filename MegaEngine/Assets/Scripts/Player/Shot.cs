using UnityEngine;
using System.Collections;

public class Shot : MonoBehaviour 
{
    #region Variables
    /// <summary>
    /// Particle system for when the projectile hits a robot
    /// </summary>
    public ParticleSystem destroyEffect;

	// Properties
    /// <summary>
    /// Direction of Velocity
    /// </summary>
	public Vector3 VelocityDirection { get; set; }

    /// <summary>
    /// Speed of the projectile
    /// </summary>
	public float ShotSpeed { get; set; }

    /// <summary>
    /// projectiles parent
    /// </summary>
    public Shooting parent { get; set; }

	// private Instance Variables
	private float lifeSpan = 3f;
    private int damage = 4;

	#endregion
	
	
	#region MonoBehaviour

	// Use this for initialization
	private void Start () 
	{
        // destroy projectile after a small bit of time
        Destroy(gameObject, lifeSpan);
	}

	// Update is called once per frame
	private void Update () 
	{
        // interesting.. (TODO)
		GetComponent<Rigidbody2D>().velocity = VelocityDirection * ShotSpeed;
	}

	// Called when the Collider other enters the trigger.
	private void OnTriggerEnter2D(Collider2D other) 
	{
        // if object is shootable, inflict damage
		if (other.tag == "shootable")
		{
			InflictDamage(other.gameObject);
		}
		// if an enemy robot and unshootable destory
		else if (other.gameObject.layer == (int)CollisionLayer.EnemyRobot && other.tag == "unshootable")
		{
			Destroy(gameObject);	
		}
        // if unshootable destory
        else if (other.gameObject.layer == (int)CollisionLayer.Defualt && other.tag == "unshootable")
		{
			Destroy(gameObject);
		}
        // if platform destory
        else if (other.gameObject.layer == (int)CollisionLayer.Defualt && other.tag == "platform")
		{
			Destroy(gameObject);
		}		
	}

    #endregion


    #region Private Functions
    private void InflictDamage(GameObject enemy)
	{
		if (enemy.tag == "shootable")
		{
            Instantiate(destroyEffect, transform.position, transform.rotation);

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

    /// <summary>
    /// When destroyed, let parent know to decrement shot count
    /// </summary>
    private void OnDestroy()
    {
        parent.ShotDestroyed();        
    }

    #endregion
}
