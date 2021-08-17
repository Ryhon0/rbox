namespace Sandbox.Tools
{
	[Library( "tool_barrelgun", Title = "Barrel Shooter", Description = "Shoot explosive barrels", Group = "fun" )]
	public class BarrelShooter : BaseTool
	{
		TimeSince timeSinceShoot;

		public override void Simulate()
		{
			if ( Host.IsServer )
			{
				if ( Input.Pressed( InputButton.Attack1 ) )
				{
					ShootBarrel();
				}

				if ( Input.Down( InputButton.Attack2 ) && timeSinceShoot > 0.05f )
				{
					timeSinceShoot = 0;
					ShootBarrel();
				}
			}
		}

		void ShootBarrel()
		{
			var ent = new Prop
			{
				Position = Owner.EyePos + Owner.EyeRot.Forward * 50,
				Rotation = Owner.EyeRot
			};

			ent.SetModel( "models/rust_props/barrels/fuel_barrel.vmdl" );
			ent.Velocity = Owner.EyeRot.Forward * 1000;

			if ( Host.IsServer )
				Undo.Add( Owner.GetClientOwner(), new EntityUndo( ent ) );
		}
	}

}
