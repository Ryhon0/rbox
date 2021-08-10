namespace Sandbox.Tools
{
	[Library( "tool_slider", Title = "Slider", Description = "Physical Slider", Group = "gmod" )]
	public partial class SliderTool : BaseTool
	{
		private PhysicsBody body1, body2;
		private int body1Bone;

		private Vector3 LPos1;
		private Vector3 WPos1, WPos2;

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

				if ( !body1.IsValid() )
				{
					body1 = tr.Body;
					body1Bone = tr.Bone;

					WPos1 = tr.EndPos;
					LPos1 = body1.Transform.PointToLocal( WPos1 );
				}
				else
				{
					body2 = tr.Body;

					if ( body1 == body2 )
					{
						body1 = null;
						return;
					}

					WPos1 = body1.Transform.PointToWorld( LPos1 );
					WPos2 = tr.EndPos;

					#region Rope

					var rope = Particles.Create( "particles/rope.vpcf" );

					if ( body1.Entity.IsWorld )
					{
						rope.SetPosition( 0, WPos1 );
					}
					else
					{
						rope.SetEntityBone( 0, body1.Entity, body1Bone, new Transform( LPos1 * (1.0f / body1.Entity.Scale) ) );
					}

					var localOrigin2 = tr.Body.Transform.PointToLocal( WPos2 );

					if ( tr.Entity.IsWorld )
					{
						rope.SetPosition( 1, localOrigin2 );
					}
					else
					{
						rope.SetEntityBone( 1, tr.Entity, tr.Bone, new Transform( localOrigin2 * (1.0f / tr.Entity.Scale) ) );
					}

					#endregion Rope

					var slider = PhysicsJoint.Prismatic
						.From( body1 )
						.To( body2 )
						.WithPivot( WPos2 )
						.WithBasis( Rotation.LookAt( (WPos2 - WPos1).Normal ) * Rotation.From( new Angles( 90, 0, 0 ) ) )
						.WithCollisionsEnabled()
						.Create();

					if ( Host.IsServer )
						Undo.Add( Owner.GetClientOwner(), new PhysicsJointUndo( slider ) );

					slider.OnBreak( () =>
					{
						rope?.Destroy();
					} );

					body1.PhysicsGroup?.Wake();
					body2.PhysicsGroup?.Wake();

					body1 = null;
					body2 = null;
				}
				CreateHitEffects( tr.EndPos );
			}
		}
	}
}
