using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;

namespace MoreMountains.CorgiEngine
{
	/// <summary>
	/// Add this ability to a character and it'll bounce up every time it hits the ground
	/// </summary>
	public class CharacterBounce : CharacterAbility
	{
		/// the vertical  force to apply at every bounce
		[Tooltip("the vertical force to apply at every bounce")]
		public float BounceForce = 1f;
		/// whether or not this ability should reset the CharacterJump's flags on every bounce 
		[Tooltip("whether or not this ability should reset the CharacterJump's flags on every bounce")]
		public bool SetJumpFlags = true;

		protected CharacterJump _characterJump;
		protected bool _collidingAboveLastFrame = false;
		protected bool _collidingBelowLastFrame = false;
		protected bool _collidingRightLastFrame = false;
		protected bool _collidingLeftLastFrame = false;

		/// <summary>
		/// On initialization we store our CharacterJump ability
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			_characterJump = _controller.GetComponent<CharacterJump>();
		}

		/// <summary>
		/// On late process, we apply our force 
		/// </summary>
		public override void LateProcessAbility()
		{
			if (_controller.State.IsCollidingAbove && !_collidingAboveLastFrame)
			{
				_controller.SetVerticalForce(-BounceForce);
				_characterJump.CanJumpStop = false;
				if (SetJumpFlags)
				{
					_characterJump.SetJumpFlags();
				}
				MMCharacterEvent.Trigger(_character, MMCharacterEventTypes.Bounce);
			}

			if (_controller.State.IsCollidingBelow && !_collidingBelowLastFrame)
			{
				_controller.SetVerticalForce(BounceForce); //ごり押しで反射させてるっぽい
				_characterJump.CanJumpStop = false;
				if (SetJumpFlags)
				{
					_characterJump.SetJumpFlags();
				}
				MMCharacterEvent.Trigger(_character, MMCharacterEventTypes.Bounce);
			}

			if (_controller.State.IsCollidingRight && !_collidingRightLastFrame)
			{
				_controller.SetHorizontalForce(-BounceForce);
				_characterJump.CanJumpStop = false;
				if (SetJumpFlags)
				{
					_characterJump.SetJumpFlags();
				}
				MMCharacterEvent.Trigger(_character, MMCharacterEventTypes.Bounce);
			}

			if (_controller.State.IsCollidingLeft && !_collidingLeftLastFrame)
			{
				_controller.SetHorizontalForce(BounceForce);
				_characterJump.CanJumpStop = false;
				if (SetJumpFlags)
				{
					_characterJump.SetJumpFlags();
				}
				MMCharacterEvent.Trigger(_character, MMCharacterEventTypes.Bounce);
			}
			_collidingBelowLastFrame = _controller.State.IsCollidingBelow;
		}
	}
}