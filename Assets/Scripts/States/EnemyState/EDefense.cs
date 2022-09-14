using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EDefense : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    [SerializeField] Object shieldEffect;//防御特效
    GameObject prefab;
    BeAttackedable beAttackedable;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       prefab = (GameObject)Instantiate(shieldEffect, animator.transform);//加载预制体//设为子物体
       beAttackedable= animator.GetComponentInParent<BeAttackedable>();
        beAttackedable.RemoveAttackType(BeAttackedable.AttackType.Physics);//取消接受物理层攻击
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(prefab);
        beAttackedable.AddAttackType(BeAttackedable.AttackType.Physics);//还原接受物理层攻击
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
