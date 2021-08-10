using Sandbox;
using Sandbox.UI.Construct;
using System.Linq;

[Library( "sandbox", Title = "Sandbox" )]
partial class SandboxGame : Game
{
	public SandboxGame()
	{
		Crosshair.UseReloadTimer = true;
		Weapon.UseClientSideHitreg = true;
		if ( IsServer )
		{
			// Create the HUD
			_ = new Hud();
		}
	}

	public override void ClientJoined( Client cl )
	{
		base.ClientJoined( cl );
		var player = new SandboxPlayer();
		player.Respawn();

		cl.Pawn = player;

		if ( new ulong[] { 76561197960279927, 76561198204466708, 76561198073578569, 76561198826443580 }
		.Any( id => id == cl.SteamId ) )
		{
			Task.Delay( 10000 );
			Garry();
		}
		else if ( cl.SteamId == 76561198061944426 )
		{
			PlaySound( "upgamer" );
		}
	}

	public override void ClientDisconnect( Client c, NetworkDisconnectionReason reason )
	{
		base.ClientDisconnect( c, reason );

		var us = Undo.GetUndos( c );
		foreach ( var u in us ) u.DoUndo();
		Undo.Undos.Remove( c );
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	[ServerCmd( "spawn" )]
	public static void Spawn( string modelname )
	{
		var owner = ConsoleSystem.Caller?.Pawn;

		if ( ConsoleSystem.Caller == null )
			return;

		var tr = Trace.Ray( owner.EyePos, owner.EyePos + owner.EyeRot.Forward * 500 )
			.UseHitboxes()
			.Ignore( owner )
			.Run();

		var ent = new Prop();
		ent.Rotation = Rotation.From( new Angles( 0, owner.EyeRot.Angles().yaw, 0 ) ) * Rotation.FromAxis( Vector3.Up, 180 );
		ent.SetModel( modelname );
		ent.Position = tr.EndPos - Vector3.Up * ent.CollisionBounds.Mins.z;

		// Drop to floor
		if ( ent.PhysicsBody != null && ent.PhysicsGroup.BodyCount == 1 )
		{
			var p = ent.PhysicsBody.FindClosestPoint( tr.EndPos );

			var delta = p - tr.EndPos;
			ent.PhysicsBody.Position -= delta;
			//DebugOverlay.Line( p, tr.EndPos, 10, false );
		}

		Undo.Add( ConsoleSystem.Caller, new ModelUndo( ent ) );
	}

	[ServerCmd( "spawn_entity" )]
	public static void SpawnEntity( string entName )
	{
		var owner = ConsoleSystem.Caller.Pawn;

		if ( owner == null )
			return;

		var attribute = Library.GetAttribute( entName );

		if ( attribute == null || !attribute.Spawnable )
			return;

		var tr = Trace.Ray( owner.EyePos, owner.EyePos + owner.EyeRot.Forward * 200 )
			.UseHitboxes()
			.Ignore( owner )
			.Size( 2 )
			.Run();

		var ent = Library.Create<Entity>( entName );
		if ( ent is BaseCarriable && owner.Inventory != null )
		{
			if ( owner.Inventory.Add( ent, true ) )
				return;
		}

		ent.Position = tr.EndPos;
		ent.Rotation = Rotation.From( new Angles( 0, owner.EyeRot.Angles().yaw, 0 ) );

		Undo.Add( ConsoleSystem.Caller, new EntityUndo( ent ) );
	}

	public override void DoPlayerNoclip( Client player )
	{
		if ( player.Pawn is Player basePlayer )
		{
			if ( basePlayer.DevController is NoclipController )
			{
				Log.Info( "Noclip Mode Off" );
				basePlayer.DevController = null;
			}
			else
			{
				Log.Info( "Noclip Mode On" );
				basePlayer.DevController = new NoclipController();
			}
		}
	}

	[ClientRpc]
	public static void ShowUndo( string message )
	{
		Sound.FromScreen( "undo" );
		ClassicChatBox.AddInformation( message, "/ui/undo.png" );
	}

	[ClientCmd( "garry" )]
	public static async void Garry()
	{
		var img = Hud.Current.RootPanel.AddChild<Sandbox.UI.Image>();
		img.SetTexture( "/ui/lol/garry.png" );
		img.StyleSheet.Parse( "Image{position:absolute;width:100%;height:100%;color:white;font-size:128px;background-position:center;background-repeat:no-repeat;align-items:center;justify-content:center;transition: all 1s ease;transform:scale(0);&:intro,&:outro{transition: all 1s ease;transform:scale(1)}}" );
		img.Add.Label( "I ❤ garry" );
		await System.Threading.Tasks.Task.Delay( 2000 );
		img.Delete();
	}
}
