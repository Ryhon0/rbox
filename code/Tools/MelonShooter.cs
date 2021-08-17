namespace Sandbox.Tools
{
	[Library( "tool_melongun", Title = "Melon Shooter", Description = "Shoot melons", Group = "fun" )]
	public class MelonShooter : BaseTool
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
			var ent = new Prop
			{
				Position = Owner.EyePos + Owner.EyeRot.Forward * 50,
				Rotation = Owner.EyeRot
			};

			ent.SetModel( "models/sbox_props/watermelon/watermelon.vmdl" );
			ent.Velocity = Owner.EyeRot.Forward * 1000;

			if ( Host.IsServer )
				Undo.Add( Owner.GetClientOwner(), new EntityUndo( ent ) );
		}
	}

}
