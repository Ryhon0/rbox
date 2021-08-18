
namespace Sandbox
{
	public class CarAnimator : PawnAnimator
	{
		public override void Simulate()
		{
			ResetParams();

			SetParam( "b_grounded", true );
			SetParam( "b_sit", true );

			var eyeAngles = (Pawn.Rotation.Inverse * Pawn.EyeRot).Angles();
			eyeAngles.pitch = eyeAngles.pitch.Clamp( -25, 70 );
			eyeAngles.yaw = eyeAngles.yaw.Clamp( -90, 90 );

			var aimPos = Pawn.EyePos + (Pawn.Rotation * Rotation.From( eyeAngles )).Forward * 200;

			SetLookAt( "aim_eyes", aimPos );
			SetLookAt( "aim_head", aimPos );
			SetLookAt( "aim_body", aimPos );

			if ( Pawn.ActiveChild is BaseCarriable carry )
			{
				carry.SimulateAnimator( this );
			}
			else
			{
				SetParam( "holdtype", 4 );
				SetParam( "holdtype_handedness", 0 );
				SetParam( "holdtype_pose_hand", 0.07f );
				SetParam( "aim_body_weight", 0.5f );
			}
		}
	}
}
