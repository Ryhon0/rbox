using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using Sandbox.UI.Tests;
using System.Linq;

[Library]
public partial class EntityList : Panel
{
	VirtualScrollPanel Canvas;

	public EntityList()
	{
		AddClass( "spawnpage" );
		Reload();
	}

	[Event.Hotload]
	void Reload()
	{
		Canvas?.Delete();

		AddChild( out Canvas, "canvas entities" );

		Canvas.Layout.AutoColumns = true;
		Canvas.Layout.ItemSize = new Vector2( 100, 125 );
		Canvas.OnCreateCell = ( cell, data ) =>
		{
			var entry = (LibraryAttribute)data;
			var btn = cell.Add.Button( entry.Title );
			btn.AddClass( "icon" );
			btn.AddEventListener( "onclick", () => ConsoleSystem.Run( "spawn_entity", entry.Name ) );

			var entityicon = $"/entity/{entry.Name}.png";
			var weaponicon = $"/ui/weapons/{entry.Name}.png";
			string icn = "";
			if ( FileSystem.Mounted.FileExists( entityicon ) ) icn = entityicon;
			else if ( FileSystem.Mounted.FileExists( weaponicon ) ) icn = weaponicon;

			btn.Style.Background = new PanelBackground
			{
				Texture = Texture.Load( icn, false )
			};
		};

		var ents = Library.GetAllAttributes<Entity>().Where( x => x.Spawnable ).OrderBy( x => x.Title ).ToArray();

		foreach ( var entry in ents )
		{
			Canvas.AddItem( entry );
		}
	}
}
