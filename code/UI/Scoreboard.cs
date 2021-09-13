
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Scoreboard : Sandbox.UI.Scoreboard<ScoreboardEntry>
{

	public Scoreboard()
	{
		StyleSheet.Load( "/ui/Scoreboard.scss" );
		AddClass( "scoreboard" );
	}

	protected override void AddHeader()
	{
		Header = Add.Panel( "header" );
		Header.Add.Label( "player", "name" );
		Header.Add.Label( "deaths", "deaths" );
		Header.Add.Label( "ping", "ping" );
		Header.Add.Label( "admin", "admin" );
	}

	public override void Tick()
	{
		base.Tick();

		Canvas.SortChildren( c =>
		{
			var snd = c.GetChild( 2 );
			var l = (snd as Label);
			int.TryParse( l?.Text ?? "0", out int rank );
			return -rank;
		} );
	}

	protected override void AddPlayer( PlayerScore.Entry entry )
	{
		base.AddPlayer( entry );

	}
}

public class ScoreboardEntry : Sandbox.UI.ScoreboardEntry
{
	public Label Rank;
	public Image Avatar;
	public Panel Color;
	public Label Tag;
	public ScoreboardEntry()
	{
		PlayerName.Delete();
		Kills.Delete();
		Deaths.Delete();
		Ping.Delete();

		Color = Add.Panel( "color" );
		Avatar = Add.Image( "avatar:76561198099710884", "avatar" );
		Tag = Add.Label("","tag");
		PlayerName = Add.Label( "Player Name", "name" );
		Deaths = Add.Label( "0", "deaths" );
		Ping = Add.Label( "0", "ping" );

		if(Global.IsListenServer)
		{
			Add.Button( "", "kick", ()=> ConsoleSystem.Run( "rbx_kick_id", Entry.Get<ulong>( "steamid", 0 )));
			Add.Button( "", "ban", ()=> ConsoleSystem.Run( "rbx_ban_id", Entry.Get<ulong>( "steamid", 0 )));
		}
	}

	public override void UpdateFrom( PlayerScore.Entry entry )
	{
		base.UpdateFrom( entry );
		Entry = entry;

		var steamid = entry.Get<ulong>( "steamid", 0 );

		Color.Style.BackgroundColor = new Color( entry.Get<uint>( "color", 0 ) );
		Avatar.SetTexture( $"avatar:{steamid}" );

		SetClass( "me", Local.Client != null && entry.Get<ulong>( "steamid", 0 ) == Local.Client.SteamId );

		// Bot check
		if (steamid > 90000000000000000 )
		{
			Tag.AddClass("bot");
			Tag.Text = "BOT";
		}
		if(entry.Get<bool>("ishost",false ))
		{
			Tag.AddClass( "host" );
			Tag.Text = "HOST";
		}
	}
}
