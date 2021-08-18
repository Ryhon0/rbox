using Sandbox;

[Library( "ent_maluch", Title = "Maluch", Group = "Vehicles", Spawnable = true )]
public class Maluch : CarEntity
{
	public override string ModelPath => "entities/maluch.vmdl";
	public override string WheelModelPath => "entities/maluch_wheel.vmdl";
	public override Vector3 FrontAxlePosition => base.FrontAxlePosition + Vector3.Forward * 12.5f;
	public override Vector3 SeatPosition => base.SeatPosition + Vector3.Left * 15 + Vector3.Down * 5;
}
