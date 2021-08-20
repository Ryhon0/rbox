using Sandbox;
using System.Linq;

[Library( "grapplinggun", Title = "Grappling Gun", Spawnable = true, Group = "Weapons" )]
public class GrapplingGun : Weapon
{
	public override float Damage => 10;
	public override bool IsAutomatic => false;
	public override int RPM => 600;
	public override float ReloadTime => 1f;
	public override float Spread => 0;
	public override string ShootShound => "rust_pistol.shoot";
	public override string ViewModelPath => "weapons/grapplegun/v_grapplegun.vmdl";
	public override string WorldModelPath => "weapons/grapplegun/grapplegun.vmdl";
	public override CrosshairType CrosshairType => CrosshairType.Dot;
	public override int ClipSize => 1;
	public override string Projectile => "grapplinghook";

	public override void AttackPrimary()
	{
		if ( !IsServer ) return;

		foreach ( var pr in Entity.All.OfType<GrapplingHook>()
			.Where( p => p.Owner == Owner ) )
			pr.Delete();

		var proj = Library.Create<Projectile>( Projectile );
		proj.Owner = Owner;
		proj.Weapon = this;
		proj.Damage = Damage;
		proj.Rotation = ShootFromAngle;
		proj.Position = ShootFrom;
	}

	public override void OnCarryDrop( Entity dropper )
	{
		base.OnCarryDrop( dropper );
		foreach ( var pr in Entity.All.OfType<GrapplingHook>()
			.Where( p => p.AttachedEntity == dropper ) )
		{
			pr.AttachedEntity = this;
			pr.Rope.SetEntityBone( 0, this, 0 );
		}
	}
}

[Library( "grapplinghook" )]
public class GrapplingHook : Projectile
{
	public override string ModelPath => "weapons/grapplegun/hook.vmdl";
	bool Stuck;
	float MaxDistance = 2500;

	public Entity AttachedEntity;
	public Entity HitEntity;
	public Particles Rope;

	public override void Spawn()
	{
		SetModel( ModelPath );
		MoveType = MoveType.None;
	}

	[Event.Physics.PostStep]
	void PhysicsPostStep()
	{
		if ( AttachedEntity is Player p && p.LifeState == LifeState.Dead )
		{
			AttachedEntity = p.Corpse;
			Rope.SetEntityBone( 0, AttachedEntity, 0 );
		}

		if ( IsServer )
			if ( Rope == null )
			{
				AttachedEntity = Owner;

				Rope = Particles.Create( "particles/rope.vpcf" );

				Rope.SetEntityBone( 0, Owner, 0 );
				Rope.SetEntityBone( 1, this, 0 );
			}

		if ( Stuck ) Pull();
		else MoveForward();
	}

	void Pull()
	{
		if ( !AttachedEntity.IsValid() || !HitEntity.IsValid() )
		{
			Delete();
			return;
		}

		var speed = 50;

		// Pull prop/player towards player
		if ( (HitEntity is Prop prp &&
			prp.PhysicsGroup?.GetBody( 0 )?.BodyType != PhysicsBodyType.Static)
			|| HitEntity is Player )
		{
			if ( HitEntity is Player pl )
				if ( pl.LifeState == LifeState.Dead )
				{
					HitEntity = pl.Corpse;
					Rope.SetEntityBone( 1, HitEntity, 0 );
				}

			if ( HitEntity is Prop )
				HitEntity.PhysicsGroup.AddVelocity( -(HitEntity.Position - AttachedEntity.Position).Normal * speed );
			else HitEntity.Velocity -= (HitEntity.Position - AttachedEntity.Position).Normal * speed;
		}
		// Pull player towards hook
		else
		{
			if ( AttachedEntity is Prop pr )
			{
				pr.PhysicsGroup.AddVelocity( -(AttachedEntity.PhysicsGroup.Pos - Position).Normal * speed );
			}
			else
			{
				AttachedEntity.Velocity += AttachedEntity.EyeRot.Forward * speed / 2;
				AttachedEntity.Velocity -= (AttachedEntity.Position - Position).Normal * speed;
			}
		}

		if ( IsServer )
		{
			float dist = 300;
			if ( HitEntity is Prop ) dist = AttachedEntity.Position.Distance( HitEntity.Position );
			else dist = AttachedEntity.Position.Distance( Position );

			if ( dist < 200 ) Delete();
		}
	}

	protected override void OnDestroy()
	{
		if ( !IsServer ) return;

		Rope.Destroy( true );
	}

	void MoveForward()
	{
		if ( !AttachedEntity.IsValid() ) return;
		if ( AttachedEntity.Position.Distance( Position ) > MaxDistance )
		{
			Delete();
			return;
		}

		var ray = Trace.Ray( Position, Position + Rotation.Forward * 40 )
			.HitLayer( CollisionLayer.Water, false )
			.UseHitboxes()
			.Size( 1 )
			.Ignore( Owner )
			.Ignore( Weapon );

		var tr = ray.Run();

		DebugOverlay.Line( tr.StartPos, tr.EndPos, tr.Hit ? Color.Red : Color.Green, 1 );

		if ( tr.Hit )
		{
			Stuck = true;

			HitEntity = tr.Entity;

			if ( HitEntity.IsValid() )
			{
				var dmg = DamageInfo.FromBullet( tr.EndPos, tr.Direction * 200, Damage )
					.UsingTraceResult( tr )
					.WithAttacker( Owner )
					.WithWeapon( Weapon );
				HitEntity.TakeDamage( dmg );

				if ( !HitEntity.IsValid() ) return;
			}

			if ( HitEntity.PhysicsGroup?.GetBody( 0 )?.BodyType == PhysicsBodyType.Static )
				if ( AttachedEntity.GroundEntity != null )
				{
					AttachedEntity.Position += Vector3.Up * 20;
					AttachedEntity.Velocity = AttachedEntity.Velocity.WithZ( 100 );
				}

			if ( HitEntity.GroundEntity != null )
			{
				HitEntity.Position += Vector3.Up * 20;
				HitEntity.Velocity = HitEntity.Velocity.WithZ( 100 );
			}

			Position = tr.EndPos;
			SetParent( HitEntity, tr.Bone );

			tr.Normal = Rotation.Forward * -1;
			tr.Surface.DoBulletImpact( tr );
		}
		else Position = tr.EndPos;
	}
}
