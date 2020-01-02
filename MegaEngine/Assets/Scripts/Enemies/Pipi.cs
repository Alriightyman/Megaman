using UnityEngine;
using System.Collections;

public class Pipi : MonoBehaviour 
{
	#region Variables

	// Editor variables
	[SerializeField] private float speed = 10.0f;
	[SerializeField] private float dropDistance = 10f;

	// Private Instance Variables
	private Egg egg;
	private bool moving = false;
	private bool attacking = false;
	private float lifeSpan = 5.0f;
	private float lifeTimer;
	private float damage = 20.0f;

    private SpriteRenderer spriteRenderer;
    private SpriteRenderer eggRenderer;

	#endregion


	#region MonoBehaviour

	// Constructor 
	protected void Awake ()
	{
		egg = gameObject.GetComponentInChildren<Egg>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        eggRenderer = egg.GetComponent<SpriteRenderer>();

    }

	// Update is called once per frame
	protected void Update () 
	{
		if (moving == true)
		{
			transform.position += (-Vector3.right * speed * Time.deltaTime);
			
            if(!GetComponent<SpriteRenderer>().isVisible)
			{
                Invoke("DelayDestroy", 3f);
			}
            else
            {
                CancelInvoke("DelayDestroy");
            }
		}
		
		if (attacking == true)
		{
			if (Mathf.Abs(GameEngine.Player.transform.position.x - transform.position.x) <= dropDistance)
			{
                if (egg != null)
                {
                    egg.ReleaseEgg(speed);
                    attacking = false;
                }
			}
		}
	}
	
	//
	protected void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			GameEngine.Player.TakeDamage(damage);
		}
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
	public void Attack()
	{
		lifeTimer = Time.time;
		moving = true;
		attacking = true;
        spriteRenderer.enabled = true;
        eggRenderer.enabled = true;
	}

    void DelayDestroy()
    {
        Destroy(gameObject);
    }
	#endregion
}
