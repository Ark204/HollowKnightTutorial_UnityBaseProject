using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Combat;
using Core.Character;
using Core.Combat.Projectiles;

public class EAttack : StateMachineBehaviour
{
    [System.Serializable]
    public class Weapon
    {
        public Vector3 pos;
        public Vector3 rot = Vector3.zero;
        public Vector3 scale = Vector3.one;
        //public AbstractProjectile projectilePrefab;
        public Rigidbody2D rigidbody2D;
        public float horizontalForce = 5.0f;
        public float verticalForce = 4.0f;
    }
    [SerializeField] Weapon[] weapons;
    [SerializeField] bool shakeCamera;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Weapon weapon = weapons[Random.Range(0, weapons.Length)];//随机选择一种武器
        Transform transform = animator.transform;
        int direct = (int)(Mathf.Abs(transform.lossyScale.x) / transform.lossyScale.x);
        Vector3 childPos = transform.position + new Vector3(weapon.pos.x * direct, weapon.pos.y, weapon.pos.z);//计算子物体的坐标
        Vector3 childScale = new Vector3(weapon.scale.x * direct, weapon.scale.y, weapon.scale.z);//计算子物体缩放
        var projectile = Instantiate(weapon.rigidbody2D/*projectilePrefab*/,childPos , Quaternion.identity);//实例化子物体
        projectile.transform.localScale = childScale;//修改子物体缩放
        //projectile.Shooter = animator.gameObject;

        var force = new Vector2(weapon.horizontalForce * direct, weapon.verticalForce);
        projectile.AddForce(force);//.SetForce(force);
        //画面震动
        if (shakeCamera)
            CameraController.Instance.ShakeCamera(0.5f);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
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
