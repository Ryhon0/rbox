using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Tests;
using Sandbox.UI.Construct;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

class CPropertiesMenu : Form
{
	bool Recursive = true;
	public Entity Target;
	public CPropertiesMenu(Entity ent)
	{
		Target = ent;

		var properties = Reflection.GetProperties( Target );
		if ( properties == null ) throw new System.Exception( "Oops" );

		// Make a field for each property
		foreach ( var group in properties.GroupBy( x => GetCategory( x ) ).OrderBy( x => x.Key ) )
		{
			Add.Label( group.Key , "category");

			foreach ( var prop in group.OrderBy( x => x.Name ) )
			{
				if ( !Recursive && prop.DeclaringType != Target.GetType() )
					continue;

				if ( prop.GetGetMethod() == null )
					continue;

				CreateControlFor( Target, prop );
			}
		}
	}

	public virtual void CreateControlFor( object obj, PropertyInfo prop )
	{
		if ( prop.PropertyType.IsEnum )
		{
			var control = new DropDown();

			var names = prop.PropertyType.GetEnumNames();
			var values = prop.PropertyType.GetEnumValues();

			for ( int i = 0; i < names.Length; i++ )
			{
				control.Options.Add( new Option( names[i], values.GetValue( i ).ToString() ) );
			}

			AddRow( prop, Target, control );
		}
		else if(prop.PropertyType == typeof(bool))
		{
			AddRow( prop, Target, new Checkbox());
		}
		else
		{
			AddRow( prop, Target, new TextEntry() );
		}
	}

	private string GetCategory( MemberInfo prop )
	{
		var category = prop.GetCustomAttribute<CategoryAttribute>();
		if ( category != null ) return category.Category;

		return "Misc";
	}
}
