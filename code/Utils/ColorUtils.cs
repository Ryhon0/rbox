using System;

public static class ColorUtils
{
	public static byte ComponentToByte( float v ) => (byte)MathF.Floor( (v >= 1.0f) ? 255f : v * 256.0f );
	public static string ToHex( this Color c ) => $"#{ComponentToByte( c.r ).ToString( "X" )}{ComponentToByte( c.g ).ToString( "X" )}{ComponentToByte( c.b ).ToString( "X" )}";
}
