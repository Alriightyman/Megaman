
// Sound enums...
public enum AirmanLevelSounds
{
	STAGE,
	STAGE_END,
	LEAVE_LEVEL,
	DEATH,
	HURTING,
	LANDING,
    SHOOTING,
	BOSS_MUSIC,
	BOSS_DOOR,
	BOSS_HURTING,
	HEALTHBAR_FILLING
}

public enum CollisionLayer
{
    Defualt,
    TransparentFX,
    IgnoreRayCast,
    Water = 4,
    UI,
    Player = 8,
    PlayerShot,
    EnemyRobot,
    EnemyShot,
    Trigger,
    Unshootable,
    Platform,
    PlatformOneWay
}