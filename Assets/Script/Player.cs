using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int maxHealth = 4;
    public Text health; 
    private float movement;
    public float moveSpeed;
    private bool facingRight = true;
    public Rigidbody2D rb;
    public float jumpHeight;
    public bool isGround = true;
    public Animator animator;

    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask attackLayer;

    public int currentCoin = 0;
    public Text coinText;

    public GameObject YouDie;

    public AudioSource aus;
    public AudioClip attackSound;
    public AudioClip GetHitSoud;
    public AudioClip GetCoinSound;

    [Header("Mobile Controls")]
    private bool moveLeft = false;
    private bool moveRight = false;
    private bool jumpPressed = false;
    private bool attackPressed = false;





    // Start is called before the first frame update
    void Start()
    {
        YouDie.SetActive(false);
    }

    // Update is called once per frame  
    void Update()
    {
        

        if (maxHealth <= 0)
        {
            Die();
           
        }

        coinText.text = currentCoin.ToString();

        health.text = maxHealth.ToString();

        movement = 0f;
        if (moveLeft) movement = -1f;
        if (moveRight) movement = 1f;// -1 trai 0 dung im 1 la ben phai

        if (movement < 0f && facingRight == true )
        {
            transform.eulerAngles = new Vector3(0f, -180, 0f);
            facingRight = false;

        }else if(movement > 0f && facingRight == false)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            facingRight = true;
        }

        if (jumpPressed && isGround)
        {
            Jump();
            isGround = false;
            animator.SetBool("Jump", true);
            jumpPressed = false; 
        }

        if (Mathf.Abs(movement) > 0.1f)
        {
            animator.SetFloat("Run", 1f);
        }else if(movement < 0.1f)
        {
            animator.SetFloat("Run", 0f);
        }

        if (attackPressed)
        {
            if (aus != null && attackSound != null)
            {
                aus.PlayOneShot(attackSound);
            }
            animator.SetTrigger("Attack");
            attackPressed = false; // Reset sau khi attack
        }



    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(movement, 0f, 0f) * Time.fixedDeltaTime * moveSpeed;
    }
    
    void Jump()
    {
        float limitedJumpHeight = Mathf.Min(jumpHeight, 18f);
        rb.AddForce(new Vector2(0f, limitedJumpHeight), ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            isGround = true;
            animator.SetBool("Jump",false);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1); 
        }
    }

    public void Attack()
    {
       Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius,attackLayer);
        if (collInfo == true) 
        {
            if (collInfo.gameObject.GetComponent<PatrolEnemy>() != null) 
            {
                collInfo.gameObject.GetComponent<PatrolEnemy>().TakeDamage(1);  
                 
            }
        }
    }

     void OnDrawGizmosSelected()
     {
     if(attackPoint== null)
        {
            return;
        }
       Gizmos.color = Color.red;
       Gizmos.DrawWireSphere(attackPoint.position, attackRadius);   
     }

    public void TakeDamage(int damage)
    {
        if(maxHealth <= 0)
        {
            return;
        }

        maxHealth -= damage;
        animator.SetTrigger("Hurt");

        if (aus != null && GetHitSoud != null)
        {
            aus.PlayOneShot(GetHitSoud);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Coin")
        {
            currentCoin ++;
            other.gameObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Collected");
            Destroy(other.gameObject, 1f); // 1f tuc la thoi gian ma coin bien mat sau khi collected
            if (aus != null && GetCoinSound!= null)
            {
                aus.PlayOneShot(GetCoinSound);
            }


        }
        

        if(other.gameObject.tag == "VictoryPoint" )
        {            
            FindObjectOfType<SceneManagement>().LoadSceneVictory(); 
        }

        if(other.gameObject.tag == "DeathZone")
        {
            Die();
        }

    }

    void Die() 
    {
        
        
        FindObjectOfType<GameManager>().isGameActive = false;
        YouDie.SetActive(true);
                   
        Destroy(this.gameObject);

    }

    
    public void OnMoveLeftDown() { moveLeft = true; }
    public void OnMoveLeftUp() { moveLeft = false; }

    public void OnMoveRightDown() { moveRight = true; }
    public void OnMoveRightUp() { moveRight = false; }

    public void OnJumpPressed() { jumpPressed = true; }

    public void OnAttackPressed() { attackPressed = true; }

}
