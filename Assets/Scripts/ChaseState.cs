using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : StateMachineBehaviour
{
    private Units unit;

    private const string IsChase = "IsChase";
    private const string IsAttack = "IsAttack";
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        unit = animator.GetComponent<Units>();
        if(unit.isAlive) unit.agent.speed = unit.speed;
    }
    
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!unit.isAlive) return;
        
        if (unit.target == null)
        {
            animator.SetBool(IsChase, false);
            animator.SetBool(IsAttack, false);
            return;
        }
        
        var target = unit.target.transform.position;
        unit.agent.SetDestination(target);

        float distanceToPlayer = Vector3.Distance(unit.transform.position, target);
        
        if (distanceToPlayer <= unit.attackRange)
        {
            unit.agent.speed = 0;
            animator.SetBool(IsChase, false);
            animator.SetBool(IsAttack, true);
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