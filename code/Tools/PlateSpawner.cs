using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace Sandbox.Tools
{
	[Library( "tool_platespawner", Title = "Plate Spanwer", Description = "Spawn plates", Group = "rbox" )]
	class PlateSpawner : BaseTool
	{
		[ConVar.ClientData( "tool_platespawner_x" )]
		public static int PlateWidth { get; set; } = 100;
		[ConVar.ClientData( "tool_platespawner_y" )]
		public static int PlateHeight { get; set; } = 10;
		[ConVar.ClientData( "tool_platespawner_z" )]
		public static int PlateDepth { get; set; } = 100;
		[ConVar.ClientData( "tool_platespawner_texturesize" )]
		public static int TextureSize { get; set; } = 128;
		[ConVar.ClientData( "tool_platespawner_material" )]
		public static string Material { get; set; } = "materials/dev/dev_measuregeneric01.vmat";

		PreviewEntity previewModel;
		public override void CreatePreviews()
		{
			if ( Host.IsServer ) return;

			var mdl = VertexMeshBuilder.CreateRectangleModel( new Vector3(
				GetConvarInt( "tool_platespawner_y", 10 ),
				GetConvarInt( "tool_platespawner_x", 100 ),
				GetConvarInt( "tool_platespawner_z", 100 ) ),
				GetConvarValue( "tool_platespawner_material", "materials/dev/dev_measuregeneric01.vmat" ) );

			if ( TryCreatePreview( ref previewModel, "" ) )
			{
				previewModel.RelativeToNormal = true;
				previewModel.SetModel( VertexMeshBuilder.Models[mdl] );
			}
		}

		public override void Simulate()
		{
			base.Simulate();

			if ( !Host.IsServer ) return;
			using ( Prediction.Off() )
			{
				if ( Input.Pressed( InputButton.Attack1 ) )
				{
					var tr = Trace.Ray( Owner.EyePos, Owner.EyePos + Owner.EyeRot.Forward * 500 )
						.UseHitboxes()
						.Ignore( Owner )
						.Run();

					var ent = new MeshEntity();
					ent.Model = VertexMeshBuilder.GenerateRectangleServer(
						GetConvarInt( "tool_platespawner_y", 10 ),
						GetConvarInt( "tool_platespawner_x", 100 ),
						GetConvarInt( "tool_platespawner_z", 100 ),
						GetConvarValue( "tool_platespawner_material", "materials/dev/dev_measuregeneric01.vmat" ) );

					ent.Rotation = Rotation.LookAt( tr.Normal, tr.Direction );
					ent.Position = tr.EndPos + (ent.Rotation.Forward * ((Math.Abs( GetConvarInt( "tool_platespawner_y", 10 ) ) / 2) + 1));

					// Drop to floor
					if ( ent.PhysicsBody != null && ent.PhysicsGroup.BodyCount == 1 )
					{
						var p = ent.PhysicsBody.FindClosestPoint( tr.EndPos );

						var delta = p - tr.EndPos;
						ent.PhysicsBody.Position -= delta;
						//DebugOverlay.Line( p, tr.EndPos, 10, false );
					}

					Undo.Add( Owner.GetClientOwner(), new EntityUndo( ent ) );
				}
			}
		}

		public override void OnFrame()
		{
			base.OnFrame();

			if ( previewModel.IsValid() )
			{
				CreatePreviews();
				previewModel.RenderAlpha = 0.1f;
				previewModel.Position += previewModel.Rotation.Forward * ((Math.Abs( GetConvarInt( "tool_platespawner_y", 10 ) ) / 2) + 1);
			}
		}

		public override Panel CreatePanel()
		{
			return new PlateSpawnerPanel( this );
		}

		int GetConvarInt( string convar, int fallback = 1 )
		{
			int i = fallback;
			var s = GetConvarValue( convar, fallback.ToString() );
			int.TryParse( s, out i );
			return i;
		}
	}

	class PlateSpawnerPanel : Panel
	{
		public PlateSpawnerPanel( PlateSpawner p )
		{
			Style.FlexDirection = FlexDirection.Column;

			Add.Label( "Width" );
			var x = Add.TextEntry( p.GetConvarValue( "tool_platespawner_x", "100" ) );
			x.Numeric = true;
			x.AddEventListener( "onchange", () =>
			{
				ConsoleSystem.Run( "tool_platespawner_x", x.Text );
			} );

			Add.Label( "Depth" );
			var z = Add.TextEntry( p.GetConvarValue( "tool_platespawner_z", "100" ) );
			z.Numeric = true;
			z.AddEventListener( "onchange", () =>
			{
				ConsoleSystem.Run( "tool_platespawner_z", z.Text );
			} );

			Add.Label( "Height" );
			var y = Add.TextEntry( p.GetConvarValue( "tool_platespawner_y", "10" ) );
			y.Numeric = true;
			y.AddEventListener( "onchange", () =>
			{
				ConsoleSystem.Run( "tool_platespawner_y", y.Text );
			} );

			StyleSheet.Load( "Tools/Material.scss" );
			var mats = Add.Panel( "materiallist" );
			foreach ( var file in FileSystem.Mounted.FindFile( "", "*.vmat_c.png", true ) )
			{
				if ( string.IsNullOrWhiteSpace( file ) ) continue;

				var b = mats.Add.Button( "", "material" );
				b.AddEventListener( "onclick", () =>
				{
					Log.Info( file );
					ConsoleSystem.Run( "tool_platespawner_material", file.Remove( file.Length - 6 ) );
					Log.Info( ConsoleSystem.GetValue( "tool_platespawner_material", "" ) );
				} );
				b.Style.Background = new PanelBackground
				{
					Texture = Texture.Load( $"{file}", false )
				};
			}
		}

		int GetConvarInt( string convar, int fallback = 1 )
		{
			int i = fallback;
			var s = ConsoleSystem.GetValue( convar, fallback.ToString() );
			int.TryParse( s, out i );
			return i;
		}
	}
}
