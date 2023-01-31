using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent2 : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.ResetTrigger("TakeOff");
    }
}
