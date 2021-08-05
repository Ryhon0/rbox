
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
	public ScoreboardEntry()
	{
		PlayerName.Delete();
		Kills.Delete();
		Deaths.Delete();
		Ping.Delete();

		Color = Add.Panel( "color" );
		Avatar = Add.Image( "avatar:76561198099710884", "avatar" );
		PlayerName = Add.Label( "Player Name", "name" );
		Deaths = Add.Label( "0", "deaths" );
		Ping = Add.Label( "0", "ping" );
	}

	public override void UpdateFrom( PlayerScore.Entry entry )
	{
		base.UpdateFrom( entry );

		Color.Style.BackgroundColor = new Color( entry.Get<uint>( "color", 0 ) );
		Avatar.SetTexture( $"avatar:{entry.Get<ulong>( "steamid", 0 )}" );

		SetClass( "me", Local.Client != null && entry.Get<ulong>( "steamid", 0 ) == Local.Client.SteamId );
	}
}
