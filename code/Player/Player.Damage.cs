using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

	public partial class SandboxPlayer
	{
	DamageInfo LastDamage;

	[ClientRpc]
	public void TookDamage( DamageFlags damageFlags, Vector3 forcePos, Vector3 force )
	{
	}

	public override void TakeDamage( DamageInfo info )
	{
		LastDamage = info;

		if ( info.Attacker != null )
		{
			//if ( !(info.Weapon as Weapon)?.IsMelee ?? false )
			{
				// Check for headshot
				if ( info.HitboxIndex == 5 )
				{
					info.Damage *= 2.0f;
				}
			}
			/*
			else
			{
				// Check for backstab
				var facing = Rotation.Forward;
				var attackedfrom = info.Attacker.Rotation.Forward;
				var dot = facing.Dot( attackedfrom );
				if ( dot > 0 ) info.Damage *= 2;
			}
			*/
		}

		base.TakeDamage( info );

		/*
		if ( info.Attacker is Player attacker && attacker != this )
		{
			// Note - sending this only to the attacker!
			attacker.DidDamage( To.Single( attacker ), info.Position, info.Damage, Health.LerpInverse( 100, 0 ) );

			TookDamage( To.Single( this ), info.Weapon.IsValid() ? info.Weapon.Position : info.Attacker.Position );
		}
		*/
	}
}
