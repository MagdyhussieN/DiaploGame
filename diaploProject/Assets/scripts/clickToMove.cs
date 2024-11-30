using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class clickToMove : MonoBehaviour
{
    const string IDLE = "Idle";
    const string WALK = "Walk";
    const string Bash = "Bash";
    const string Shield = "Shield";


    NavMeshAgent agent;
    Animator animator;
    public float speed;
    //public CharacterController controller;
    private Vector3 position;
    float lookRotationSpeed = 8f;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            animator.Play(Shield);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            animator.Play(Bash);
        }
        else if(Input.GetMouseButton(0))
        {
            locatePosition();
            moveToPosition();

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
            animator.Play(WALK);
        }
        else
        {
            animator.Play(IDLE);
        }
    }
}
