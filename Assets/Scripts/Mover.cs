using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{
    public AudioClip[] audioClips;
    private Animator animator;
    private AudioSource audioSource;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        if(agent != null)
        {
            if(agent.velocity.sqrMagnitude > 0)
            {
                animator.SetBool("Moving", true);
                animator.SetFloat("Velocity Z", agent.velocity.magnitude);
            }
            else
            {
                animator.SetFloat("Velocity Z", 0);
            }
        }
    }

    public void MoveTo(Vector3 destination)
    {
        agent.destination = destination;
        //agent.isStopped = false;
    }

    public void FootR()
    {
        audioSource.clip = audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
        audioSource.Play();
    }

    public void FootL()
    {
        audioSource.clip = audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
        audioSource.Play();
    }

    public void Stop()
    {
        agent.isStopped = true;
    }

}
