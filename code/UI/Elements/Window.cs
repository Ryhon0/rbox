using Sandbox;
using Sandbox.UI;
using System;

[UseTemplate]
public class Window : Panel
{
	public Label Title { get; set; }
	public Panel Header { get; set; }
	public Panel CloseButton { get; set; }
	public Panel Resizer { get; set; }
	public Panel Content { get; set; }

	public bool Resizable
	{
		get => Resizer.Parent.HasClass( "visible" );
		set => Resizer.Parent.SetClass( "visible", value );
	}
	public bool Dragging;
	public bool Resizing;
	public Vector2 DragOffset;

	public static Window With( Panel p )
	{
		var w = new Window();

		w.Content.AddChild( p );
		return w;
	}

	public Window WithTitle( string title )
	{
		Title.Text = title;
		return this;
	}

	public Window WithMinSize( float width, float height )
	{
		Style.MinWidth = width;
		Style.MinHeight = height;
		return this;
	}

	public Window WithSize( float width, float height )
	{
		Style.Width = width;
		Style.Height = height;
		FinalLayout();
		return this;
	}

	public Window WithResizable( bool resizable = true )
	{
		Resizable = resizable;
		return this;
	}

	public Window()
	{
		StyleSheet.Load( "/ui/Elements/Window.scss" );

		Header.AddEventListener( "OnMouseDown", () =>
		{
			Dragging = true;
			DragOffset = Header.MousePosition;
		} );
		Header.AddEventListener( "OnMouseUp", () => Dragging = false );

		Resizer.AddEventListener( "OnMouseDown", () =>
		{
			Resizing = true;
			DragOffset = Resizer.MousePosition;
		} );
		Resizer.AddEventListener( "OnMouseUp", () => Resizing = false );

		CloseButton.AddEventListener( "OnClick", () => Delete() );
	}

	public void Center()
	{
		Style.Dirty();
		var screen = Screen.Size;

		var w = 0f;
		if ( Style.Width.HasValue )
			w = Style.Width.Value.Value;
		var h = 0f;
		if ( Style.Height.HasValue )
			w = Style.Height.Value.Value;

		Log.Info( w );
		Log.Info( h );

		Move( (screen.x / 2) - (w / 2), (screen.y / 2) - (h / 2) );
	}

	public void Resize( Vector2 vec )
		=> Resize( vec.x, vec.y );

	public void Resize( float width, float height )
	{
		Vector2 minimun = Vector2.Zero;

		if ( Style.MinWidth.HasValue )
			minimun.x = Style.MinWidth.Value.Value;

		if ( Style.MinHeight.HasValue )
			minimun.y = Style.MinHeight.Value.Value;

		Style.Width = Math.Max( minimun.x, width );
		Style.Height = Math.Max( minimun.y, height );
		Style.Dirty();
	}

	public void Move( Vector2 vec )
		=> Move( vec.x, vec.y );

	public void Move( float x, float y )
	{
		if ( Style.Left.HasValue )
			x += Style.Left.Value.Value;

		if ( Style.Top.HasValue )
			y += Style.Top.Value.Value;

		Style.Left = x;
		Style.Top = y;
		Style.Dirty();
	}

	[Event.Frame]
	void Frame()
	{
		if ( Resizing )
		{
			var size = MousePosition + DragOffset * 2;
			Resize( size.x, size.y );
		}
		else if ( Dragging )
		{
			var pos = MousePosition - DragOffset;
			Move( pos );
		}
	}
}
