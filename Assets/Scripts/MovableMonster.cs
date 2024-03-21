using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableMonster : Unit
{
    private int speed = 2;
    
    Vector3 direction;
    public void Start()
    {
        direction = transform.right * -1.0f;
    }

    public void Update()
    {
        Move();
    }

    protected SpriteRenderer sprite;
    public void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Move()
    {
        State = MovableMonsterState.Run;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + transform.up * 0.6f + transform.right * direction.x * 0.2f, 0.2f);
        if (colliders.Length > 1 && colliders.All(x => !x.GetComponent<Character>()))
        {
            direction *= -1.0f;
            sprite.flipX = !sprite.flipX;
        }

        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
    }

    public enum MovableMonsterState { Idle, Run };
    private Animator animator;
    private MovableMonsterState State
    {
        get { return (MovableMonsterState)animator.GetInteger("State"); }
        set { animator.SetInteger("State", (int)value); }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Unit unit = collision.GetComponent<Unit>();
        if (unit)
            if (unit is Character)
            {
                if (Mathf.Abs(unit.transform.position.x - transform.position.x) < 0.3f)
                    ReceiveDamage(unit);
                else
                    unit.ReceiveDamage(this);
            }
            else if (unit is Bullet)
            {
                ReceiveDamage(unit);
            }
    }
}
