using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Sandbox.Tools
{
	[Library( "tool_color", Title = "Color", Description = "Change render color and alpha of entities", Group = "construction" )]
	public partial class ColorTool : BaseTool
	{
		[ConVar.ClientData( "tool_color_tint" )]
		public static uint Tint { get; set; }

		public override void Simulate()
		{
			if ( !Host.IsServer )
				return;

			using ( Prediction.Off() )
			{
				if ( Input.Pressed( InputButton.Attack1 ) || Input.Pressed( InputButton.Reload ) )
				{
					var clear = Input.Pressed( InputButton.Reload );

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

					modelEnt.RenderColor = clear ? Color.White : this.GetColorConvar( "tool_color_tint", Color.White );

					CreateHitEffects( tr.EndPos );
				}
			}
		}

		public override Panel CreatePanel()
			=> new ColorToolPanel( this );
	}


	class ColorToolPanel : Panel
	{
		public ColorToolPanel( ColorTool t )
		{
			StyleSheet.Load( "/Tools/Color.scss" );
			Add.Label( "Color", "title" );

			var color = new ColorPicker();
			AddChild( color );

			var s = ConsoleSystem.GetValue( "tool_color_tint", Color.Red.RGBA.ToString() );
			uint rgba = 0;
			uint.TryParse( s, out rgba );
			color.Value = new Color( rgba );

			color.AddEventListener( "OnValueChanged", () =>
			{
				ConsoleSystem.Run( "tool_color_tint", color.Value.ToColor().RGBA );
			} );
		}
	}
}
