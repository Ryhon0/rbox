using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class CButton : Button
{
	public virtual string IconPath => "/ui/weapons/weapon_tool.png";
	public Image Icon;
	public CButton()
	{
		Icon = Add.Image(IconPath, "icon");
	}
}

/*
[Library("cb_reload", Title = "Reload CButtons")]
public class ReloadCButtons : CButton
{
	public override string IconPath => "ui/cmenu/reload.png";
	public ReloadCButtons() : base()
	{
		AddEventListener( "onclick", () => CMenu.Current.LoadCButtons() );
	}
}
*/
