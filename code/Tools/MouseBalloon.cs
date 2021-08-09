namespace Sandbox.Tools
{
	[Library( "tool_mouse_balloon", Title = "Mouse Balloons", Description = "Create Mouse Balloons!", Group = "balloons" )]
	public partial class MouseBalloonTool : BalloonTool
	{
		public override string BalloonModelPath => "models/citizen_props/balloonears01.vmdl";
		public override BalloonEntity CreateBalloon()
			=> new MouseBalloonEntity();
	}
}
