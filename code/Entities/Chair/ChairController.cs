using Sandbox;
public class ChairController : PawnController
{
	public override void FrameSimulate()
	{
		base.FrameSimulate();

		Simulate();
	}

	public override void Simulate()
	{
		var chair = Pawn.Parent as Chair;
		chair.Simulate( Client );

		EyeRot = Input.Rotation;
		EyePosLocal = Vector3.Up * (64 - 10) * chair.Scale;
		Velocity = chair.Velocity;

		SetTag( "noclip" );
		SetTag( "sitting" );
	}
}
