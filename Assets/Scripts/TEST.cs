using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : MonoBehaviour
{
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

       //OverlapCircle(punto donde se genera el círculo, radio del círculo, layer a detectar)
                                                                                           //Si el contador de KnockBack se ha vaciado, el jugador recupera el control del movimiento
        //if (knockBackCounter <= 0)
        //{

        //        //Si se pulsa el botón de salto
        //        if (Input.GetButtonDown("Jump"))
        //    {
        //        //Si el jugador está en el suelo
        //        if (isGrounded)
        //        {
        //            //El jugador salta, manteniendo su velocidad en X, y aplicamos la fuerza de salto
        //            theRB.velocity = new Vector2(theRB.velocity.x, jumpForce);
        //            //Una vez en el suelo, reactivamos la posibilidad de doble salto
        //            canDoubleJump = true;


        //        }
        //        //Si el jugador no está en el suelo
        //        else
        //        {

        //            //Si la variable booleana canDoubleJump es verdadera
        //            if (canDoubleJump)
        //            {
        //                //El jugador salta, manteniendo su velocidad en X, y aplicamos la fuerza de salto
        //                theRB.velocity = new Vector2(theRB.velocity.x, jumpForce);
        //                //Hacemos que no se pueda volver a saltar de nuevo
        //                canDoubleJump = false;


        //            }
        //        }
        //    }
        //}
        //else
        //{
        //    //Hacemos decrecer el contador en 1 cada segundo
        //    knockBackCounter -= Time.deltaTime;
        //    //Si el jugador mira a la izquierda
        //    if (!theSR.flipX)
        //    {
        //        //Aplicamos un pequeño empuje a la derecha
        //        theRB.velocity = new Vector2(knockBackForce, theRB.velocity.y);
        //    }
        //    //Si el jugador mira a la derecha
        //    else
        //    {
        //        //Aplicamos un pequeño empuje a la izquierda
        //        theRB.velocity = new Vector2(-knockBackForce, theRB.velocity.y);
        //    }
        //}

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
