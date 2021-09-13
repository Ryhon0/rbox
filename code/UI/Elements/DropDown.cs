using Sandbox.Html;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sandbox.UI
{
	public class Option
	{
		public string Title;
		public string Icon;
		public string Subtitle;
		public string Tooltip;
		public object Value;

		public Option()
		{

		}

		public Option( string title, string value )
		{
			Title = title;
			Value = value;
		}
	}

	[Library( "select" )]
	public class DropDown : PopupButton
	{
		protected IconPanel DropdownIndicator;

		public List<Option> Options { get; } = new();

		public DropDown()
		{
			AddClass( "dropdown" );
			DropdownIndicator = Add.Icon( "expand_more", "dropdown_indicator" );
			StyleSheet.Load( "UI/Elements/DropDown.scss" );
		}

		public override void SetProperty( string name, string value )
		{
			base.SetProperty( name, value );

			if ( name == "value" )
			{
				Select( value, false );
			}
		}

		public override void Open()
		{
			Popup = new Popup( this, Popup.PositionMode.BelowStretch, 4.0f );
			Popup.AddClass( "flat-top" );

			foreach ( var option in Options )
			{
				var o = Popup.AddOption( option.Title, option.Icon, () => Select( option ) );
				if ( Selected != null && option.Value == Selected.Value )
				{
					o.AddClass( "active" );
				}
			}
		}

		protected virtual void Select( Option option, bool triggerChange = true )
		{
			if ( !triggerChange )
			{
				selected = option;

				if ( option != null )
				{
					Value = $"{option.Value}";
					Icon = option.Icon;
					Text = option.Title;
				}
			}
			else
			{
				Selected = option;
			}
		}

		protected virtual void Select( string value, bool triggerChange = true )
		{
			if ( Value == value ) return;
			Value = value;

			Select( Options.FirstOrDefault( x => string.Equals( x.Value.ToString(), value, StringComparison.OrdinalIgnoreCase ) ), triggerChange );
		}

		public string Value { get; protected set; }

		Option selected;

		public Option Selected
		{
			get => selected;
			set
			{
				if ( selected == value ) return;

				selected = value;

				if ( selected != null )
				{
					Value = $"{selected.Value}";
					Icon = selected.Icon;
					Text = selected.Title;

					CreateValueEvent( "value", selected?.Value );
				}
			}
		}

		public override bool OnTemplateElement( INode element )
		{
			Options.Clear();

			foreach ( var child in element.Children )
			{
				if ( !child.IsElement ) continue;

				if ( child.Name.Equals( "option", StringComparison.OrdinalIgnoreCase ) )
				{
					var o = new Option();

					o.Title = child.InnerHtml;
					o.Value = child.GetAttribute( "value", o.Title );
					o.Icon = child.GetAttribute( "icon", null );

					Options.Add( o );
				}
			}

			Select( Value );
			return true;
		}
	}
}
