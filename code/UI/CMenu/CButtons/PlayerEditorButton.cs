using Sandbox;
using Sandbox.UI;

[Library( "cb_player", Title = "cb_player" )]
public class CBPlayer : CButton
{
	public override string IconPath => "ui/cmenu/player.png";

	Window current;
	public CBPlayer() : base()
	{
		AddEventListener( "onclick", () =>
		{
			if ( current?.Parent == null )
			{
				current = Window.With( new PlayerEditor() )
					.WithSize( 450, 500 )
					.WithTitle( "Player" )
					.WithResizable( false );

				CMenu.Current.AddChild( current );

				current.Center();
			}
			else current.Delete();
		} );
	}

	public override void OnDeleted()
	{
		base.OnDeleted();
		current?.Delete();
	}
}
