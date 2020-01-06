using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Assets.Scripts.Interfaces;
using UnityEngine.SceneManagement;

public class GameEngine : MonoBehaviour
{

    public static GameObject PlayerHealth = null;
    public static GameEngine instance = null;
    private static Player player = null;
	public static Player Player 
    { 
        get
        {
            if(player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            }
            return player;
        }
        set { player = value; }
    }
	public static SoundManager SoundManager { get; set; }
	public static AirmanBoss AirMan { get; set; }
    public static bool LevelStarting { get; set; } = true;

	private static event Action ResetCallbackList;

    // private static List<IResetable> resetableObjects = new List<IResetable>();
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        PlayerHealth = GameObject.Find("PlayerHealth");
    }

    public static void AddResetCallback(Action resetCallback)
	{
		ResetCallbackList += resetCallback;
	}
	
	public static void RemoveResetCallback(Action resetCallback)
	{
		ResetCallbackList -= resetCallback;
	}

    //public static List<IResetable> GetResetableObjectList()
    //{
    //    lock (resetableObjects)
    //    {
    //        if (resetableObjects == null)
    //            resetableObjects = new List<IResetable>();
    //    }
    //    return resetableObjects;
    //}

    public static IEnumerator Restart()
    {
        StopMusic();

        Player.KillPlayer();
        yield return new WaitForSeconds(3.6f);

        //ResetCallbackList?.Invoke();

        //if (AirMan != null)
        //    AirMan.Reset();

        // Start another wait to avoid double deaths by the hand of deathtriggers... 
        yield return new WaitForSeconds(0.3f);

        Player.RevivePlayer();

        SceneManager.LoadScene(0);

        PlayerHealth.SetActive(true);
    }

    private static void StopMusic()
    {
        SoundManager.Stop(AirmanLevelSounds.STAGE);
        SoundManager.Stop(AirmanLevelSounds.BOSS_MUSIC);
        SoundManager.Play(AirmanLevelSounds.DEATH);
    }
}
