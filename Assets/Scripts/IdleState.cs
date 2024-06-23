using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class IdleState : StateMachineBehaviour
{
    private Units unit;
    private const string IsChase = "IsChase";
    private const string IsAttack = "IsAttack";
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public async override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        unit = animator.GetComponent<Units>();
        animator.SetBool(IsChase, false);
        animator.SetBool(IsAttack, false);
        unit.agent.speed = 0;
        
        if (unit.target != null)
        {
            await Task.Delay(1000);
            animator.SetBool(IsChase, true);
        }
       
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Implement code that processes and affects root motion
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Implement code that sets up animation IK (inverse kinematics)
    }
}
