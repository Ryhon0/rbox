using System.Collections.Generic;

public enum ClothingSlot
{
	Skin,
	Hat,
	Shirt,
	Pants,
	Gloves,
	Shoes,
}

public class Clothing
{
	public string Model { get; set; }
	/// null will use bone merge
	public string AttachmentPoint { get; set; }
	/// Only one layer of children will be added
	public List<Clothing> Children { get; set; }
	public bool CopyMaterial { get; set;}
	public int Material { get; set; }
	public bool Colorable { get; set; }
	public BodyMask BodyMask { get; set; } = BodyMask.All;
}
