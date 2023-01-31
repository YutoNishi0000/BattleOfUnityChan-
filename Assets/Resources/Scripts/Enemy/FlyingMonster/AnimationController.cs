using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Land");
        animator.ResetTrigger("TakeOff");
    }
}