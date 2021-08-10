namespace Sandbox.Tools
{
	[Library( "tool_nocollide", Title = "No Collide", Description = "Disable collisions", Group = "gmod" )]
	public partial class NoCollideTool : BaseTool
	{
		private PhysicsBody body;

		public override void Simulate()
		{
			if ( !Host.IsServer )
				return;

			using ( Prediction.Off() )
			{
				if ( !Input.Pressed( InputButton.Attack1 ) )
					return;

				var startPos = Owner.EyePos;
				var dir = Owner.EyeRot.Forward;

				var tr = Trace.Ray( startPos, startPos + dir * MaxTraceDistance )
					.Ignore( Owner )
					.Run();

				if ( !tr.Hit )
					return;

				if ( !tr.Entity.IsValid() )
					return;

				if ( !(tr.Body.IsValid() && (tr.Body.PhysicsGroup != null) && tr.Body.Entity.IsValid()) ) return;

				if ( !body.IsValid() )
				{
					body = tr.Body;
				}
				else
				{
					if ( body == tr.Body )
					{
						body = null;
						return;
					}

					var j = PhysicsJoint.Generic
						.From( body )
						.To( tr.Body )
						.Create();

					if ( Host.IsServer )
						Undo.Add( Owner.GetClientOwner(), new PhysicsJointUndo( j ) );

					body.PhysicsGroup?.Wake();
					tr.Body.PhysicsGroup?.Wake();

					body = null;
				}

				CreateHitEffects( tr.EndPos );
			}
		}
	}
}
