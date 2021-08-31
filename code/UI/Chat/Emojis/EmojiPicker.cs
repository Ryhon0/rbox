using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;
using System.Linq;

public class EmojiPicker : Panel
{
	public TextEntry Search;
	Panel EmojiPanel;
	EmojiPanel CurrentPanel;

	static Dictionary<string, string> GroupEmojis = new Dictionary<string, string>()
	{
		{ "Animals & Nature", "🐱" },
		{ "Smileys & People", "😃" },
		{ "Travel & Places", "✈" },
		{ "Flags", "🏳" },
		{ "Objects", "🔑" },
		{ "Activities", "⚽" },
		{ "Symbols", "➡" },
		{ "Food & Drink", "🍔" },
		{ "Extra", "⭐" },
	};

	public EmojiPicker()
	{
		StyleSheet.Load( "UI/Chat/Emojis/EmojiPicker.scss" );

		var tabs = Add.ButtonGroup();
		EmojiPanel = Add.Panel( "emojipanel" );
		Search = Add.TextEntry( "" );
		Search.Placeholder = "Click to search";
		Search.AllowEmojiReplace = false;
		Search.AddClass( "emojisearch" );
		Search.AddEventListener( "onchange", () => UpdateSearch() );

		foreach ( var cat in Emojis.Categories )
		{
			if ( cat.Name == "Skin Tones" ) continue;

			var page = EmojiPanel.AddChild<EmojiPanel>("emojilist" );
			page.Emojis = cat.Emojis.ToList();

			if ( CurrentPanel == null ) CurrentPanel = page;

			tabs.AddButtonActive( GroupEmojis.GetValueOrDefault( cat.Name ) ?? cat.Name,
				( b ) =>
				{
					page.SetClass( "active", b );
					if ( b )
					{
						CurrentPanel = page;
						if(HasClass("open"))
							page.OnShow();
					}
					else page.OnHide();
				});
			
			tabs.SelectedButton = tabs.GetChild( 0 );
		}
	}

	void UpdateSearch()
	{
		var query = Search.Text;
		foreach ( var page in EmojiPanel.Children )
		{
			foreach ( var e in page.Children )
			{
				if ( e is EmojiButton b )
				{
					var found = !b.Names.Any( n => n.ToLower().Replace( "_", "" ).Replace( " ", "" ).
						Contains( query.ToLower().Replace( "_", "" ).Replace( " ", "" ) ) );
					b.SetClass( "hidden", found );
				}
			}
		}
	}

	public void OnShow()
	{
		CurrentPanel.OnShow();
	}

	public void OnHide()
	{
		CurrentPanel.OnHide();
	}
}

public class EmojiPanel : Panel
{
	public List<Emoji> Emojis;
	public void OnShow()
	{
		foreach ( var e in Emojis )
		{
			var eb = AddChild<EmojiButton>();
			eb.Text = e.Unicode;
			eb.Names = e.Names;
			eb.AddClass( "emoji" );
			eb.AddEventListener( "onclick", () => ClassicChatBox.Current.InsertEmoji( e.Unicode ) );
		}
	}
	public void OnHide()
	{
		DeleteChildren();
	}
}

public class EmojiButton : Button
{
	public List<string> Names;
}
