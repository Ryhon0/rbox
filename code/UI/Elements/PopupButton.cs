using Sandbox.UI.Construct;
using System;
using System.Linq;

namespace Sandbox.UI
{
	public class PopupButton : Button
	{
		protected Popup Popup;

		public PopupButton()
		{
			AddClass( "popupbutton" );
		}

		protected override void OnClick( MousePanelEvent e )
		{
			base.OnClick( e );

			Open();
		}

		public virtual void Open()
		{
			Popup = new Popup( this, Popup.PositionMode.BelowCenter, 4.0f );
			Popup.AddOption( "Override Me!" );
		}

		public override void Tick()
		{
			base.Tick();

			SetClass( "open", Popup != null && !Popup.IsDeleting );
			SetClass( "active", Popup != null && !Popup.IsDeleting );

			if ( Popup != null )
			{
				Popup.Style.Width = Box.Rect.width;
			}
		}
	}
}
