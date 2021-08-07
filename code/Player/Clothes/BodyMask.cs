public enum BodyMask : byte
{
	Head = 1,
	Chest = 2,
	Legs = 4,
	Hands = 8,
	Feet = 16,

	All = Head | Chest | Legs | Hands | Feet
}

public static class BodyMaskExtension
{
	public static int ToGroup( this BodyMask m )
	{
		switch ( m )
		{
			case BodyMask.Head: return 0;
			case BodyMask.Chest: return 1;
			case BodyMask.Legs: return 2;
			case BodyMask.Hands: return 3;
			case BodyMask.Feet: return 4;
			default: return -1;
		}
	}
}
