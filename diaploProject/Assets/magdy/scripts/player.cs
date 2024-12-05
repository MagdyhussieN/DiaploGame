using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{
    bool walking = false;
    [SerializeField] private ParticleSystem shieldParticles;

    NavMeshAgent agent;
    Animator animator;
    public float speed;
    //public CharacterController controller;
    private Vector3 position;
    float lookRotationSpeed = 8f;

    //For Abilitites
    float bashCooldown = 1f;
    float shieldCooldown = 10f;
    float ironMaelstormCooldown = 5f;
    float chargeCooldown = 10f;
    bool shieldActive = false;

    public int health = 100;
    public int XP = 0;

    public Transform boss;
    public LayerMask bossLayer;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        position = transform.position;
    }

    // Update is called once per frame
    void Update(){
        if(health <= 0)
        {
            Die();
        }
        if(Input.GetMouseButton(0))
        {
            locatePosition();
        }
        if (!agent.destination.Equals(transform.position))
        {
            moveToPosition();
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
        HandleAbilities();
    }

    void HandleAbilities()
    {
        // Update cooldowns
        if (bashCooldown > 0) bashCooldown -= Time.deltaTime;
        if (shieldCooldown > 0) shieldCooldown -= Time.deltaTime;
        if (ironMaelstormCooldown > 0) ironMaelstormCooldown -= Time.deltaTime;
        if (chargeCooldown > 0) chargeCooldown -= Time.deltaTime;


        // Handle Bash (Basic Ability)
        if (Input.GetKeyDown(KeyCode.Q) && bashCooldown <= 0)
        {
            Bash();
            bashCooldown = 1f;
            //animator.SetTrigger("basic attack");
        }

        // Handle Shield (Defensive Ability)
        if (Input.GetKeyDown(KeyCode.W) && shieldCooldown <= 0)
        {
            Shield();
            shieldCooldown = 10f;
            animator.SetTrigger("shield");

        }

        // Handle Iron Maelstorm (Wild Card Ability)
        if (Input.GetKeyDown(KeyCode.E) && ironMaelstormCooldown <= 0)
        {
            IronMaelstorm();
            ironMaelstormCooldown = 5f;
            animator.SetTrigger("bash");
        }

        // Handle Charge (Ultimate Ability)
        if (Input.GetKeyDown(KeyCode.R) && chargeCooldown <= 0)
        {
            Charge();
            chargeCooldown = 10f;
            animator.SetTrigger("attack jump");
        }
    }

    void Bash()
    {
        Debug.Log("starttt");
        //RaycastHit hit;
        //if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000))
        //{
            Debug.Log("starttt11111");
            animator.SetTrigger("basic attack");
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2, bossLayer);
            foreach (Collider hit in hitColliders)
            {
                if (hit.CompareTag("Boss"))
                {
                    boss boss = hit.GetComponent<boss>();
                    if (boss != null)
                    {
                        boss.TakeDamage(10);
                    }
                }
            }
            //if (hit.collider.CompareTag("Boss"))
            //{
            //    Debug.Log("starttt222222");
            //    boss boss = hit.collider.GetComponent<boss>();
            //    //if (enemyHealth != null)
            //    //{
            //    //    animator.Play(BashAnim);
            //    //    enemyHealth.TakeDamage(5); // Damage value for Bash
            //    //}
            //    boss.TakeDamage(5);
            //}
            
            //animator.Play(BashAnim);
        //}
    }

    void Shield()
    {
        if (!shieldActive)
        {
            shieldActive = true;
            if (shieldParticles != null)
            {
                shieldParticles.gameObject.SetActive(true);
                shieldParticles.Play();
            }
            // Shield effect (e.g., visual shield activation)
            //animator.Play(ShieldAnim);
            animator.SetTrigger("shield");
            StartCoroutine(ShieldDuration());
        }
    }

    // Shield duration coroutine
    IEnumerator ShieldDuration()
    {
        // Shield lasts for 3 seconds
        yield return new WaitForSeconds(3f);
        shieldActive = false;
        // End shield effect

        // Stop the particle effect
        if (shieldParticles != null)
        {
            shieldParticles.Stop();
            shieldParticles.gameObject.SetActive(false);
        }
        //animator.Play(IDLE);
    }

    // Implement Iron Maelstorm (Wild Card Ability)
    void IronMaelstorm()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, 5f); // Adjust radius as needed
        //foreach (var enemy in enemiesInRange)
        //{
        //    if (enemy.CompareTag("Enemy"))
        //    {
        //        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        //        if (enemyHealth != null)
        //        {
        //            animator.Play(IronAnim);
        //            enemyHealth.TakeDamage(10); // Damage value for Iron Maelstorm
        //        }
        //    }
        //}
        //animator.Play(IronAnim);
        animator.SetTrigger("Bash");
    }

    // Implement Charge (Ultimate Ability)
    void Charge()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000))
        {
            Vector3 chargeDirection = hit.point - transform.position;
            agent.SetDestination(hit.point); // Use NavMesh to charge the position

            // Optionally, apply damage to enemies in the charge path
            Collider[] enemiesInPath = Physics.OverlapSphere(transform.position, 1f); // Small radius for path
            foreach (var enemy in enemiesInPath)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(10); // Damage value for Charge
                    }
                }
            }
            animator.SetTrigger("attack jump");
            //animator.Play(WALK);
            //if (Vector3.Distance(transform.position, position) < 1)
            //{
            //    animator.Play(AttackJump);
            //}

        }

    }


    void locatePosition()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit , 1000))
        {
            agent.destination = hit.point;
            //position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
        }
    }

    void moveToPosition()
    {
        if (Vector3.Distance(transform.position, agent.destination) > 1)
        {
            //Quaternion newRotation = Quaternion.LookRotation(position - transform.position,Vector3.forward);
            //newRotation.x = 0f;
            //newRotation.z = 0f;
            //transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 10);
            Vector3 direction = (agent.destination - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
            //controller.SimpleMove(transform.forward * speed);
            //animator.Play(WALK);
            animator.SetBool("isWalking", true);
            
        }
    }


    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Player took damage. Remaining health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        animator.Play("Sword And Shield Death");
        Debug.Log("Player is dead!");
        // Add logic for player death (e.g., respawn, game over)
    }
}
