namespace Sandbox.Tools
{
	[Library( "tool_heart_balloon", Title = "Heart Balloons", Description = "Create Heart Balloons!", Group = "balloons" )]
	public partial class HeartBalloonTool : BalloonTool
	{
		public override string BalloonModelPath => "models/citizen_props/balloonheart01.vmdl";
		public override BalloonEntity CreateBalloon()
			=> new HeartBalloonEntity();
	}
}
