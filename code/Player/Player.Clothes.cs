using Sandbox;
using System.Collections.Generic;

partial class SandboxPlayer
{
	ModelEntity pants;
	ModelEntity jacket;
	ModelEntity shoes;
	ModelEntity hat;

	bool dressed = false;

	BodyMask BodyMask = BodyMask.All;
	void ApplyBodyMask( BodyMask mask )
	{
		BodyMask = BodyMask & mask;

		if ( BodyMask == 0 )
		{
			for ( int i = 0; i < 4; i++ )
				SetBodyGroup( i, 0 );
			RenderAlpha = 0;
		}
		else
		{
			RenderAlpha = 1;
			SetBodyGroup( BodyMask.Head.ToGroup(), BodyMask.HasFlag( BodyMask.Head ) ? 0 : 1 );
			SetBodyGroup( BodyMask.Chest.ToGroup(), BodyMask.HasFlag( BodyMask.Chest ) ? 0 : 1 );
			SetBodyGroup( BodyMask.Legs.ToGroup(), BodyMask.HasFlag( BodyMask.Legs ) ? 0 : 1 );
			SetBodyGroup( BodyMask.Hands.ToGroup(), BodyMask.HasFlag( BodyMask.Hands ) ? 0 : 1 );
			SetBodyGroup( BodyMask.Feet.ToGroup(), BodyMask.HasFlag( BodyMask.Feet ) ? 0 : 1 );
		}
	}

	public void Dress()
	{
		if ( !IsServer ) return;

		if ( dressed ) return;
		//dressed = true;

		Wear(
		FileSystem.Mounted.ReadJson<List<Clothing>>( "/config/clothes/skin.json" ).GetRandom(),
		ClothingSlot.Skin );
		WearHeadset();

		return;

		if ( true )
		{
			var model = Rand.FromArray( new[]
			{
				"models/citizen_clothes/trousers/trousers.jeans.vmdl",
				"models/citizen_clothes/trousers/trousers.lab.vmdl",
				"models/citizen_clothes/trousers/trousers.police.vmdl",
				"models/citizen_clothes/trousers/trousers.smart.vmdl",
				"models/citizen_clothes/trousers/trousers.smarttan.vmdl",
				"models/citizen_clothes/trousers/trousers_tracksuitblue.vmdl",
				"models/citizen_clothes/trousers/trousers_tracksuit.vmdl",
				"models/citizen_clothes/shoes/shorts.cargo.vmdl",
			} );

			pants = Wear( model );

			//ApplyBodyMask( BodyMask.Chest | BodyMask.Head);
		}

		if ( true )
		{
			var model = Rand.FromArray( new[]
			{
				"models/citizen_clothes/jacket/labcoat.vmdl",
				"models/citizen_clothes/jacket/jacket.red.vmdl",
				"models/citizen_clothes/jacket/jacket.tuxedo.vmdl",
				"models/citizen_clothes/jacket/jacket_heavy.vmdl",
			} );

			jacket = Wear( model );
		}

		if ( true )
		{
			var model = Rand.FromArray( new[]
			{
				"models/citizen_clothes/shoes/trainers.vmdl",
				"models/citizen_clothes/shoes/shoes.workboots.vmdl"
			} );

			shoes = Wear( model );
		}

		if ( true )
		{
			var model = Rand.FromArray( new[]
			{
				"models/citizen_clothes/hat/hat_hardhat.vmdl",
				"models/citizen_clothes/hat/hat_woolly.vmdl",
				"models/citizen_clothes/hat/hat_securityhelmet.vmdl",
				"models/citizen_clothes/hair/hair_malestyle02.vmdl",
				"models/citizen_clothes/hair/hair_femalebun.black.vmdl",
				"models/citizen_clothes/hat/hat_beret.red.vmdl",
				"models/citizen_clothes/hat/hat.tophat.vmdl",
				"models/citizen_clothes/hat/hat_beret.black.vmdl",
				"models/citizen_clothes/hat/hat_cap.vmdl",
				"models/citizen_clothes/hat/hat_leathercap.vmdl",
				"models/citizen_clothes/hat/hat_leathercapnobadge.vmdl",
				"models/citizen_clothes/hat/hat_securityhelmetnostrap.vmdl",
				"models/citizen_clothes/hat/hat_service.vmdl",
				"models/citizen_clothes/hat/hat_uniform.police.vmdl",
				"models/citizen_clothes/hat/hat_woollybobble.vmdl",
			} );

			hat = Wear( model );
			hat.Tags.Add( "hat" );
		}
	}

	public void Wear( Clothing clothing, ClothingSlot slot )
	{
		if ( slot == ClothingSlot.Skin )
		{
			SetModel( clothing.Model );
			SetMaterialGroup( clothing.Material );
			if ( clothing.Colorable )
			{
				Tags.Add( "colorable" );
				RenderColor = PlayerColor;
			}
			else
			{
				RenderColor = Color.White;
				Tags.Remove( "colorable" );
			}

			if ( clothing.Children != null )
				foreach ( var c in clothing.Children )
				{
					var m = new ModelEntity( c.Model );
					m.Tags.Add( "clothing" );
					m.EnableShadowInFirstPerson = true;
					m.EnableHideInFirstPerson = true;


					if ( c.AttachmentPoint == null ) m.SetParent( this, true );
					else m.SetParent( this, c.AttachmentPoint );

					if ( c.CopyMaterial ) m.SetMaterialGroup( GetMaterialGroup() );
					else m.SetMaterialGroup( c.Material );

					if ( c.Colorable )
					{
						m.Tags.Add( "colorable" );
						m.RenderColor = PlayerColor;
					}
					else
					{
						m.RenderColor = Color.White;
						m.Tags.Remove( "colorable" );
					}

					ApplyBodyMask( c.BodyMask );
				}
		}
		else
		{
			var m = new ModelEntity( clothing.Model );
			m.Tags.Add( "clothing" );
			m.EnableShadowInFirstPerson = true;
			m.EnableHideInFirstPerson = true;

			if ( clothing.AttachmentPoint == null ) m.SetParent( this, true );
			else m.SetParent( this, clothing.AttachmentPoint );

			if ( clothing.CopyMaterial ) m.SetMaterialGroup( GetMaterialGroup() );
			else m.SetMaterialGroup( clothing.Material );

			if ( clothing.Colorable )
			{
				m.Tags.Add( "colorable" );
				m.RenderColor = PlayerColor;
			}
			else
			{
				m.RenderColor = Color.White;
				m.Tags.Remove( "colorable" );
			}

			if ( clothing.Children != null )
				foreach ( var c in clothing.Children )
				{
					var cm = new ModelEntity( c.Model );
					cm.Tags.Add( "clothing" );
					cm.EnableShadowInFirstPerson = true;
					cm.EnableHideInFirstPerson = true;

					if ( c.AttachmentPoint == null ) cm.SetParent( this, true );
					else cm.SetParent( this, c.AttachmentPoint );

					if ( c.CopyMaterial ) m.SetMaterialGroup( GetMaterialGroup() );
					else cm.SetMaterialGroup( c.Material );

					if ( c.Colorable )
					{
						cm.Tags.Add( "colorable" );
						cm.RenderColor = PlayerColor;
					}
					else
					{
						cm.RenderColor = Color.White;
						cm.Tags.Remove( "colorable" );
					}

					ApplyBodyMask( c.BodyMask );
				}
		}

		ApplyBodyMask( clothing.BodyMask );
	}

	public ModelEntity Wear( string path )
	{
		var outfit = new ModelEntity();

		outfit.SetModel( path );
		outfit.SetParent( this, true );
		outfit.EnableShadowInFirstPerson = true;
		outfit.EnableHideInFirstPerson = true;
		outfit.Tags.Add( "clothing" );

		return outfit;
	}

	public void RemoveClothes()
	{
		dressed = false;
		foreach ( var c in Children )
		{
			if ( c.Tags.Has( "clothes" ) ) c.Delete();
		}
	}
}
