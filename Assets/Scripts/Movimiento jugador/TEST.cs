using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : MonoBehaviour
{
    public GameObject lanza;

    public Transform spawner;
    public GameObject bulletPrefab;
    private float horizontal;
    private float speed = 10f;
    private float jumpForce = 18f;
    private bool isFacingRight = true;

    public bool isGrounded;
    private bool canDoubleJump;

    public float knockBackLength, knockBackForce; //Valor que tendrá el contador de KnockBack, y la fuerza de KnockBack
    private float knockBackCounter;

    //Planeo
    public float slideForce;

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 26f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = .5f;

    [SerializeField] private Rigidbody2D theRB;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private SpriteRenderer theSR;

    public static TEST sharedInstance;

    private void Awake()
    {
        if (sharedInstance == null)
        {
            sharedInstance = this;
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.position = spawner.position;
            bullet.transform.rotation = lanza.transform.rotation;
            Destroy(bullet, 2f);
        }


        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, .2f, whatIsGround);

        if (isDashing)
        {
            return;
        }

        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                theRB.velocity = new Vector2(theRB.velocity.x, jumpForce);
                canDoubleJump = true;
            }
            else
            {
                if (canDoubleJump)
                {
                    theRB.velocity = new Vector2(theRB.velocity.x, jumpForce);
                    canDoubleJump = false;
                }
            }
        }

        if (Input.GetButtonUp("Jump") && theRB.velocity.y > 0f)
        {
            theRB.velocity = new Vector2(theRB.velocity.x, theRB.velocity.y * 0.5f);
        }

        if (Input.GetButton("Dash") && canDash)
        {
            StartCoroutine(Dash());
        }

        Flip();

        //Si se pulsa el boton de Slide
        if (Input.GetButton("Slide"))
        {
            //Slide
            theRB.velocity = new Vector2(theRB.velocity.x, slideForce);
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        theRB.velocity = new Vector2(horizontal * speed, theRB.velocity.y);
    }


    
    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }


    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = theRB.gravityScale;
        theRB.gravityScale = 0f;
        theRB.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        theRB.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }


}
