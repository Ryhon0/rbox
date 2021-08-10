using Sandbox;
using System.Collections.Generic;
using System.Linq;

public abstract class Undo
{
	public static Dictionary<Client, Stack<Undo>> Undos = new Dictionary<Client, Stack<Undo>>();
	public static void Add( Client c, Undo u )
	{
		GetUndos( c ).Push( u );
		//Log.Info( "Added " + u.ToString() );
	}

	[AdminCmd( "cleanup" )]
	public static void Cleanup()
	{
		foreach ( var kv in Undos )
		{
			foreach ( var u in kv.Value )
			{
				u.DoUndo();
			}
		}
		Undos.Clear();
	}

	[ServerCmd( "undoall" )]
	public static void DeleteAll()
	{
		var c = ConsoleSystem.Caller;

		var us = GetUndos( c );
		if ( us.Any() ) SandboxGame.ShowUndo( To.Single( c ), "Undone all" );
		foreach ( var u in us )
		{
			//Log.Info( "Undoing " + u.ToString() );
			u.DoUndo();
		}
		us.Clear();
	}

	[ServerCmd( "undo" )]
	public static void UndoCmd()
	{
		var c = ConsoleSystem.Caller;

		var us = GetUndos( c );

		bool success = false;
		while ( !success )
		{
			if ( us.TryPop( out Undo u ) )
			{
				var s = "Undone " + u.ToString();
				success = u.DoUndo();
				if ( success ) SandboxGame.ShowUndo( To.Single( c ), s );
			}
			else success = true;
		}
	}

	public static Stack<Undo> GetUndos( Client c )
	{
		if ( Undos.ContainsKey( c ) )
			return Undos[c];
		else
		{
			var s = new Stack<Undo>();
			Undos.Add( c, s );
			return s;
		}
	}

	public abstract bool DoUndo();
}
