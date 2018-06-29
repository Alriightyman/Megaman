using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Assets.Scripts.Interfaces;

public class GameEngine
{
	public static Player Player { get; set; }
	public static SoundManager SoundManager { get; set; }
	public static AirmanBoss AirMan { get; set; }

	protected static event Action ResetCallbackList;

    private static List<IResetable> resetableObjects = new List<IResetable>();

	public static void AddResetCallback(Action resetCallback)
	{
		ResetCallbackList += resetCallback;
	}
	
	public static void RemoveResetCallback(Action resetCallback)
	{
		ResetCallbackList -= resetCallback;
	}

    public static List<IResetable> GetResetableObjectList()
    {
        lock (resetableObjects)
        {
            if (resetableObjects == null)
                resetableObjects = new List<IResetable>();
        }
        return resetableObjects;
    }

    public static IEnumerator Reset()
    {
        StopMusic();

        Player.KillPlayer();
        yield return new WaitForSeconds(3.6f);
        foreach (IResetable resetableObject in resetableObjects)
        {
            resetableObject.Reset();
        }
        if (AirMan != null)
            AirMan.Reset();

        // Start another wait to avoid double deaths by the hand of deathtriggers... 
        yield return new WaitForSeconds(0.3f);

        Player.RevivePlayer();
    }

    private static void StopMusic()
    {
        SoundManager.Stop(AirmanLevelSounds.STAGE);
        SoundManager.Stop(AirmanLevelSounds.BOSS_MUSIC);
        SoundManager.Play(AirmanLevelSounds.DEATH);
    }
}
