namespace Sandbox.Tools
{
	[Library( "tool_cargun", Title = "Car Shooter", Description = "Shoot cars", Group = "fun" )]
	public class CarShooter : BaseTool
	{
		TimeSince timeSinceShoot;

		public override void Simulate()
		{
			if ( Host.IsServer )
			{
				if ( Input.Pressed( InputButton.Attack1 ) )
				{
					ShootMelon();
				}

				if ( Input.Down( InputButton.Attack2 ) && timeSinceShoot > 0.05f )
				{
					timeSinceShoot = 0;
					ShootMelon();
				}
			}
		}

		void ShootMelon()
		{
			var ent = new CarEntity
			{
				Position = Owner.EyePos + Owner.EyeRot.Forward * 50,
				Rotation = Owner.EyeRot
			};

			ent.Velocity = Owner.EyeRot.Forward * 1000;

			if ( Host.IsServer )
				Undo.Add( Owner.GetClientOwner(), new EntityUndo( ent ) );
		}
	}

}
