using Sandbox;
using System;
using System.Linq;

partial class SandboxPlayer
{
	[ServerCmd( "explode", Help = "Explode yourself" )]
	public static void Explode()
	{
		var target = ConsoleSystem.Caller;
		var p = target.Pawn as Player;
		if ( p != null )
		{
			p.TakeDamage( DamageInfo.FromBullet( p.PhysicsBody.MassCenter, Vector3.Zero, p.Health ).WithFlag( DamageFlags.Blast ) );
		}
	}

	public override void OnKilled()
	{
		base.OnKilled();

		VehicleController = null;
		VehicleAnimator = null;
		VehicleCamera = null;
		Vehicle = null;

		LastCamera = MainCamera;
		MainCamera = new SpectateRagdollCamera();
		Camera = MainCamera;
		Controller = null;

		Inventory.DeleteContents();

		if(IsServer)
		{
			if ( LastDamage.Flags.HasFlag( DamageFlags.Blast ) || LastDamage.Flags.HasFlag( DamageFlags.AlwaysGib ) )
			{
				Gib();
			}
			else
			{
				if ( LastDamage.Flags.HasFlag( DamageFlags.Vehicle ) )
				{
					Particles.Create( "particles/impact.flesh.bloodpuff-big.vpcf", LastDamage.Position );
					Particles.Create( "particles/impact.flesh-big.vpcf", LastDamage.Position );
					PlaySound( "kersplat" );
				}

				BecomeRagdoll( LastDamage.Force, GetHitboxBone( LastDamage.HitboxIndex ) );
			}
		}

		Camera = new SpectateRagdollCamera();

		RemoveClothes();
		RemoveAllDecals();

		Controller = null;
		Camera = new SpectateRagdollCamera();

		EnableAllCollisions = false;
		EnableDrawing = false;
	}

	// TODO - make ragdolls one per entity
	// TODO - make ragdolls dissapear after a load of seconds
	static EntityLimit RagdollLimit = new EntityLimit { MaxTotal = 10 };
	static EntityLimit GibLimit = new EntityLimit { MaxTotal = 30 };

	void Gib()
	{
		var pos = Position + Vector3.Up * 32;
		Particles.Create( "particles/impact.flesh.bloodpuff-big.vpcf", pos );
		Particles.Create( "particles/impact.flesh-big.vpcf", pos );
		PlaySound( "kersplat" );

		var head = SpawnHead();
		if(head != null)
		{
			head.PhysicsGroup.AddVelocity( ((Vector3.Up * 0.5f) + Vector3.Random) * 200 );
			Corpse = head;
		}	

		for ( int i = 0; i < 10; i++ )
		{
			var mdl = new string[]
			{
				"models/sbox_props/watermelon/watermelon_gib07.vmdl",
				"models/sbox_props/watermelon/watermelon_gib08.vmdl",
				"models/sbox_props/watermelon/watermelon_gib09.vmdl",
				"models/sbox_props/watermelon/watermelon_gib06.vmdl"
			}.GetRandom();

			var gib = new Prop();
			gib.SetModel( mdl );
			gib.RenderColor = new ColorHsv( 0, 0, 0.5f ).ToColor();
			gib.Position = Position + Vector3.Up * 32;
			gib.Velocity = ((Vector3.Up * 0.5f) + Vector3.Random) * 200;
			gib.Scale = 1 + (float)new Random().NextDouble() * 3f;
			gib.DeleteAsync( 10 );

			GibLimit.Watch( gib );

			if ( HasHead() ) Corpse = gib;
		}
	}

	void BecomeRagdoll( Vector3 force, int forceBone )
	{
		var headshot = forceBone == GetBoneIndex( "head" );

		var ent = new Prop();
		ent.Position = Position;
		ent.Rotation = Rotation;
		ent.MoveType = MoveType.Physics;
		ent.UsePhysicsCollision = true;
		//ent.SetInteractsAs( CollisionLayer.Debris );
		//ent.SetInteractsWith( CollisionLayer.WORLD_GEOMETRY );
		//ent.SetInteractsExclude( CollisionLayer.Player | CollisionLayer.Debris );

		ent.SetModel( GetModelName() );
		ent.SetMaterialGroup( GetMaterialGroup() );
		ent.CopyBonesFrom( this );
		ent.TakeDecalsFrom( this );
		ent.SetRagdollVelocityFrom( this );
		//ent.DeleteAsync( 20.0f );

		// Copy body groups
		{
			if ( BodyMask == 0 ) ent.RenderAlpha = 0;
			ent.SetBodyGroup( BodyMask.Head.ToGroup(), BodyMask.HasFlag( BodyMask.Head ) ? 0 : 1 );
			ent.SetBodyGroup( BodyMask.Chest.ToGroup(), BodyMask.HasFlag( BodyMask.Chest ) ? 0 : 1 );
			ent.SetBodyGroup( BodyMask.Legs.ToGroup(), BodyMask.HasFlag( BodyMask.Legs ) ? 0 : 1 );
			ent.SetBodyGroup( BodyMask.Hands.ToGroup(), BodyMask.HasFlag( BodyMask.Hands ) ? 0 : 1 );
			ent.SetBodyGroup( BodyMask.Feet.ToGroup(), BodyMask.HasFlag( BodyMask.Feet ) ? 0 : 1 );
		}

		if ( headshot && HasHead() )
		{
			// Disable head body group
			ent.SetBodyGroup(0, 1);

			var bone = new ModelEntity( "models/citizen/head_bone.vmdl" );
			bone.SetMaterialGroup(GetMaterialGroup());
			bone.SetParent(ent, true);
			
			var head = SpawnHead();
			head.PhysicsGroup.AddVelocity(force);

			Corpse = head;
		}

		// Copy the clothes over
		foreach ( var child in Children )
		{
			if ( child is ModelEntity e )
			{
				if ( !e.Tags.Has( "clothing" ) )
					continue;

				if ( e.Tags.Has( "hat" ) && forceBone == GetBoneIndex( "head" ) ) continue;

				var clothing = new ModelEntity( e.GetModelName() );
				clothing.SetMaterialGroup(e.GetMaterialGroup());
				clothing.SetParent( ent, true );
			}
		}
		
		ent.PhysicsGroup.AddVelocity( force );

		if ( forceBone >= 0 )
		{
			var body = ent.GetBonePhysicsBody( forceBone );
			if ( body != null )
			{
				body.ApplyForce( force * 1000 );
			}
			else
			{
				ent.PhysicsGroup.AddVelocity( force );
			}
		}


		Corpse = ent;

		//RagdollLimit.Watch( ent );
	}

	bool HasHead()
	{
		return BodyMask.HasFlag(BodyMask.Head);
	}

	Prop SpawnHead()
	{
		if ( HasHead() )
		{
			var head_ragdoll = new Prop();
			head_ragdoll.Position = Position;
			head_ragdoll.Rotation = Rotation;
			head_ragdoll.MoveType = MoveType.Physics;
			head_ragdoll.UsePhysicsCollision = true;
			//head_ragdoll.SetInteractsAs( CollisionLayer.Debris );
			//head_ragdoll.SetInteractsWith( CollisionLayer.WORLD_GEOMETRY );
			//head_ragdoll.SetInteractsExclude( CollisionLayer.Player | CollisionLayer.Debris );

			head_ragdoll.SetModel( "models/citizen/head_ragdoll.vmdl" );
			head_ragdoll.SetMaterialGroup( GetMaterialGroup() );
			//head_ragdoll.CopyBonesFrom( this );
			head_ragdoll.TakeDecalsFrom( this );
			//head_ragdoll.DeleteAsync( 20.0f );

			var hats = Children.Where( c => c.Tags.Has( "hat" ) );
			foreach ( var c in hats )
			{
				if ( c is not ModelEntity h ) continue;

				h = new ModelEntity( h.GetModelName() );
				h.SetParent( head_ragdoll, true );
			}
			GibLimit.Watch( head_ragdoll );
			return head_ragdoll;
		}

		return null;
	}
	void SpawnHats()
	{
		var hats = Children.Where( c => c.Tags.Has( "hat" ) );
		foreach ( var c in hats )
		{
			if ( c is not ModelEntity h ) continue;

			var hat = new Prop();
			hat.SetModel( h.GetModelName() );
		}
	}
}
