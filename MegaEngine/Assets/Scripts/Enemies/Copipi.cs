using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class Copipi : MonoBehaviour 
{
    #region Variables
    public GameObject powerup;

    [SerializeField] private float lifeSpan = 10.0f;
    // private Instance Variables
    private float speed = 7.5f;
	private bool attacking = false;
	private Vector3 direction;
	private float lifeTimer;
	private float damage = 2f;

    private Animator anim;
    private SpriteRenderer renderer;

    #endregion


    #region MonoBehaviour

    private void Start()
    {
        anim.Play("Fly");

    }

    private void Awake()
    {
        anim = GetComponent<Animator>();

        Assert.IsNotNull(anim);

        renderer = GetComponent<SpriteRenderer>();
        Assert.IsNotNull(renderer);

    }


    // Called when the Collider other enters the trigger.
    private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			GameEngine.Player.TakeDamage(damage);
		}
	}
	
	// 
	private void Update () 
	{
		if (attacking == true)
		{
			transform.position += (direction * speed * Time.deltaTime);
			
			if (Time.time - lifeTimer >= lifeSpan)
			{
				Destroy (gameObject);	
			}
		}
	}

	#endregion


	#region Public Functions

	// 
	public void Attack(bool goLeft, float birdSpeed)
	{
		attacking = true;
        direction = new Vector3(1f, Random.Range(-1, 1), 0f);
        StartCoroutine("ChangeDirection");
		speed = birdSpeed;
		lifeTimer = Time.time;
	}
	
	// 
	public void TakeDamage(float dam)
	{
		GameEngine.SoundManager.Play(AirmanLevelSounds.BOSS_HURTING);
		Destroy (gameObject);
	}

    private void OnDestroy()
    {
        Instantiate(powerup, transform);
    }

    #endregion

    private  IEnumerator ChangeDirection()
    {
        while (true)
        {
            float x = Random.Range(-1, 1);
            float y = Random.Range(-1, 1);

            if(x == 0f && y == 0f)
            {
                x = Mathf.Round(Time.time) % 2 == 0 ? 1 : 0;
                y = Mathf.Round(Time.time) % 1 == 0 ? 1 : 0;
            }

            renderer.flipX = x == -1 ? false : true;
            direction = new Vector3(x, y, 0f);
            yield return new WaitForSeconds(0.25f);
        }
    }
}
