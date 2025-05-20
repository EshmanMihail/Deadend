using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class AtakePlayerScript : MonoBehaviour
{
    [SerializeField] private float distanceToAtake = 1f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float atakeRate = 2f;

    private Animator animator;

    private GameObject player;
    private float timerAtake;

    void Start()
    {
        animator = GetComponent<Animator>();
    }


    private void FixedUpdate()
    {
        if (timerAtake > 0)
        {
            timerAtake -= Time.fixedDeltaTime;
        }
        
        if (timerAtake <= 0)
        {
            if (player != null)
            {
                animator.SetTrigger("Atake");
                player.GetComponent<HealthBar>().CmdTakeDamage(damage);
                timerAtake = atakeRate;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = null;
        }

    }
}
