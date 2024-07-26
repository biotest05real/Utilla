using GorillaLocomotion.Climbing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Utilla
{
	public class PageButton : MonoBehaviour
	{
		public Action onPressed;

		public void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.layer == (int)UnityLayer.GorillaHand && other.gameObject.TryGetComponent(out GorillaTriggerColliderHandIndicator hand))
			{
				GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(212, hand.isLeftHand, 0.12f);
                GorillaTagger.Instance.StartVibration(hand.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
                onPressed();
			}
		}
	}
}
