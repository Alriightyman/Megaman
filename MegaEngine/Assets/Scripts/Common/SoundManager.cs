using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour 
{
    #region Variables
    [SerializeField]
    [Range(0,1)]
    private int VolumeScale = 1;

	// private Instance Variables
	private string path = "Sounds/";
	private AudioSource stageMusic;
	private AudioSource stageEndMusic;
	private AudioSource megamanLeavesStageSound;
	private AudioSource deathSound;
	private AudioSource hurtingSound;
	private AudioSource landingSound;
	private AudioSource shootingSound;
	private AudioSource bossMusic;
	private AudioSource bossDoorSound;
	private AudioSource bossHurtingSound;
	private AudioSource healthBarFillSound;

	#endregion


	#region MonoBehaviour

	// Constructor
	private void Awake() 
	{
		GameEngine.SoundManager = this;
	}

	// Use this for initialization
	private void Start()
	{
		AudioClip stageMusicClip = (AudioClip) Resources.Load( path + "StageMusic" );
		stageMusic = AddAudio(stageMusicClip, true, true, VolumeScale * 0.50f);
		
		AudioClip stageEndMusicClip = (AudioClip) Resources.Load( path + "StageEndMusic" );
		stageEndMusic = AddAudio(stageEndMusicClip, false, false, VolumeScale * 1.0f);
		
		AudioClip deathSoundClip = (AudioClip) Resources.Load( path + "DeathSound" );
		deathSound = AddAudio(deathSoundClip, false, false, VolumeScale * 0.25f);
		
		AudioClip healthBarFillSoundClip = (AudioClip) Resources.Load( path + "HealthBarFillSound" );
		healthBarFillSound = AddAudio(healthBarFillSoundClip, true, false, VolumeScale * 1.0f);
		
		AudioClip hurtingSoundClip = (AudioClip) Resources.Load( path + "HurtingSound" );
		hurtingSound = AddAudio(hurtingSoundClip, false, false, VolumeScale * 1.0f);
		
		AudioClip landingSoundClip = (AudioClip) Resources.Load( path + "LandingSound" );
		landingSound = AddAudio(landingSoundClip, false, false, VolumeScale * 1.0f);
		
		AudioClip shootingSoundClip = (AudioClip) Resources.Load( path + "ShootingSound" );
		shootingSound = AddAudio(shootingSoundClip, false, false, VolumeScale * 1.0f);		
		
		AudioClip bossMusicClip = (AudioClip) Resources.Load( path + "BossMusic" );
		bossMusic = AddAudio(bossMusicClip, true, true, VolumeScale * 0.90f);
		
		AudioClip bossDoorSoundClip = (AudioClip) Resources.Load( path + "BossDoorSound" );
		bossDoorSound = AddAudio(bossDoorSoundClip, false, false, VolumeScale * 0.75f);
		
		AudioClip bossHurtingSoundClip = (AudioClip) Resources.Load( path + "BossHurtingSound" );
		bossHurtingSound = AddAudio(bossHurtingSoundClip, false, false, VolumeScale * 0.95f);
		
		AudioClip megamanLeavesClip = (AudioClip) Resources.Load( path + "MegamanLeaveStageSound" );
		megamanLeavesStageSound = AddAudio(megamanLeavesClip, false, false, VolumeScale * 0.99f);
		
		Play(AirmanLevelSounds.STAGE);
	}

	// Called when the behaviour becomes disabled or inactive
	private void OnDisable()
	{
		GameEngine.SoundManager = null;
	}

	#endregion


	#region private Functions
	
	// 
	private AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol) 
	{
		AudioSource newAudio = gameObject.AddComponent<AudioSource>();
		newAudio.clip = clip;
		newAudio.loop = loop;
		newAudio.playOnAwake = playAwake;
		newAudio.volume = vol;
		return newAudio;
	}
	
	#endregion


	#region Public Functions

	// Plays a sound / music
	public void Play(AirmanLevelSounds soundToPlay)
	{
		switch(soundToPlay)
		{
		case AirmanLevelSounds.STAGE:
			stageMusic.Play();
			break;
		case AirmanLevelSounds.STAGE_END:
			stageEndMusic.Play();
			break;
		case AirmanLevelSounds.LEAVE_LEVEL:
			megamanLeavesStageSound.Play();
			break;
		case AirmanLevelSounds.DEATH:
			deathSound.Play();
			break;
		case AirmanLevelSounds.HURTING:
			hurtingSound.Play();
			break;
		case AirmanLevelSounds.LANDING:
			landingSound.Play();
			break;
		case AirmanLevelSounds.SHOOTING:
			shootingSound.Play();
			break;
		case AirmanLevelSounds.BOSS_MUSIC:
			bossMusic.Play();
			break;
		case AirmanLevelSounds.BOSS_DOOR:
			bossDoorSound.Play();
			break;
		case AirmanLevelSounds.BOSS_HURTING:
			bossHurtingSound.Play();
			break;
		case AirmanLevelSounds.HEALTHBAR_FILLING:
			healthBarFillSound.Play();
			break;
		}
	}

	// Stops a sound / music
	public void Stop(AirmanLevelSounds soundToPlay)
	{
		switch(soundToPlay)
		{
		case AirmanLevelSounds.STAGE:
			stageMusic.Stop();
			break;
		case AirmanLevelSounds.STAGE_END:
			stageEndMusic.Stop();
			break;
		case AirmanLevelSounds.LEAVE_LEVEL:
			megamanLeavesStageSound.Stop();
			break;
		case AirmanLevelSounds.DEATH:
			deathSound.Stop();
			break;
		case AirmanLevelSounds.HURTING:
			hurtingSound.Stop();
			break;
		case AirmanLevelSounds.LANDING:
			landingSound.Stop();
			break;
		case AirmanLevelSounds.SHOOTING:
			shootingSound.Stop();
			break;
		case AirmanLevelSounds.BOSS_MUSIC:
			bossMusic.Stop();
			break;
		case AirmanLevelSounds.BOSS_DOOR:
			bossDoorSound.Stop();
			break;
		case AirmanLevelSounds.BOSS_HURTING:
			bossHurtingSound.Stop();
			break;
		case AirmanLevelSounds.HEALTHBAR_FILLING:
			healthBarFillSound.Stop();
			break;
		}
	}

	#endregion

}
