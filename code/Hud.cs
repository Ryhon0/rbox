using Sandbox;
using Sandbox.UI;

public partial class Hud : HudEntity<RootPanel>
{
	public Hud()
	{
		if ( !IsClient )
			return;

		RootPanel.StyleSheet.Load( "/Hud.scss" );

		RootPanel.AddChild<NameTags>();
		RootPanel.AddChild<CrosshairCanvas>();
		RootPanel.AddChild<VoiceList>();
		RootPanel.AddChild<Scoreboard>();
		RootPanel.AddChild<Health>();
		RootPanel.AddChild<InventoryBar>();
		RootPanel.AddChild<CurrentTool>();
		RootPanel.AddChild<SpawnMenu>();

		RootPanel.AddChild<ClassicChatBox>();
		RootPanel.AddChild<KillFeed>();
	}
}
