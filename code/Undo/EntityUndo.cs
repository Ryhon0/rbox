using Sandbox;

public class EntityUndo : Undo
{
	Entity Entity;
	public EntityUndo( Entity ent )
	{
		Entity = ent;
	}

	public override bool DoUndo()
	{
		if ( !Entity.IsValid() ) return false;

		Entity.Delete();
		return true;
	}

	public override string ToString()
	{
		return $"{Entity.ClassInfo.Title ?? Entity.ClassInfo.Name}";
	}
}
