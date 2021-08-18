public static class VectorExtensions
{
	public static Rotation LookAt( this Vector3 from, Vector3 to )
	{
		var diff = to - from;
		return Rotation.LookAt( diff.Normal );
	}
}
