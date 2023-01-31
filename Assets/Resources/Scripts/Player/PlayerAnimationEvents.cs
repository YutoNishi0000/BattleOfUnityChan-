using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//�v���C���[�̃A�j���[�V�����C�x���g���Ǘ�����N���X
public class PlayerAnimationEvents : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("Attack");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Attack3"))
        {
            animator.ResetTrigger("Attack");
        }
    }
}
