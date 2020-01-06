using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public class AirmanWind : MonoBehaviour 
{
	#region Variables

	// Unity Editor Variables
	public List<Material> animationMaterials;
    public float windSpeed = 20f;
	// private Instance Variables
	private int texIndex = 0;
	private bool leaving = false;
	private bool beginSequence = true;
	private bool shouldBlowLeft = true;
	private float texChangeInterval = 0.1f;
	private float damage = 4f; 
	private Vector2 texScale = Vector2.zero;
	private Vector2 texScaleRight = new Vector2(1.0f, -1.0f);
	private Vector2 texScaleLeft = new Vector2(-1.0f, -1.0f);
	private Vector3 windPosition = Vector3.zero;
	private SpriteRenderer rend = null;
    private Animator anim = null;
	#endregion
	
	
	#region MonoBehaviour

	// Constructor
	private void Awake()
	{
		rend = GetComponent<SpriteRenderer>();
		Assert.IsNotNull(rend);

        anim = GetComponent<Animator>();
        anim.Play("Wind");
    }

	// Update is called once per frame
	private void Update () 
	{
		if (beginSequence == true)
		{
			transform.position += (windPosition - transform.position) * Time.deltaTime * 3.0f;
			
			if ((windPosition - transform.position).magnitude <= 1.0f)
			{
				beginSequence = false;
			}
		}

        // Update the textures...
        //texIndex = (int) (Time.time / texChangeInterval);
        //rend.material = animationMaterials[texIndex % (animationMaterials.Count-1)];
        //rend.material.SetTextureScale("_MainTex", texScale);
        rend.flipX = !shouldBlowLeft;

        // If the wind is being blown away...
        if (leaving == true)
		{
			// Move the wind away...
			if (shouldBlowLeft)
			{
				transform.position -= new Vector3(windSpeed * Time.deltaTime , 0f, 0f);
			}
			else
			{
				transform.position += new Vector3(windSpeed * Time.deltaTime , 0f, 0f);
			}				
		}
	}

	// Called when the Collider other enters the trigger.
	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.tag == "Player")
		{
			other.gameObject.SendMessage("TakeDamage", damage);
		}
		else if (other.tag == "shot")
		{
			if (shouldBlowLeft == true)
			{
				other.GetComponent<Shot>().VelocityDirection = new Vector3(-1f, 1f, 0f);
			}
			else
			{
				other.GetComponent<Shot>().VelocityDirection = new Vector3(1f, 1f, 0f);
			}
		}
	}

	#endregion
	
	
	#region private Functions

	// 
	private void SetPosition(Vector3 pos)
	{
		windPosition = pos;
		shouldBlowLeft = (pos.x - transform.position.x < 0.0f);
		texScale = (shouldBlowLeft) ? texScaleLeft : texScaleRight;
	}

	#endregion
	
	
	#region Public Functions

	//
	public void GoAway()
	{
		leaving = true;
		GetComponent<Collider2D>().isTrigger = true;
	}

	#endregion
}