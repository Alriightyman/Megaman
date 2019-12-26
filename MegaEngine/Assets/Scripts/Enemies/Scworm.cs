using UnityEngine;
using System.Collections;
using System;

public class Scworm : MonoBehaviour 
{
	#region Variables

	// Unity Editor Variables
	[SerializeField] protected Rigidbody2D shotPrefab;
	
	// Protected Instance Variables
	protected float distanceToStop = 14.0f;
	protected float attackDelay = 2.0f;
	protected float attackTimer;
    protected int health = 30;
    protected int currentHealth;

    #endregion


    #region MonoBehaviour

    // Use this for initialization
    protected void Start () 
	{
		attackTimer = Time.time;
	}

	// Update is called once per frame
	protected void Update () 
	{
		Vector3 direction = GameEngine.Player.transform.position - transform.position;
		
		// Kill this object if the player is too far away
		if ( direction.magnitude <=  distanceToStop)
		{
			Attack();
		}
	}

	#endregion

	#region Protected Functions

	// 
	protected void KillChildren()
	{
		// Destroy all the shots...
		foreach(Transform child in transform)
		{
			Destroy( child.gameObject );
		}
	}
	
	// 
	protected void Attack()
	{
		if ( Time.time - attackTimer >= attackDelay )
		{
			attackTimer = Time.time;
			
			Vector3 pos = transform.position + Vector3.up * 0.8f + Vector3.right * 0.1f;
            Rigidbody2D electricShot = (Rigidbody2D) Instantiate(shotPrefab, pos, transform.rotation);
			Physics2D.IgnoreCollision(electricShot.GetComponent<Collider2D>(), GetComponent<Collider2D>());
			electricShot.GetComponent<ScwormShot>().Attack( GameEngine.Player.transform.position );
			electricShot.transform.parent = gameObject.transform;
		}
	}

	#endregion

	#region Public Functions
	
	// 
	public void Reset()
	{
		KillChildren();
	}

    protected void TakeDamage(int damageTaken)
    {
        GameEngine.SoundManager.Play(AirmanLevelSounds.BOSS_HURTING);
        currentHealth -= damageTaken;

        if (currentHealth <= 0)
        {
            KillRobot();
        }
    }

    private void KillRobot()
    {
        KillChildren();
        Destroy(gameObject);
    }

    #endregion
}