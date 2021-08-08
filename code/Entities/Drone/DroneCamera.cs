using Sandbox;
public class DroneCamera : Camera
{
	public override void Update()
	{
		if ( Local.Pawn is SandboxPlayer p && p.Vehicle != null )
		{
			var v = p.Vehicle;
			Rot = v.Rotation.RotateAroundAxis( new Vector3( 0, 0, 1 ), 180 );
			Pos = v.Position +
				((Vector3.Up * 30) + (Rot.Backward * 50)) * v.Scale;
		}

		FieldOfView = 65;
	}
}
