using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackState : StateMachineBehaviour
{
    [SerializeField] private float rotationSpeed = 5f;
    
    private Units unit;

    private const string IsChase = "IsChase";
    private const string IsAttack = "IsAttack";

    private float _attackTime;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        unit = animator.GetComponent<Units>();
    }
    
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!unit.isAlive) return;
        
        if (unit.target == null)
        {
            animator.SetBool(IsChase, false);
            animator.SetBool(IsAttack, false);
            return;
        }
        
        var target = unit.target.transform.position;
        var position = unit.transform.position;
        
        float distanceToPlayer = Vector3.Distance(position, target);
        
        unit.transform.forward = Vector3.Lerp(unit.transform.forward, (target - position).normalized, Time.deltaTime * rotationSpeed);

        if (unit.type == Units.UnitType.Archer || unit.type == Units.UnitType.Healer)
        {
            _attackTime += Time.deltaTime;
            if (_attackTime >= (unit.attackRate + Random.Range(0f, 2f)))
            {
                animator.SetTrigger("Shoot");
                //unit.GetClosestEnemy();
                _attackTime = 0;
            }
        }

        if (distanceToPlayer > unit.attackRange)
        {
            animator.SetBool(IsChase, true);
            animator.SetBool(IsAttack, false);
        }
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
