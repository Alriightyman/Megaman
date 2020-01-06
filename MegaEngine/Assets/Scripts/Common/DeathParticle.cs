using UnityEngine;
using System.Collections;

public class DeathParticle : MonoBehaviour 
{
    #region Variables
    public Color Color { get; set; } = new Color(1f, 1f, 1f);

     // private Instance Variables
    private float lifeSpan = 2.6f;
	private float scaleSpeed = 0.05f;
	private float timeStart;
	private Vector3 initialScale = Vector3.one;
	private Vector2 scaleAmount = new Vector2(.75f, .75f);
    private SpriteRenderer spriteRenderer = null;
	#endregion
	
	
	#region MonoBehaviour

	// Use this for initialization
	private void Start()
	{
		this.timeStart = Time.time;
		this.initialScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();

    }
	
	// Update is called once per frame
	private void Update() 
	{

		if (scaleAmount.x > 0.0f && scaleAmount.y > 0.0f)
		{
			float scaleStatus = Time.time * this.scaleSpeed;
			transform.localScale = new Vector3(
	                    this.initialScale.x + Mathf.PingPong(scaleStatus, scaleAmount.x), 
						this.initialScale.y + Mathf.PingPong(scaleStatus, scaleAmount.y),
						0);
            spriteRenderer.color = new Color(Color.r, Color.g, Color.b, Random.Range(0.5f, 1f));
            
        }
		
		if (Time.time - this.timeStart >= lifeSpan)
		{
			Destroy(this.gameObject);
		}
	}

	#endregion
}
