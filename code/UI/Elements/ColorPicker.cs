using Sandbox;
using Sandbox.UI;
using System;

public class ColorPicker : Panel
{
	Panel Color;
	Panel Current;
	Slider HueSlider;
	public ColorHsv Value
	{
		get => _value;
		set
		{
			_value = value;
			OnValueChanged();
		}
	}
	ColorHsv _value = new ColorHsv( 0, 1, 1 );
	bool Dragging;
	public ColorPicker()
	{
		Color = Add.Panel( "color" );
		Color.AddEventListener( "OnMouseDown", () => Dragging = true );
		Color.AddEventListener( "OnMouseUp", () => Dragging = false );

		Current = Color.Add.Panel( "current" );

		Color.Style.SetBackgroundImage( CreateColorTexture( new Rect( 0, 0, 250, 250 ) ) );

		HueSlider = AddChild<Slider>( "hueslider" );
		HueSlider.AddEventListener( "valuechanged", () =>
		{
			Value = Value.WithHue( HueSlider.Value * 360 );
			UpdateColor();
		} );
		HueSlider.Value = 0;
	}

	Rect oldSize;
	[Event.Frame]
	void Frame()
	{
		var newsize = Color.Box.Rect;

		if ( newsize != oldSize )
		{
			UpdateColor();
			oldSize = newsize;
		}

		if ( Dragging )
		{
			var box = Color.Box.Rect;
			var pos = Color.MousePosition;

			Value = Value
				.WithSaturation( Math.Clamp( pos.x / box.width, 0, 1 ) )
				.WithValue( Math.Clamp( (box.height - pos.y) / box.height, 0, 1 ) );
		}
	}

	void UpdateColor()
	{
		Current.Style.BackgroundColor = Value;
		Current.Style.Left = Length.Fraction( Value.Saturation );
		Current.Style.Top = Length.Fraction( 1 - Value.Value );
		Current.Style.Dirty();

		Color.Style.SetBackgroundImage( CreateColorTexture( Color.Box.Rect, Value.Value ) );
	}

	public void OnValueChanged()
	{
		CreateEvent( "OnValueChanged" );
		UpdateColor();
	}

	Texture CreateColorTexture( Rect size, float hue = 0 )
		=> CreateColorTexture( (int)size.width, (int)size.height, hue );
	Texture CreateColorTexture( int w, int h, float hue = 0 )
	{
		if ( w <= 0 || h <= 0 ) return null;

		var hsv = new ColorHsv( hue, 1, 1 );
		var img = new byte[w * h * 4];

		void SetColor( int x, int y, Color col )
		{
			img[((x + (y * w)) * 4)] = ColorUtils.ComponentToByte( col.r );
			img[((x + (y * w)) * 4) + 1] = ColorUtils.ComponentToByte( col.g );
			img[((x + (y * w)) * 4) + 2] = ColorUtils.ComponentToByte( col.b );
			img[((x + (y * w)) * 4) + 3] = 255;
		}

		for ( int x = 0; x < w; x++ )
		{
			for ( int y = 0; y < h; y++ )
			{
				var c = new ColorHsv()
					.WithHue( Value.Hue )
					.WithSaturation( (float)x / w )
					.WithValue( (float)(h - y) / h );
				SetColor( x, y, c );
			}
		}

		return Texture.Create( w, h ).WithStaticUsage().WithData( img ).WithName( "color" ).Finish();
	}
}
