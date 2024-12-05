using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class boss : MonoBehaviour
{
    public float speed;
    public float range;
    public CharacterController controller;
    public Transform player;
    private float initialYPosition;
    private Animator animator;
    private NavMeshAgent agent;

    public float divebombCooldown = 5f;  // Cooldown for Divebomb
    public float summonMinionsCooldown = 10f;  // Cooldown for Summon Minions
    public float reflectiveAuraCooldown = 15f;  // Cooldown for Reflective Aura
    public float bloodSpikesCooldown = 8f;  // Cooldown for Blood Spikes

    private float divebombTimer = 0f;
    private float summonMinionsTimer = 0f;
    private float reflectiveAuraTimer = 0f;
    private float bloodSpikesTimer = 0f;

    public int phase = 1;
    public int health = 50;
    public int shield = 0;
    private bool isShieldActive = false;

    public LayerMask playerLayer;
    public int divebombDamage = 20;
    public float attackRange = 10f;

    public GameObject bombEffectPrefab;

    // Start is called before the first frame update
    void Start()
    {
        initialYPosition = transform.position.y;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (divebombTimer > 0) divebombTimer -= Time.deltaTime;
        if (summonMinionsTimer > 0) summonMinionsTimer -= Time.deltaTime;
        if (reflectiveAuraTimer > 0) reflectiveAuraTimer -= Time.deltaTime;
        if (bloodSpikesTimer > 0) bloodSpikesTimer -= Time.deltaTime;

        if (health <= 0 && phase == 1)
        {
            Debug.Log("hahahahhaahh");
            Die();
            animator.SetInteger("rescue", 1);
            
        }

        if (health <= 0 && phase == 1)
        {
            PhaseTwo();
        }

        if (phase == 2 && shield <= 0)
        {
            isShieldActive = false;
        }

        if (!inRage())
        {
            chase();
        }
        else
        {
            //AttackPlayer();
            if (Input.GetKeyDown(KeyCode.Z))
            {
                animator.Play("Standing 2H Cast Spell 01");
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                //animator.Play("Jump Attack");
                Divebomb();
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                animator.Play("Standing Greeting");
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                animator.Play("Standing Torch Melee Attack 03");
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                animator.Play("Chicken Dance");
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                animator.Play("shove reaction");
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                animator.Play("Zombie Stand Up");
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                animator.Play("Mutant Dying");
            }
            animator.SetInteger("walk", 0);
        }

    }

    bool IsPlayerInFront()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < 45f; // Player is in a 90-degree cone in front of the boss
    }


    void PhaseTwo()
    {
        phase = 2;
        health = 50;
        shield = 50;
        isShieldActive = true;
        //animator.Play("Zombie Stand Up");
    }

    void Die()
    {
        animator.Play("Mutant Dying");
    }

    bool inRage()
    {
        return Vector3.Distance(transform.position, player.position) < range;
    }

    void chase()
    {
        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
        animator.SetInteger("walk", 1);
    }

    void AttackPlayer()
    {
        if (phase == 1)
        {
            if (divebombTimer <= 0)
            {
                Divebomb();
                divebombTimer = divebombCooldown;
            }
            else if (summonMinionsTimer <= 0)
            {
                SummonMinions();
                summonMinionsTimer = summonMinionsCooldown;
            }
        }
        else if (phase == 2)
        {
            if (reflectiveAuraTimer <= 0 && isShieldActive)
            {
                ReflectiveAura();
                reflectiveAuraTimer = reflectiveAuraCooldown;
            }
            else if (bloodSpikesTimer <= 0)
            {
                BloodSpikes();
                bloodSpikesTimer = bloodSpikesCooldown;
            }
        }
    }

    IEnumerator TriggerBombEffectAfterDelay()
    {
        yield return new WaitForSeconds(1.5f); // Adjust to match animation length
        if (bombEffectPrefab != null)
        {
            Instantiate(bombEffectPrefab, transform.position, Quaternion.identity);
            Debug.Log("Bomb effect triggered at the end of animation!");
        }
    }


    IEnumerator waitForResecue()
    {
        yield return new WaitForSeconds(5f);
    }

    void Divebomb()
    {
        animator.Play("Jump Attack");
        Debug.Log("Boss performs Divebomb!");

        StartCoroutine(TriggerBombEffectAfterDelay());


        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, playerLayer);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                player player = hit.GetComponent<player>();
                if (player != null)
                {
                    player.TakeDamage(divebombDamage);
                }
            }
        }
        divebombTimer = divebombCooldown;
    }

    void SummonMinions()
    {
        animator.Play("SummonMinions");
        // Spawn 3 minions at random positions
    }

    void ReflectiveAura()
    {
        animator.Play("ReflectiveAura");
        isShieldActive = true;
        // Reflect damage logic
    }

    void BloodSpikes()
    {
        animator.Play("BloodSpikes");
        // Deal damage in a line or area
    }


    public void TakeDamage(int damage)
    {
        Debug.Log("boss health:- " + this.health);
        if (isShieldActive)
        {
            shield -= damage;
        }
        else
        {
            health -= damage;
        }

        if (health <= 0 && phase == 2)
        {
            Die();
        }
    }

}
