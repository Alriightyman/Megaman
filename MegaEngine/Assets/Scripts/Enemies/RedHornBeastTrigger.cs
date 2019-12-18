using UnityEngine;
using System.Collections;

public class RedHornBeastTrigger : MonoBehaviour
{
	#region MonoBehaviour

	// Called when the Collider other enters the trigger.
	protected void OnTriggerEnter2D(Collider2D other) 
	{
		// Make the beast appear...
		if (other.tag == "Player")
		{
			transform.parent.gameObject.SendMessage("Appear");
		}
    }

	#endregion
}

