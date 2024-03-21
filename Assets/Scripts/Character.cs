using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Unit
{
    new private Rigidbody2D rigidbody;
    private Animator animator;
    private SpriteRenderer sprite;

    public enum CharState { Idle, Run, Jump, Fall };

    private bool IsGrounded = false;

    [SerializeField]
    private float Speed = 10;
    private int JumpForce = 10;
    private int Lives = 1;


    private Bullet bullet;

    private void Awake()
    {
        bullet = Resources.Load<Bullet>("Bullet");
        Debug.Log(bullet);
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Run()
    {
        if (IsGrounded) State = CharState.Run;

        Vector3 direction = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, Speed * Time.deltaTime);

        sprite.flipX = direction.x < 0;
    }

    void Jump()
    {
        rigidbody.AddForce(transform.up * JumpForce, ForceMode2D.Impulse);
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1")) Shoot();

        if (IsGrounded) State = CharState.Idle;

        if (Input.GetButton("Horizontal")) Run();
        if (IsGrounded && Input.GetButtonDown("Jump")) Jump();
    }

    private void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.3f);
        IsGrounded = true;

        float YVelocity = rigidbody.velocity.y;
        if (!IsGrounded && YVelocity > 0)
            State = CharState.Jump;
        else if (!IsGrounded && YVelocity < 0)
            State = CharState.Fall;
    }

    private void Shoot()
    {
        Vector3 position = transform.position;
        position.y += 0.5f;
        Bullet newBullet = Instantiate(bullet, position, bullet.transform.rotation);
        newBullet.Direction = newBullet.transform.right * (sprite.flipX ? -1.0f : 1.0f);
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    private CharState State
    {
        get { return (CharState)animator.GetInteger("State"); }
        set { animator.SetInteger("State", (int)value); }
    }

    public override void ReceiveDamage(Unit enemy)
    {
        Lives--;
        if (Lives == 0) Die();
        else
        {
            Vector2 dir = (transform.position - enemy.transform.position).normalized;
            rigidbody.velocity = Vector2.zero;
            rigidbody.AddForce(dir * 7.0f, ForceMode2D.Impulse);
        }
    }
}
