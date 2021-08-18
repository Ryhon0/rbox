using Sandbox;

partial class SandboxPlayer
{
	void WearHeadset()
	{
		if ( Input.VR.IsActive )
		{
			// TODO: Give each headset an unique model
			Wear( "models/clothes/headset/headset.vmdl" ).Tags.Add( "hat" );
		}
	}

	[Event.Tick]
	void VRTick()
	{
		if ( !Input.VR.IsActive ) return;
		Camera = new ThirdPersonCamera();

		SetAnimBool( "b_vr", true );

		Transform LocalLeftHand = Transform.ToLocal( Input.VR.LeftHand.Transform );
		Transform LocalRightHand = Transform.ToLocal( Input.VR.RightHand.Transform );
		Transform LocalHead = Transform.ToLocal( Input.VR.Head );

		var positionOffset = new Vector3( -7, 1, 2.5f );
		var leftpos = LocalLeftHand.Position + (positionOffset * LocalLeftHand.Rotation);
		var rightpos = LocalRightHand.Position + (positionOffset * new Vector3( 1, -1, 1 ) * LocalRightHand.Rotation);

		SetAnimVector( "left_hand_ik.position", leftpos );
		SetAnimVector( "right_hand_ik.position", rightpos );

		Angles rotationOffset = new Angles( 50, 0, 90 );

		if ( Input.VR.LeftHand.Grip.Value > 0.75 )
		{
			SetAnimRotation( "right_hand_ik.rotation",
				LocalRightHand.Position.LookAt( LocalLeftHand.Position )
				.RotateAroundAxis( Vector3.Forward, 90 ) );

			SetAnimRotation( "left_hand_ik.rotation",
				LocalRightHand.Position.LookAt( LocalLeftHand.Position )
				.RotateAroundAxis( Vector3.Forward, 90 ) );
		}
		else
		{
			SetAnimRotation( "left_hand_ik.rotation",
				LocalLeftHand.Rotation * rotationOffset.ToRotation() );
			SetAnimRotation( "right_hand_ik.rotation",
				LocalRightHand.Rotation * rotationOffset.ToRotation() );
		}

		SetAnimFloat( "duck", (1 - (LocalHead.Position.z / 65f) * 3f) );

		// Hide head
		if ( IsClient )
			using ( Prediction.Off() )
				SetBone( GetBoneIndex( "head" ), GetBoneTransform( GetBoneIndex( "head" ) ).WithScale( 0 ) );
	}
}
