using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Keeps track of the games state
/// </summary>
public class GameStateManager : MonoBehaviour
{
    /// <summary>
    /// The instance of this game state manager
    /// </summary>
    public static GameStateManager Instance = null;

    #region Static Variables and Properties
    // important gameobjects to keep track of
    private static Player playerInstance = null;
    private static LevelCamera cameraInstance = null;

    /// <summary>
    /// Property to get/set the Player Object
    /// </summary>
    public static Player PlayerInstance
    {
        get
        {
            if(playerInstance == null)
            {
                playerInstance = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
            }

            return playerInstance;
        }

        set { playerInstance = value; }
    }

    /// <summary>
    /// Property to get/set the LevelCamera object
    /// </summary>
    public static LevelCamera CameraInstance
    {
        get
        {
            if (cameraInstance == null)
            {
                cameraInstance = Camera.main.GetComponent<LevelCamera>();
            }

            return cameraInstance;
        }

        set { cameraInstance = value; }
    }
    #endregion

    #region private variables
    int currentNumberOfLives = 3;
    #endregion

    #region Monobehavior Methods
    private void Awake()
    {
        // there should only be one game state manager at all times.
        // set the instance of the gamestate manager. 
        // if one already exists, then destroy this one.
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    #endregion

    #region Public Methods
    /// <summary>
    /// Restarts the current scene
    /// </summary>
    public void RestartLevel()
    {
        var currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    /// <summary>
    /// Add a Life to current number of lives
    /// </summary>
    public void AddLife()
    {
        currentNumberOfLives++;
    }

    /// <summary>
    /// Subtrack a life from current number of lives
    /// </summary>
    public void SubtractLife()
    {
        currentNumberOfLives--;
    }
    #endregion
}
