using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

partial class Weapon
{
	public static bool UseClientSideHitreg = false;
	public static SoundEvent Dryfire = new SoundEvent( "weapons/rust_shotgun/sounds/rust-shotgun-dryfire.vsnd" );

	public bool IsUsingVR => Input.VR.IsActive;

	public Vector3 ShootFrom => IsUsingVR ?
		GetAttachment( "muzzle" ).Value.Position :
		Owner.EyePos;

	public Rotation ShootFromAngle => IsUsingVR ?
		GetAttachment( "muzzle" ).Value.Rotation :
		Owner.EyeRot;

	public bool WantsToShoot
	{
		get
		{
			if ( Owner is Player )
			{
				if ( IsAutomatic )
				{
					return IsUsingVR ?
						Input.VR.RightHand.Trigger.Value == 1 :
						Input.Down( InputButton.Attack1 );
				}
				else
				{
					return IsUsingVR ?
						Input.VR.RightHand.Trigger.Value == 1 && Input.VR.RightHand.Trigger.Delta != 0 :
						Input.Pressed( InputButton.Attack1 );
				}
			}
			// TODO: shooting for bots
			else
			{
				return false;
			}
		}
	}

	public bool WantsToShootSecondary
	{
		get
		{
			if ( Owner is Player )
			{
				return IsUsingVR ?
					Input.VR.RightHand.Grip.Value == 1 :
					Input.Down( InputButton.Attack2 );
			}
			// TODO: shooting for bots
			else
			{
				return false;
			}
		}
	}

	public override bool CanPrimaryAttack()
	{
		if ( Owner == null || Owner.Health <= 0 ) return false;

		if ( BurstShotsRemaining > 0 && TimeSincePrimaryAttack > BurstInterval ) return true;

		if ( !IsMelee )
			if ( ReloadMagazine )
				if ( IsReloading ) return false;

		if ( AmmoClip <= 0 ) return IsUsingVR ?
						Input.VR.RightHand.Trigger.Value == 1 && Input.VR.RightHand.Trigger.Delta != 0 :
						Input.Pressed( InputButton.Attack1 );

		var chambered = TimeSincePrimaryAttack > AttackInterval;
		var shooting = WantsToShoot;

		return chambered && shooting;
	}

	public override bool CanSecondaryAttack()
	{
		if ( Owner == null || Owner.Health <= 0 ) return false;

		return base.CanSecondaryAttack();
	}

	public int BurstShotsRemaining = 0;
	public int ShotsThisBurst => Math.Min( AmmoClip, ShotsPerTriggerPull );

	public override void AttackPrimary()
	{
		if ( AmmoClip <= 0 )
		{
			DryFire();
			BurstShotsRemaining = 0;
			return;
		}

		if ( BurstShotsRemaining > 0 )
		{
			BurstShotsRemaining--;
		}
		else BurstShotsRemaining = ShotsThisBurst - 1;

		if ( IsMelee )
		{
			if ( IsClient ) ShootEffects();
			PlaySound( ShootShound );
			ShootBullet( 0, Force, Damage, 10f, 1 );
		}
		else
		{
			if ( TakeAmmo( 1 ) )
			{
				IsReloading = false;

				ShootEffects();
				PlaySound( ShootShound );

				if ( Projectile != null ) ShootProjectile( Projectile, Spread, ProjectileSpeed, Force, Damage, BulletsPerShot );
				else ShootBullet( Spread, Force, Damage, BulletSize, BulletsPerShot, DamageFlags );

				(Owner as AnimEntity).SetAnimBool( "b_attack", true );
			}
		}
	}

	public void ShootProjectile( string projectile, float spread, float projectilespeed, float force, float damage, int count = 1, DamageFlags flags = DamageFlags.Bullet )
	{
		if ( !IsServer ) return;

		for ( int i = 0; i < BulletsPerShot; i++ )
		{
			if ( Owner == null ) continue;

			var forward = ShootFromAngle.Forward;
			forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
			forward = forward.Normal;

			var p = Create( projectile );
			p.Owner = Owner;

			p.Position = GetProjectilePosition();
			p.Rotation = ShootFromAngle;

			var vel = forward * projectilespeed;
			p.Velocity = vel;

			if ( p is Projectile pp )
			{
				pp.Damage = damage;
				pp.Force = force;
				pp.Weapon = this;
				pp.DamageFlags = flags;
			}
		}
	}

	Vector3 GetProjectilePosition( float MaxDistance = 30 )
	{
		var start = ShootFrom;
		var end = start + ShootFromAngle.Forward * MaxDistance;
		var t = Trace.Ray( start, end )
					.UseHitboxes()
					.HitLayer( CollisionLayer.Water, false )
					.Size( 1.0f );

		if ( !IsUsingVR )
		{
			t.Ignore( this );
			t.Ignore( Owner );
		}
		var tr = t.Run();
		TraceBullet( start, end );
		return tr.Hit ? tr.EndPos : end;
	}

	public void ShootBullet( float spread, float force, float damage, float bulletSize, int count = 1, DamageFlags flags = DamageFlags.Bullet )
	{
		if ( UseClientSideHitreg && IsServer ) return;

		//
		// ShootBullet is coded in a way where we can have bullets pass through shit
		// or bounce off shit, in which case it'll return multiple results
		//
		for ( int i = 0; i < BulletsPerShot; i++ )
		{
			if ( Owner == null ) continue;
			var forward = ShootFromAngle.Forward;
			forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
			forward = forward.Normal;

			foreach ( var tr in TraceBullet( ShootFrom, ShootFrom + forward * Range, bulletSize ) )
			{
				if ( tr.Hit ) tr.Surface.DoBulletImpact( tr );

				if ( !tr.Entity.IsValid() ) continue;

				//
				// We turn predictiuon off for this, so any exploding effects don't get culled etc
				//
				using ( Prediction.Off() )
				{
					var damageInfo = DamageInfo.FromBullet( tr.EndPos, forward * 100 * force, damage / count )
						.UsingTraceResult( tr )
						.WithAttacker( Owner )
						.WithWeapon( this )
						.WithFlag( flags );

					if ( !UseClientSideHitreg )
					{
						tr.Entity.TakeDamage( damageInfo );
					}
					else
					{
						ServerTakeDamage( tr.Entity.NetworkIdent, tr.EndPos,
							damageInfo.Damage, damageInfo.Force, damageInfo.HitboxIndex, damageInfo.Flags );
					}
				}
			}
		}
	}

	public override IEnumerable<TraceResult> TraceBullet( Vector3 start, Vector3 end, float radius = 2 )
	{

		bool InWater = Physics.TestPointContents( start, CollisionLayer.Water );

		var t = Trace.Ray( start, end )
				.UseHitboxes()
				.HitLayer( CollisionLayer.Water, !InWater )
				.Size( radius );

		if ( !IsUsingVR )
		{
			t.Ignore( this );
			t.Ignore( Owner );
		}
		var tr = t.Run();

		yield return tr;
	}

	[ServerCmd]
	public static void ServerTakeDamage( int TargetID, Vector3 pos, float damage, Vector3 force, int hitbox, DamageFlags flags )
	{
		Host.AssertServer();

		var attacker = ConsoleSystem.Caller.Pawn as Player;
		var attacked = Entity.All.FirstOrDefault( e => e.NetworkIdent == TargetID );
		if ( attacked == null ) return;

		var dmg = DamageInfo.FromBullet( pos, Vector3.Zero, damage )
			.WithAttacker( attacker )
			.WithWeapon( attacker.ActiveChild )
			.WithFlag( flags )
			.WithHitbox( hitbox )
			.WithForce( force );

		attacked.TakeDamage( dmg );
	}

	[ClientRpc]
	protected virtual void ShootEffects()
	{
		Host.AssertClient();

		if ( MuzzleFlash != null )
			Particles.Create( MuzzleFlash, EffectEntity, "muzzle" );
		if ( Brass != null )
			Particles.Create( Brass, EffectEntity, "ejection_point" );

		if ( Owner == Local.Pawn )
		{
			new Sandbox.ScreenShake.Perlin();
		}

		ViewModelEntity?.SetAnimBool( "fire", true );
		if ( CrosshairPanel is Crosshair c ) c.fireCounter += 2;
	}
	public bool TakeAmmo( int amount )
	{
		if ( AmmoClip < amount )
			return false;

		AmmoClip -= amount;
		return true;
	}
	public virtual void DryFire()
	{
		PlaySound( "Weapon.Dryfire" );

		if ( !IsReloading ) Reload();
	}
}
