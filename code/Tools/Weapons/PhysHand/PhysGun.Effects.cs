
using Sandbox;
using System.Linq;

public partial class PhysGun
{
	ModelEntity lastGrabbedEntity;

	[Event.Frame]
	public void OnFrame()
	{
		UpdateEffects();
	}

	protected virtual void KillEffects()
	{
		if ( lastGrabbedEntity.IsValid() )
		{
			foreach ( var child in lastGrabbedEntity.Children.OfType<ModelEntity>() )
			{
				if ( child is Player )
					continue;

				child.GlowActive = false;
				child.GlowState = GlowStates.GlowStateOff;
			}

			lastGrabbedEntity.GlowActive = false;
			lastGrabbedEntity.GlowState = GlowStates.GlowStateOff;
			lastGrabbedEntity = null;
		}
	}

	protected virtual void UpdateEffects()
	{
		var owner = Owner;

		if ( owner == null || !BeamActive || !IsActiveChild() )
		{
			KillEffects();
			return;
		}

		var startPos = owner.EyePos;
		var dir = owner.EyeRot.Forward;

		var tr = Trace.Ray( startPos, startPos + dir * MaxTargetDistance )
			.UseHitboxes()
			.Ignore( owner )
			.Run();

		if ( GrabbedEntity.IsValid() && !GrabbedEntity.IsWorld )
		{
			if ( GrabbedEntity is ModelEntity modelEnt )
			{
				lastGrabbedEntity = modelEnt;
				modelEnt.GlowState = GlowStates.GlowStateOn;
				modelEnt.GlowDistanceStart = 0;
				modelEnt.GlowDistanceEnd = 1000;
				modelEnt.GlowColor = (Owner as SandboxPlayer).PlayerColor;
				modelEnt.GlowActive = true;

				foreach ( var child in lastGrabbedEntity.Children.OfType<ModelEntity>() )
				{
					if ( child is Player )
						continue;

					child.GlowState = GlowStates.GlowStateOn;
					child.GlowDistanceStart = 0;
					child.GlowDistanceEnd = 1000;
					child.GlowColor = (Owner as SandboxPlayer).PlayerColor;
					child.GlowActive = true;
				}
			}
		}
	}
}
