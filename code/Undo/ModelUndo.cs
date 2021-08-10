using Sandbox;

public class ModelUndo : Undo
{
	ModelEntity Entity;
	public ModelUndo( ModelEntity ent )
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
		return $"Prop ({Entity.GetModelName()})";
	}
}
