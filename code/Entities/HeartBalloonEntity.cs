using Sandbox;

[Library( "ent_heartballoon", Title = "Heart Balloon", Spawnable = true, Group = "Fun" )]
public partial class HeartBalloonEntity : BalloonEntity
{
	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/citizen_props/balloonheart01.vmdl" );
	}
}
