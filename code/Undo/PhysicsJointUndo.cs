using Sandbox;

public class PhysicsJointUndo : Undo
{
	IPhysicsJoint Joint;
	public PhysicsJointUndo( IPhysicsJoint j )
	{
		Joint = j;
	}

	public override bool DoUndo()
	{
		if ( !Joint.IsValid ) return false;

		Joint.Remove();
		return true;
	}

	public override string ToString()
	{
		return "Physics Joint";
	}
}
