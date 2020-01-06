using UnityEngine;
using System.Collections;
using System;

public class Scworm : MonoBehaviour 
{
    #region Variables

    // Unity Editor Variables
    public GameObject powerup;
	[SerializeField] private Rigidbody2D shotPrefab;
	
	// private Instance Variables
	[SerializeField] private float distanceToStop = 14.0f;
    [SerializeField] private float attackDelay = .5f;
	private float attackTimer;
    private int health = 30;
    private int currentHealth;
    static int shotCount = 0;
    private const int maxShotCount = 3;

    #endregion

    #region MonoBehaviour

    // Use this for initialization
    private void Start () 
	{
		attackTimer = Time.time;
	}

	// Update is called once per frame
	private void Update () 
	{
		Vector3 direction = GameEngine.Player.transform.position - transform.position;
		
		// Kill this object if the player is too far away
		if ( direction.magnitude <=  distanceToStop)
		{
			Attack();
		}
	}

    private void OnDestroy()
    {
        Instantiate(powerup, transform);
    }

    #endregion

    #region private Functions

    // 
    private void KillChildren()
	{
		// Destroy all the shots...
		foreach(Transform child in transform)
		{
			Destroy( child.gameObject );
		}
	}
	
	// 
	private void Attack()
	{
		if ( Time.time - attackTimer >= attackDelay )
		{
			attackTimer = Time.time;

            if (shotCount < maxShotCount)
            {
                shotCount++;
                Vector3 pos = transform.position + Vector3.up * 0.8f + Vector3.right * 0.1f;
                pos = new Vector3(pos.x, pos.y + 8, pos.z);
                Rigidbody2D electricShot = (Rigidbody2D)Instantiate(shotPrefab, pos, transform.rotation);
                Physics2D.IgnoreCollision(electricShot.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                electricShot.GetComponent<ScwormShot>().Attack(GameEngine.Player.transform.position);
                electricShot.transform.parent = gameObject.transform;
            }
		}
	}

	#endregion

	#region Public Functions
	
    public void DecrementShotCount()
    {
        shotCount--;
    }

	// 
	public void Reset()
	{
		KillChildren();
	}

    private void TakeDamage(int damageTaken)
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

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, distanceToStop);
    }
    #endregion
}
