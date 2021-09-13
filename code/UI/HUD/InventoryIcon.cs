
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class InventoryIcon : Panel
{
	public Entity TargetEnt;
	public Label Label;
	public Label Number;
	public int index;
	public bool clicked;

	public InventoryIcon( int i, Panel parent )
	{
		index = i;
		Parent = parent;
		Label = Add.Label( "empty", "item-name" );
		Number = Add.Label( $"{i}", "slot-number" );

		AddEventListener( "OnClick", ()=>
		{
			clicked = true;
		} );
	}

	[Event( "buildinput" )]
	public void ProcessClientInput( InputBuilder input )
	{
		if(clicked)
		{
			clicked = false;

			if(Local.Pawn is Player p)
				if(p.Inventory != null)
					if(p.Inventory.GetSlot(index-1) != null)
						input.ActiveChild = p.Inventory.GetSlot( index-1 );
		}
	}

	public void Clear()
	{
		Style.SetBackgroundImage("");
		Label.Text = "";
		SetClass( "active", false );
	}
}
