using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;

public class PlayerEditor : Panel
{
	public Scene heroScene;
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

			Light.Point( Vector3.Up * 10.0f + Vector3.Forward * 100.0f + Vector3.Right * 100.0f, 2000, Color.White * 15000.0f );
			Light.Point( Vector3.Up * 10.0f + Vector3.Forward * 100.0f + Vector3.Right * 100.0f, 2000, Color.White * 15000.0f );

			// Clothes
			//playerCostumePreview = new AnimSceneObject( Model.Load( "models/clothes/hotdog/hotdog.vmdl" ), Transform.Zero );
			//playerPreview.AddChild( "outfit", playerCostumePreview );
			startTime = Time.Now;

			heroScene = Add.Scene( SceneWorld.Current, new Vector3( 175, 0, 30 ), CamAngles, 25 );
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

		Player.Update( Time.Now - startTime );
		foreach ( var c in Clothes )
		{
			c.Update( Time.Now - startTime );
		}
	}
}
