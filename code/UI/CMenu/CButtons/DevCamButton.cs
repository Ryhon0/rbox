using Sandbox;
using Sandbox.UI;

[Library("cb_devcam", Title = "DevCam")]
public class DevCamButton : CButton
{
	public override string IconPath => "ui/cmenu/devcam.png";
	public DevCamButton() : base()
	{
		AddEventListener( "onclick", () => ConsoleSystem.Run( "devcam" ) );
	}
}
