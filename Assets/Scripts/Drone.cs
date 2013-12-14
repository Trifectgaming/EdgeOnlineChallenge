public class Drone : GameSceneObject
{
    public tk2dAnimatedSprite sprite;
    public ShieldManager shieldManager;

    protected override void Start ()
    {
        base.Start();
    }

    protected override void OnGamePaused(GamePausedMessage obj)
    {
        base.OnGamePaused(obj);
    }

    protected override void OnGameResume(GameResumeMessage obj)
    {
        base.OnGameResume(obj);
    }

    private void Update()
    {

    }
}