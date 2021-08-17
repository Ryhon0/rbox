using Sandbox;
[Library( "chair", Title = "Chair", Spawnable = true, Group = "Vehicles" )]
public partial class Chair : Prop, IUse
{
	[Net]
	public Player Driver { get; private set; }
	public Vector3 SitOffset => Vector3.Up * 5 + Vector3.Forward * 10;
	public Vector3 GetOffPosition => Vector3.Forward * 100;

	public override void Spawn()
	{
		var modelName = "models/citizen_props/chair02.vmdl";

		SetModel( modelName );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
		EnableSelfCollisions = false;
	}

	public override void Simulate( Client owner )
	{
		if ( owner == null ) return;
		if ( !IsServer ) return;

		using ( Prediction.Off() )
		{
			if ( Input.Pressed( InputButton.Use ) || Input.Pressed( InputButton.Jump ) )
			{
				if ( owner.Pawn is SandboxPlayer player && !player.IsUseDisabled() )
				{
					RemoveDriver( player );

					return;
				}
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if ( Driver is SandboxPlayer player )
		{
			RemoveDriver( player );
		}
	}

	private void RemoveDriver( SandboxPlayer player )
	{
		player.LocalPosition = GetOffPosition;
		Driver = null;
		player.Vehicle = null;
		player.VehicleController = null;
		player.VehicleAnimator = null;
		player.VehicleCamera = null;
		player.Parent = null;
		player.PhysicsBody.Enabled = true;
	}

	public bool OnUse( Entity user )
	{
		if ( user is SandboxPlayer player && player.Vehicle == null )
		{
			player.Vehicle = this;
			player.Parent = this;
			player.VehicleAnimator = new CarAnimator();
			player.VehicleController = new ChairController();
			player.LocalPosition = SitOffset;
			player.LocalRotation = Rotation.Identity;
			player.LocalScale = 1;
			player.PhysicsBody.Enabled = false;

			Driver = player;
		}

		return true;
	}

	public bool IsUsable( Entity user )
	{
		return Driver == null;
	}
}
