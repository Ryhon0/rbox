
using Sandbox;

public class WeldUndo : Undo
{
	Prop Prop;
	public WeldUndo( Prop p )
	{
		Prop = p;
	}
	public override bool DoUndo()
	{
		if ( !Prop.IsValid() ) return false;

		Prop.Unweld( true );
		return true;
	}

	public override string ToString()
		=> "Weld";
}
