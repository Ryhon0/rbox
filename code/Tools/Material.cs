using Sandbox.UI;
using Sandbox.UI.Construct;

// https://github.com/Nebual/sandbox-plus/blob/main/code/tools/Material.cs
namespace Sandbox.Tools
{
	[Library( "tool_material", Title = "Material", Group = "construction", Description = "Override model material" )]
	public partial class MaterialTool : BaseTool
	{
		[ConVar.ClientData( "tool_material_current" )]
		public static string CurentMaterial { get; set; }

		public override void Simulate()
		{
			using ( Prediction.Off() )
			{
				if ( Input.Pressed( InputButton.Attack1 ) )
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

					if ( Input.Pressed( InputButton.Attack1 ) )
					{
						modelEnt.SetClientMaterialOverride( GetConvarValue( "tool_material_current" ) );

						CreateHitEffects( tr.EndPos );
					}
					else if ( Input.Pressed( InputButton.Attack2 ) )
					{
						modelEnt.SetMaterialGroup( modelEnt.GetMaterialGroup() + 1 );
						if ( modelEnt.GetMaterialGroup() == 0 )
						{
							// cycle back to start
							modelEnt.SetMaterialGroup( 0 );
						}

						CreateHitEffects( tr.EndPos );
					}

				}
			}
		}

		[ClientRpc]
		public static void SetEntityMaterialOverride( ModelEntity target, string path )
		{
			if ( Host.IsClient )
			{
				target?.SceneObject?.SetMaterialOverride( Material.Load( path ) );
			}
		}

		public override Panel CreatePanel()
		{
			return new MaterialOptions();
		}
	}

	public class MaterialOptions : Panel
	{
		public MaterialOptions()
		{
			StyleSheet.Load( "Tools/Material.scss" );
			AddClass( "materiallist" );

			foreach ( var file in FileSystem.Mounted.FindFile( "", "*.vmat_c.png", true ) )
			{
				if ( string.IsNullOrWhiteSpace( file ) ) continue;

				var b = Add.Button( "", "material" );
				b.AddEventListener( "onclick", () => ConsoleSystem.Run( "tool_material_current", file.Remove( file.Length - 6 ) ) );
				b.Style.Background = new PanelBackground
				{
					Texture = Texture.Load( $"{file}", false )
				};
			}
		}
	}

	public static partial class ModelEntityExtensions
	{
		public static void SetClientMaterialOverride( this ModelEntity instance, string material )
		{
			Sandbox.Tools.MaterialTool.SetEntityMaterialOverride( instance, material );
		}
	}
}
