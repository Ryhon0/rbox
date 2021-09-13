using Sandbox;

public partial class SandboxPlayer
{
	[ServerCmd("switchview")]
	public static void SwitchView()
	{
		if ( ConsoleSystem.Caller.Pawn is not SandboxPlayer p ) return;

		if ( p.MainCamera is not FirstPersonCamera )
		{
			p.MainCamera = new FirstPersonCamera();
		}
		else
		{
			p.MainCamera = new ThirdPersonCamera();
		}
	}
}
