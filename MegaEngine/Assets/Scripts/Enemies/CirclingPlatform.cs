using UnityEngine;
using System.Collections;
using Assets.Scripts.Interfaces;
using System;

public class CirclingPlatform : MonoBehaviour//, IResetable
{
	#region Variables

	// Unity Editor Variables
	public bool clockWise;		// Should the platform move clockwise?
	public float beginningAngle;	// At what angle should it start with?
	public float circleWidth; 	// What is the width of the ellipse/circle?
	public float circleHeight; 	// What is the height of the ellipse/circle?
	public float speedInSeconds; 	// How long should it take to move in a full circle?

    // Public Properties
    public bool ShouldAnimate { get; set; }
	
	// Private Instance Variables
	private Vector3 currentPos;
	private float speedScale;
	private Vector3 circleCenter;
	private float angle = 0.0f;
	private float fullCircle = (2.0f*Mathf.PI);
	private float fullCircleInDeg = 360.0f;
	private float convertFromDeg;
    private Vector3 initPos = new Vector3(0, 0, 0);
    private Action resetAction;
    bool start = false;
    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        resetAction = new Action(ResetObject);
        initPos = transform.position;
        GameEngine.AddResetCallback(resetAction);

    }
    // Use this for initialization
    void Start() 
	{
        ResetObject();
    }

    // Called when the Collider other enters the trigger.
    protected void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			GameEngine.Player.transform.parent =  gameObject.transform;
		}
	}

    // 
    protected void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			GameEngine.Player.transform.parent = null;
		}
	}
	
	// Update is called once per frame
	protected void Update()
	{
        if (speedInSeconds <= 0)
		{
			return;
		}
		else if (ShouldAnimate == true)
		{
			speedScale = fullCircle / speedInSeconds;
			
			if (clockWise == true)
			{
				angle = convertFromDeg * beginningAngle + (Time.time * speedScale) % fullCircle;
			}
			else if (clockWise == false)
			{
				angle = fullCircle - convertFromDeg * beginningAngle + (Time.time * speedScale) % fullCircle;
			}
			// Ellipse approach
			currentPos.x = circleCenter.x + (circleWidth/2.0f) * Mathf.Cos(angle);
			currentPos.y = circleCenter.y + (circleHeight/2.0f) * Mathf.Sin(angle);
			
			transform.position = currentPos;           

        }
	}
    #endregion

    #region Gizmos


    void OnDrawGizmos()
    {
        // Display the explosion radius when selected
        //Gizmos.color = Color.white;
        //Gizmos.DrawWireSphere(transform.position, circleHeight);
        if (start == false)
            DrawEllipse(transform.position, transform.forward, transform.up, circleWidth * transform.localScale.x, circleHeight * transform.localScale.y, 32, Color.yellow); 
        else
            DrawEllipse(initPos, transform.forward, transform.up, circleWidth * transform.localScale.x, circleHeight * transform.localScale.y, 32, Color.yellow); 

    }

    private static void DrawEllipse(Vector3 pos, Vector3 forward, Vector3 up, float radiusX, float radiusY, int segments, Color color, float duration = 0)
    {
        float angle = 0f;
        Quaternion rot = Quaternion.LookRotation(forward, up);
        Vector3 lastPoint = Vector3.zero;
        Vector3 thisPoint = Vector3.zero;

        for (int i = 0; i < segments + 1; i++)
        {
            thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX /2;
            thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY/ 2;

            if (i > 0)
            {
                Debug.DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color, duration);
            }

            lastPoint = thisPoint;
            angle += 360f / segments;
        }
    }

    public void ResetObject()
    {
        transform.position = initPos;
        convertFromDeg = (fullCircle / fullCircleInDeg);
        circleCenter = transform.position;
        ShouldAnimate = false;
        start = true;
    }

    private void OnDestroy()
    {
        GameEngine.RemoveResetCallback(resetAction);
    }
    #endregion
}