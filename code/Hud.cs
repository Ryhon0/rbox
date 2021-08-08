using Sandbox;
using Sandbox.UI;

public partial class Hud : HudEntity<RootPanel>
{
	public static Hud Current;
	public Hud()
	{
		if ( !IsClient )
			return;

		Current = this;

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
		RootPanel.AddChild<RKillFeed>();

		RootPanel.AddChild<DamageIndicator>();
		RootPanel.AddChild<HitIndicator>();
	}
}
