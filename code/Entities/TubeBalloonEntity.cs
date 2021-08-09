using Sandbox;

[Library( "ent_tubeballoon", Title = "Tube Balloon", Spawnable = true )]
public partial class TubeBalloonEntity : BalloonEntity
{
	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/citizen_props/balloontall01.vmdl" );
	}
}
