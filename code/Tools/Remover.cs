namespace Sandbox.Tools
{
	[Library( "tool_remover", Title = "Remover", Description = "Remove entities", Group = "construction" )]
	public partial class RemoverTool : BaseTool
	{
		[Net]
		public float RemoverSphereRadius { get; set; } = 20f;
		public override void Simulate()
		{
			if ( !Host.IsServer )
				return;

			using ( Prediction.Off() )
			{

				if ( Input.Down( InputButton.Duck ) )
				{
					RemoverSphereRadius += Input.MouseWheel;
					Input.MouseWheel = 0;

					var startPos = Owner.EyePos;
					var dir = Owner.EyeRot.Forward;

					var tr = Trace.Ray( startPos, startPos + dir * MaxTraceDistance )
						.Ignore( Owner )
						.HitLayer( CollisionLayer.Debris )
						.Run();
					DebugOverlay.Sphere( tr.EndPos, RemoverSphereRadius - 1, Color.Yellow );
				}

				if ( Input.Pressed( InputButton.Attack1 ) )
				{
					var startPos = Owner.EyePos;
					var dir = Owner.EyeRot.Forward;

					var tr = Trace.Ray( startPos, startPos + dir * MaxTraceDistance )
						.Ignore( Owner )
						.HitLayer( CollisionLayer.Debris )
						.Run();

					if ( !tr.Hit || !tr.Entity.IsValid() )
						return;

					if ( tr.Entity is Player )
						return;

					CreateHitEffects( tr.EndPos );

					if ( tr.Entity.IsWorld )
						return;

					tr.Entity.Delete();

					var particle = Particles.Create( "particles/physgun_freeze.vpcf" );
					particle.SetPosition( 0, tr.Entity.Position );
				}
				else if ( Input.Down( InputButton.Attack2 ) )
				{
					var startPos = Owner.EyePos;
					var dir = Owner.EyeRot.Forward;

					var tr = Trace.Ray( startPos, startPos + dir * MaxTraceDistance )
						.Ignore( Owner )
						.HitLayer( CollisionLayer.Debris )
						.Run();

					var ents = Physics.GetEntitiesInSphere( tr.EndPos, RemoverSphereRadius );
					DebugOverlay.Sphere( tr.EndPos, RemoverSphereRadius, Color.Red );

					foreach ( var ent in ents )
					{
						if ( !ent.IsValid() || ent.IsWorld ||
							ent is Player || ent.Parent is Player ||
							ent.Parent is BaseCarriable || ent is Client ) continue;
						CreateHitEffects( ent.Position );
						var particle = Particles.Create( "particles/physgun_freeze.vpcf" );
						particle.SetPosition( 0, tr.EndPos );
						ent.Delete();
					}
				}
			}
		}
	}
}
