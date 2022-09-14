using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMove : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    Vector3 targetPoint;
    EnemyCtrl enemyCtrl;
    Transform m_transform;
    [SerializeField] float stopDistance = 0.1f;
    public float speed;
    public string nextState;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyCtrl == null) enemyCtrl = animator.GetComponent<EnemyCtrl>();
        m_transform = enemyCtrl.m_transform;
        //构造巡逻目标
        targetPoint = enemyCtrl.GetRandomTarget();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (targetPoint.x - m_transform.position.x > 0) m_transform.localScale = new Vector3(-1, m_transform.localScale.y, m_transform.localScale.z);
        else if (targetPoint.x - m_transform.position.x < 0) m_transform.localScale = new Vector3(1, m_transform.localScale.y, m_transform.localScale.z);
        if (Vector2.Distance(targetPoint, m_transform.position) > stopDistance)
        {
            m_transform.position = Vector2.MoveTowards(m_transform.position,
                 targetPoint, speed * Time.fixedDeltaTime);
        }
        else if (nextState != null) animator.Play(nextState);//animator.SetBool("isMoving", false);
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
