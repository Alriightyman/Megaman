using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class AirmanTrigger : MonoBehaviour
{
	#region Variables
	
	// Protected Instance Variables
	protected AirmanBoss airman;
	protected Collider2D col;

	#endregion


	#region MonoBehaviour
	
	// Constructor
	protected void Awake()
	{
		airman = FindObjectOfType<AirmanBoss>();
		Assert.IsNotNull(airman);

		col = GetComponent<Collider2D>();
		Assert.IsNotNull(col);
	}
	
	// Use this for initialization
	protected void Start()
	{
		airman.gameObject.SetActive(false);
	}

	// Called when the Collider other enters the trigger.
	protected void OnTriggerEnter2D(Collider2D other) 
	{
        StartCoroutine("SetBoss");

		col.enabled = false;
	}

    IEnumerator SetBoss()
    {
        yield return new WaitForSeconds(0.5f);
        airman.gameObject.SetActive(true);
        airman.SetUpAirman();
    }

	#endregion
}

