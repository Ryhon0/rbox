using Sandbox.Tools;
using System;

public static class ColorUtils
{
	public static byte ComponentToByte( float v ) => (byte)MathF.Floor( (v >= 1.0f) ? 255f : v * 256.0f );
	public static string ToHex( this Color c ) => $"#{ComponentToByte( c.r ).ToString( "X" )}{ComponentToByte( c.g ).ToString( "X" )}{ComponentToByte( c.b ).ToString( "X" )}";
	public static Color GetColorConvar( this BaseTool t, string cvar, Color fallback )
	{
		uint rgba = 0;
		var s = t.GetConvarValue( cvar, fallback.RGBA.ToString() );
		uint.TryParse( s, out rgba );
		return new Color( rgba );
	}
}
