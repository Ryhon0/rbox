
using Sandbox;
using Sandbox.UI;

public partial class RKillFeed : Sandbox.UI.KillFeed
{
	public RKillFeed()
	{
		StyleSheet.Load( "/UI/KillFeed.scss" );
	}

	public override Panel AddEntry( ulong lsteamid, string left, ulong rsteamid, string right, string method )
	{
		Log.Info( $"{left} killed {right} using {method}" );

		var e = Current.AddChild<KillFeedEntry>();

		e.Icon.Style.Set( "background-image", $"url(/ui/weapons/{method}.png)" );

		e.Left.Text = left;
		e.Left.SetClass( "me", lsteamid == (Local.SteamId) );

		e.Right.Text = right;
		e.Right.SetClass( "me", rsteamid == (Local.SteamId) );

		return e;
	}
}
