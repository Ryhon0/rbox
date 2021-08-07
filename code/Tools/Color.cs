namespace Sandbox.Tools
{
	[Library( "tool_color", Title = "Color", Description = "Change render color and alpha of entities", Group = "construction" )]
	public partial class ColorTool : BaseTool
	{
		public override void Simulate()
		{
			if ( !Host.IsServer )
				return;

			using ( Prediction.Off() )
			{
				if ( Input.Pressed( InputButton.Attack1 ) || Input.Pressed( InputButton.Attack2 ) )
				{
					var startPos = Owner.EyePos;
					var dir = Owner.EyeRot.Forward;

					var tr = Trace.Ray( startPos, startPos + dir * MaxTraceDistance )
					   .Ignore( Owner )
					   .UseHitboxes()
					   .HitLayer( CollisionLayer.Debris )
					   .Run();

					if ( !tr.Hit || !tr.Entity.IsValid() )
						return;

					if ( tr.Entity is not ModelEntity modelEnt )
						return;

					modelEnt.RenderColor =
						Input.Pressed( InputButton.Attack2 )
						? (Owner as SandboxPlayer).PlayerColor
						: Color.Random.ToColor32();

					CreateHitEffects( tr.EndPos );
				}
			}
		}
	}
}
