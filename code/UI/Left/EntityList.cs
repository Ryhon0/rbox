using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Linq;

[Library]
public partial class EntityList : Panel
{
	public EntityList()
	{
		AddClass( "spawnpage" );
		Reload();
	}

	[Event.Hotload]
	void Reload()
	{
		DeleteChildren();

		var ents = Library.GetAllAttributes<Entity>().Where( x => x.Spawnable ).OrderBy( x => x.Title );

		foreach ( var g in ents.Select( e => e.Group ).Distinct() )
		{
			var p = Add.Panel( "group canvas entities" );
			p.Add.Label( g ?? "null", "title" );
			var list = p.Add.Panel( "canvas entities" );

			foreach ( var entry in ents.Where( e => e.Group == g ) )
			{
				var btn = list.Add.Button( entry.Title, "icon cell" );

				btn.Style.Width = 100;
				btn.Style.Height = 100;

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
			}
		}
	}
}
