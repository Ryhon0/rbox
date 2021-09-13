using System;
using Sandbox;
using Sandbox.UI;

public class CMenu : Panel
{
	public static CMenu Current;
	Panel Buttons;

	public CMenu()
	{
		Current = this;
		StyleSheet.Load( "UI/CMenu/CMenu.scss" );

		Buttons = Add.Panel( "cbuttons" );

		LoadCButtons();
	}

	protected override void OnRightClick( MousePanelEvent e )
	{
		base.OnRightClick( e );

		var from = Local.Pawn.EyePos;
		var dir = Screen.GetDirection( MousePosition );
		var to = from + dir * 1000;

		var rr = Trace.Ray( from, to )
			.Ignore( Local.Pawn )
			.Run();

		if(rr.Entity.IsValid() && !rr.Entity.IsWorld)
		{
			var ent = rr.Entity;
			var window = Window.With( new CPropertiesMenu( rr.Entity ) )
				.WithSize( 200, 400 )
				.WithTitle( $"{ent.ClassInfo.Name}({ent.NetworkIdent}, " + (ent.IsClientOnly ? "Client" : "Server") + ")");

			CMenu.Current.AddChild( window );
			window.Move( MousePosition );
		}
	}

	ModelEntity LastEnt;
	public override void Tick()
	{
		base.Tick();

		SetClass( "open",
			Input.Down( InputButton.View )
			|| (InputFocus.Current is TextEntry &&
			InputFocus.Current.IsDescendantOf( this )) );

		if ( LastEnt.IsValid() )
			LastEnt.GlowActive = false;

		if (HasClass("open"))
		{
			var from = Local.Pawn.EyePos;
			var dir = Screen.GetDirection( MousePosition );
			var to = from + dir * 1000;

			var rr = Trace.Ray( from, to )
				.Ignore( Local.Pawn )
				.Run();

			if ( rr.Entity.IsValid() )
			{
				DebugOverlay.ScreenText( rr.Entity.ClassInfo.Name, 0 );
				if(rr.Entity is ModelEntity me)
				{
					LastEnt = me;

					LastEnt.GlowColor = Color.White;
					LastEnt.GlowActive = true;
					LastEnt.GlowState = GlowStates.GlowStateOn;
					LastEnt.GlowDistanceStart = 0;
					LastEnt.GlowDistanceEnd = int.MaxValue;
				}
			}
		}
	}

	public void LoadCButtons()
	{
		Buttons.DeleteChildren();
		var btns = Library.GetAll<CButton>();

		foreach(var b in btns)
		{
			if ( b == typeof( CButton ) ) continue;

			var btn = Library.Create<CButton>( b );
			btn.AddClass( "cbutton" );
			Buttons.AddChild( btn );
		}
	}
}
