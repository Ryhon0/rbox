using Sandbox;
using Sandbox.Tools;
using Sandbox.UI;

[Library( "weapon_tool", Title = "Toolgun" )]
partial class Tool : Carriable
{
	[ConVar.ClientData( "tool_current" )]
	public static string UserToolCurrent { get; set; } = "tool_boxgun";

	public override string ViewModelPath => "weapons/finger_guns/finger_guns.vmdl";

	[Net, Predicted]
	public BaseTool CurrentTool { get; set; }

	public override void Spawn()
	{
		base.Spawn();
	}

	public override void Simulate( Client owner )
	{
		UpdateCurrentTool( owner );
		CurrentTool?.Simulate();
	}

	private void UpdateCurrentTool( Client owner )
	{
		var toolName = owner.GetUserString( "tool_current", "tool_boxgun" );
		if ( toolName == null )
			return;

		// Already the right tool
		if ( CurrentTool != null && CurrentTool.Parent == this && CurrentTool.Owner == owner.Pawn && CurrentTool.ClassInfo.IsNamed( toolName ) )
			return;

		if ( CurrentTool != null )
		{
			CurrentTool?.Deactivate();
			CurrentTool = null;
		}

		CurrentTool = Library.Create<BaseTool>( toolName, false );

		if ( CurrentTool != null )
		{
			CurrentTool.Parent = this;
			CurrentTool.Owner = owner.Pawn as Player;
			CurrentTool.Activate();
		}
	}

	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart( ent );

		CurrentTool?.Activate();
	}

	public override void ActiveEnd( Entity ent, bool dropped )
	{
		base.ActiveEnd( ent, dropped );

		CurrentTool?.Deactivate();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		CurrentTool?.Deactivate();
		CurrentTool = null;
	}

	public override void OnCarryDrop( Entity dropper )
	{
	}

	[Event.Frame]
	public void OnFrame()
	{
		if ( !IsActiveChild() ) return;

		CurrentTool?.OnFrame();
	}
}

namespace Sandbox.Tools
{
	public partial class BaseTool : NetworkComponent
	{
		public Tool Parent { get; set; }
		public Player Owner { get; set; }

		protected virtual float MaxTraceDistance => 10000.0f;

		public virtual void Activate()
		{
			CreatePreviews();
		}

		public virtual void Deactivate()
		{
			DeletePreviews();
		}

		public virtual void Simulate()
		{

		}

		public virtual void OnFrame()
		{
			UpdatePreviews();
		}

		public virtual void CreateHitEffects( Vector3 pos )
		{
			Parent?.CreateHitEffects( pos );
		}

		public string GetConvarValue( string name, string defaultValue = null )
		{
			return Host.IsServer
				? Owner.GetClientOwner().GetUserString( name, defaultValue )
				: ConsoleSystem.GetValue( name, default );
		}

		public virtual Panel CreatePanel()
		{
			return null;
		}
	}
}
