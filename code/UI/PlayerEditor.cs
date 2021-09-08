using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;

public class PlayerEditor : Panel
{
	public ScenePanel heroScene;
	Angles CamAngles;
	float startTime;
	AnimSceneObject Player;
	ColorPicker PlayerColor;
	List<AnimSceneObject> Clothes = new List<AnimSceneObject>();
	public PlayerEditor()
	{
		StyleSheet.Load( "/code/UI/PlayerEditor.scss" );
		LoadWorld();
		PlayerColor = AddChild<ColorPicker>( "playercolor" );

		PlayerColor.AddEventListener( "OnValueChanged",
			() => ChangePlayerColor( PlayerColor.Value.ToColor().RGBA ) );
	}

	[ServerCmd( "player_change_color" )]
	public static void ChangePlayerColor( uint color )
	{
		ConsoleSystem.Caller.SetScore( "color", color );
		(ConsoleSystem.Caller.Pawn as SandboxPlayer).PlayerColor = new Color( color );
	}

	void LoadWorld()
	{
		if ( heroScene != null ) return;

		using ( SceneWorld.SetCurrent( new SceneWorld() ) )
		{
			Player = new AnimSceneObject( Model.Load( "models/citizen/citizen.vmdl" ), Transform.Zero );

			//< light position = "-100 100 150" radius = "2000" falloff = "0.1" color = "0 200 300" ></ light >
			//< light position = "70 -30 170" radius = "2000" falloff = "0.2" ></ light >

			var light = new Light( new Vector3(-100,100,150), 2000, Color.White);
			light.Falloff = 0;
			//light.Falloff = 0f;
			light = new Light( new Vector3( 700, -30, 170 ), 2000, Color.White );
			light.Falloff = 0;

			//light.Falloff = 0.2f; 
			light = new Light( new Vector3( 100, 100, 150 ), 2000, Color.White );
			light.Falloff = 0;


			// Clothes
			//playerCostumePreview = new AnimSceneObject( Model.Load( "models/clothes/hotdog/hotdog.vmdl" ), Transform.Zero );
			//playerPreview.AddChild( "outfit", playerCostumePreview );
			startTime = Time.Now;
			heroScene = Add.ScenePanel( SceneWorld.Current, new Vector3( 175, 0, 30 ), Rotation.From(CamAngles), 25 );
			heroScene.AddClass( "preview" );

			Angles angles = new( 25, 180, 0 );
			Vector3 pos = Vector3.Up * 40 + angles.Direction * -200;

			heroScene.World = SceneWorld.Current;
			heroScene.Position = pos;
			heroScene.Angles = angles;
			heroScene.FieldOfView = 28;
			heroScene.AmbientColor = Color.Gray * 0.2f;
		}
	}

	[Event.Tick]
	void Tick()
	{
		base.Tick();
		CamAngles.yaw = 180;

		Player.Update( RealTime.Delta );
		foreach ( var c in Clothes )
		{
			c.Update( RealTime.Delta );
		}
	}
}
