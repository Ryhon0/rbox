using Sandbox;
public class BehindCamera : Camera
{
	public override void Update()
	{
		Rot = (Local.Pawn as SandboxPlayer).Vehicle.Rotation.RotateAroundAxis( new Vector3( 0, 0, 1 ), 180 );
		Pos = (Local.Pawn as SandboxPlayer).Vehicle.Position + (Vector3.Up * 30) + (Rot.Backward * 50);
		FieldOfView = 65;
	}
}
