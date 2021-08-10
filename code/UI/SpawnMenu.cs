
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;
using System.Linq;

[Library]
public partial class SpawnMenu : Panel
{
	public static SpawnMenu Instance;
	public Panel ToolList;

	public SpawnMenu()
	{
		Instance = this;

		StyleSheet.Load( "/ui/SpawnMenu.scss" );

		var left = Add.Panel( "left" );
		{
			var tabs = left.AddChild<ButtonGroup>();
			tabs.AddClass( "tabs" );

			var body = left.Add.Panel( "body" );

			{
				var props = body.AddChild<SpawnList>();
				tabs.SelectedButton = tabs.AddButtonActive( "Props", ( b ) => props.SetClass( "active", b ) );

				var ents = body.AddChild<EntityList>();
				tabs.AddButtonActive( "Entities", ( b ) => ents.SetClass( "active", b ) );
			}
		}

		var right = Add.Panel( "right" );
		{
			Panel body = null;
			var tabs = right.Add.ButtonGroup( "tabs" );
			{
				tabs.Add.Button( "Player" );
				tabs.Add.Button( "Tools" );
				tabs.Add.Button( "Utility" );

				tabs.AddEventListener( "startactive", () =>
				{
					if ( body == null ) return;
					var id = tabs.GetChildIndex( tabs.SelectedButton );
					foreach ( var c in body?.Children )
					{
						var i = c.Parent.GetChildIndex( c );
						c.SetClass( "visible", i == id ); ;
					}
				} );

				tabs.SelectedButton = tabs.GetChild( 0 );
			}

			body = right.Add.Panel( "body" );
			{
				var player = body.AddChild<PlayerEditor>( "page visible" );

				ToolList = body.Add.Panel( "page toollist" );
				ReloadTools();

				var utils = body.Add.Panel( "page util" );
				{
					utils.Add.Button( "Undo", () => ConsoleSystem.Run( "undo" ) );
					utils.Add.Button( "Undo All", () => ConsoleSystem.Run( "undoall" ) );
					utils.Add.Button( "(Admin) Cleanup", () => ConsoleSystem.Run( "cleanup" ) );
					utils.Add.Label( "You can call undo/undoall/cleanup from console too!" );
				}
			}
		}

	}

	public override void Tick()
	{
		base.Tick();

		Parent.SetClass( "spawnmenuopen", Input.Down( InputButton.Menu ) );
	}

	[Event.Hotload]
	void ReloadTools()
	{
		ToolList.DeleteChildren();
		var tools = Library.GetAllAttributes<Sandbox.Tools.BaseTool>().Where( t => t.Title != "BaseTool" );
		var groupnames = tools.Select( t => t.Group ).Distinct().OrderBy( g => g );
		Dictionary<string, Panel> groups = new();
		foreach ( var g in groupnames )
		{
			var p = ToolList.Add.Panel( "group" );
			groups[g] = p;
			p.Add.Label( g, "groupname" );
		}

		foreach ( var entry in tools )
		{
			var button = groups[entry.Group].Add.Button( entry.Title );
			button.SetClass( "active", entry.Name == ConsoleSystem.GetValue( "tool_current" ) );

			button.AddEventListener( "onclick", () =>
			{
				ConsoleSystem.Run( "tool_current", entry.Name );
				ConsoleSystem.Run( "inventory_current", "weapon_tool" );

				foreach ( var child in ToolList.Descendants )
				{
					child.SetClass( "active", child == button );
				}
			} );
		}
	}
}
