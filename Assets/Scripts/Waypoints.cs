using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Waypoints : MonoBehaviour
{
    NavMeshAgent agent;
    Animator anim;
    public Transform target;
    public Transform eye;

    bool patrolling;
    public Transform[] patrolTargets;
    Vector3 lastKnownPosition;
    private int destinationPoint;
    bool arrived;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        // Set lastKnownPosition to our own position/agent position.
        lastKnownPosition = transform.position;
    }
    bool CanSeeTarget()
    {
        bool canSee = false;
        Ray ray = new Ray(eye.position, target.transform.position - eye.position);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            if(hit.transform != target)
            {
                canSee = false;
            }
            else
            {
                lastKnownPosition = target.transform.position;
                canSee = true;
            }
        }
        return canSee;
    }


    // Update is called once per frame
    void Update()
    {
        if(agent.pathPending)
        {
            // checks if the agent is trying to find out where to go if so return/wait/do nothing.
            return;
        }
        if(patrolling)
        {
            if(!arrived)
            {
                arrived = true;
                StartCoroutine("GoToNextPoint");
            }
            else
            {
                arrived = false;
            }
        }
        if(CanSeeTarget())
        {
            agent.SetDestination(target.transform.position);
            patrolling = false;
            if(agent.remainingDistance < agent.stoppingDistance)
            {
                anim.SetBool("Attack", true);
            }
            else
            {
                anim.SetBool("Attack", false);

            }
        }
        else
        {
            anim.SetBool("Attack", false);
            if(!patrolling)
            {
                agent.SetDestination(lastKnownPosition);
                if(agent.remainingDistance < agent.stoppingDistance)
                {
                    patrolling = true;
                    StartCoroutine("GoToNextPoint");
                }
            }
        }
        anim.SetFloat("Forward", agent.velocity.sqrMagnitude);
    }
    IEnumerator GoToNextPoint()
    {
        if(patrolTargets.Length == 0)
        {
            yield break;
        }
        patrolling = true;
        yield return new WaitForSeconds(2f);
        arrived = false;
        agent.destination = patrolTargets[destinationPoint].position;
        destinationPoint = (destinationPoint + 1) % patrolTargets.Length;
    }
}
