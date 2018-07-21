using UnityEngine;
using System.Collections;

public class BigBirdTrigger : MonoBehaviour 
{
	#region MonoBehaviour

	// Called when the Collider other enters the trigger.
	protected void OnTriggerEnter2D(Collider2D other )
	{
		if (other.tag == "Player")
		{
			transform.parent.gameObject.GetComponent<BigBird>().Attack();
			gameObject.GetComponent<Collider2D>().enabled = false;
		}
	}

	#endregion
}
