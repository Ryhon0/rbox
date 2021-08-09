namespace Sandbox.Tools
{
	[Library( "tool_tube_balloon", Title = "Tube Balloons", Description = "Create Tube Balloons!", Group = "balloons" )]
	public partial class TubeBalloonTool : BalloonTool
	{
		public override string BalloonModelPath => "models/citizen_props/balloontall01.vmdl";
		public override BalloonEntity CreateBalloon()
			=> new TubeBalloonEntity();
	}
}
