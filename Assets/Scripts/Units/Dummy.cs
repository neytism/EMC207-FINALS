using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dummy : Units
{
    public override void Attack()
    {
        throw new System.NotImplementedException();
    }

    public override void Hurt(float damageTaken)
    {
        if (!isAlive) return;
        
        healthCanvas.SetActive(true);
        hitPoints -= damageTaken;
        healthCanvas.transform.GetChild(1).GetComponent<Image>().fillAmount = hitPoints / maxHitPoints;

        if (hitPoints <= 0)
        {
           Death();
        }
        else
        {
            animator.SetTrigger("Hit");
        }
    }

    public override void Death()
    {
        isAlive = false;
        InvokeDeathEvent();
        animator.SetTrigger("Death");
        healthCanvas.SetActive(false);
        GetComponent<Collider>().enabled = false;
    }
}
