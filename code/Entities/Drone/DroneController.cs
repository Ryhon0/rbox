using Sandbox;
public class DroneController : PawnController
{
	public override void FrameSimulate()
	{
		Simulate();
	}

	public override void Simulate()
	{
		if ( Pawn is not SandboxPlayer p ) return;
		if ( p.Vehicle is DroneEntity drone ) drone.Simulate( Client );
	}
}
