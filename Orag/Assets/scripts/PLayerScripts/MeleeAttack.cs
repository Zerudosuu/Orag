using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    private Animator animator;

    public GameObject attackPoint_near;
    public GameObject attackPoint_far;
    public float attackRange = 0.5f;
    public LayerMask EnemyLayers;


    public static float attackrate =1;
    public float nextAttackTime=0;

    public static bool AttackNow;
   
    public float meleeDamage=35;
    public float freezeTime = 1;

    public GameObject AccessPlayerScripts;
    public float range;

    public GameObject sparkPrefab;


    void Start()
    {
        animator = GetComponent<Animator>();      
    }


   

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0)){

            AttackNow = true;
            freezePlayer(AttackNow, freezeTime);
        }

        if (Time.time >= nextAttackTime)
        {         
            if ( AttackNow) // ||
            {
                attack();
            }
        }
    }

    void attack()
    {
        animator.SetTrigger("Attack");
       // Debug.Log("attack");
     
        Collider2D[] hitEnemies_near = Physics2D.OverlapCircleAll(attackPoint_near.transform.position, attackRange, EnemyLayers);
        Collider2D[] hitEnemies_far = Physics2D.OverlapCircleAll(attackPoint_far.transform.position , attackRange * 0.7f, EnemyLayers);

        

        

         foreach (Collider2D enemy in hitEnemies_near) // hit near enemy
            {
            /* //   Debug.Log("I hit you in  NEAR RANGE bich  " + enemy.name);

                if(enemy.name == "EagleBody")
                enemy.GetComponentInParent<EagleHp>().TakeDamage(meleeDamage );

                if(enemy.name == "Snail")           
                enemy.GetComponent<snail>().TakeDamage(meleeDamage);
*/
               Instantiate(sparkPrefab, enemy.transform.position, Quaternion.identity);
         }

            foreach (Collider2D enemy in hitEnemies_far) // kpaag harayo 50% less damage 
            {
             /*   Debug.Log("I hit you IN FAR RANGE bich  " + enemy.name);

                if (enemy.name == "EagleBody")
                    enemy.GetComponentInParent<EagleHp>().TakeDamage(meleeDamage * 0.5f);

                if (enemy.name == "Snail")
                    enemy.GetComponent<snail>().TakeDamage(meleeDamage * 0.5f);
*/
                Instantiate(sparkPrefab, enemy.transform.position, Quaternion.identity);
            }






        nextAttackTime = Time.time + 1 / attackrate;
        AttackNow = false;
              
    }





    
     void freezePlayer(bool attack, float FreezeTime) 
      {
          jumpshit player = AccessPlayerScripts.GetComponent<jumpshit>();
          player.FreezePlayer(attack,freezeTime);

      }



    void OnDrawGizmosSelected()
    {

        //  if (attackPoint == null)
        //    return;
         Gizmos.color = Color.white;
          Gizmos.DrawWireSphere(attackPoint_near.transform.position, attackRange);
          Gizmos.DrawWireSphere(attackPoint_far.transform.position, attackRange * 0.7f);
       // Gizmos.DrawRay(attackPoint_near.transform.position, transform.TransformDirection(Vector2.left) * 3);



    }

    public void upgradeStats(float newAttackrate)
    {
      
        if (attackrate < 5)
        {
            attackrate += newAttackrate;
        }
        else
        {
            attackrate = 5;
        }
    }

}

/*
  public void AttackButtonPressed()
    {
   
        AttackNow = true;      
        freezePlayer(AttackNow,freezeTime);
      
    }
 */