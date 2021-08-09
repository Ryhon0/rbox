using Sandbox;

[Library( "ent_mouseballoon", Title = "Mouse sBalloon", Spawnable = true )]
public partial class MouseBalloonEntity : BalloonEntity
{
	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/citizen_props/balloonears01.vmdl" );
	}
}
