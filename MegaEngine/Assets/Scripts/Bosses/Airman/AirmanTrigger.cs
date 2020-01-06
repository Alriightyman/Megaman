using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class AirmanTrigger : MonoBehaviour
{
	#region Variables
	
	// private Instance Variables
	private AirmanBoss airman;
	private Collider2D col;

	#endregion


	#region MonoBehaviour
	
	// Constructor
	private void Awake()
	{
		airman = FindObjectOfType<AirmanBoss>();
		Assert.IsNotNull(airman);

		col = GetComponent<Collider2D>();
		Assert.IsNotNull(col);
	}
	
	// Use this for initialization
	private void Start()
	{
		airman.gameObject.SetActive(false);
	}

	// Called when the Collider other enters the trigger.
	private void OnTriggerEnter2D(Collider2D other) 
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

