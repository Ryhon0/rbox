using Sandbox;
namespace Sandbox.Tools
{
	[Library( "tool_bone_merge", Title = "Bone Merge", Description = "Merges the bones of 2 objects. Right click selects self", Group = "rbox" )]
	public partial class BoneMergeTool : BaseTool
	{
		private ModelEntity target;

		public override void Simulate()
		{
			if ( !Host.IsServer )
				return;

			using ( Prediction.Off() )
			{
				if ( Input.Pressed( InputButton.Attack1 ) )
				{
					var startPos = Owner.EyePos;
					var dir = Owner.EyeRot.Forward;

					var tr = Trace.Ray( startPos, startPos + dir * MaxTraceDistance )
						.Ignore( Owner )
						.Run();

					if ( !tr.Hit || !tr.Body.IsValid() || !tr.Entity.IsValid() || tr.Entity.IsWorld || tr.Entity is Client || tr.Entity.Parent is Weapon)
						return;

					if ( target.IsValid() )
					{
						tr.Entity.SetParent( target, true );
						Reset();
					}
					else target = tr.Entity as ModelEntity;

					CreateHitEffects( tr.EndPos );
				}

				if ( Input.Pressed( InputButton.Attack2 ) )
				{
					if ( target.IsValid() )
					{
						target.SetParent( Owner, true );
						Reset();
					}
				}
			}
		}

		private void Reset()
		{
			target = null;
		}

		public override void Activate()
		{
			base.Activate();

			Reset();
		}

		public override void Deactivate()
		{
			base.Deactivate();

			Reset();
		}
	}
}
