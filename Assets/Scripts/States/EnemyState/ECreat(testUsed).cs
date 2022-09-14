using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECreat : StateMachineBehaviour
{
    public Vector3 pos;
    public Vector3 rot = Vector3.zero;
    public Vector3 scale = Vector3.one;
    [SerializeField] Rigidbody2D creatObj;
    public float horizontalForce = 5.0f;
    public float verticalForce = 4.0f;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Transform transform = animator.transform;
        int direct = (int)(Mathf.Abs(transform.lossyScale.x) / transform.lossyScale.x);
        Vector3 childPos = transform.position + new Vector3(pos.x * direct, pos.y, pos.z);//计算子物体的坐标
        Vector3 childScale = new Vector3(scale.x * direct, scale.y,scale.z);//计算子物体缩放
        var projectile = Instantiate(creatObj, childPos, Quaternion.identity);//实例化子物体
        projectile.transform.localScale = childScale;//修改子物体缩放
        var force = new Vector2(horizontalForce * direct, verticalForce);
        projectile.AddForce(force);//.SetForce(force);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
