using Sandbox;
public partial class DroneEntity
{
	Sound Loop;
	void StartSounds()
	{
		PlaySound( "drone-start" );

		Loop.SetVolume( 1 );
		Loop = PlaySound( "drone-loop" );
	}

	void StopSounds()
	{
		PlaySound( "drone-stop" );

		Loop.SetVolume( 0 );
		Loop.Stop();
	}
}
