using UnityEngine;

// public class AbilityJump : MonoBehaviour, ISubmoduleMount<Ability>
// {
//     [SerializeField] private float jumpHeight = 2f;
//
//     private Actor _actor;
//     private Movement _movement;
//
//     public void Mount(Ability submodule)
//     {
//         _actor = submodule.Owner.GetComponent<Actor>();
//         _movement = submodule.Owner.GetComponent<Movement>();
//         _actor.OnMove += HandleMove;
//     }
//
//     private void HandleMove()
//     {
//         var jumpIntent = _actor.Pawn.GetBool(IntentKey.Jump);
//         if (!jumpIntent)
//         {
//             return;
//         }
//
//         if (!_movement.IsOnGround)
//         {
//             return;
//         }
//
//         var velocityX = _movement.Velocity.x;
//         var velocityY = KinematicUtil.JumpVelocityY(jumpHeight);
//         _movement.SetVelocity(new Vector2(velocityX, velocityY));
//     }
// }