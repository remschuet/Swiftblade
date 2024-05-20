using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Image manaBar;
    public float ladderClimbSpeed = 5f;

    private Animator animator;
    private float horizontal;
    private float vertical;

    private float speed = 8f;
    private GestionPv gestionPv;
    [SerializeField] private float entityMana = 100;

    [SerializeField] private float jumpingPower = 8f;
    [SerializeField] private float dashSpeed = 24f;
    [SerializeField] private float slideSpeed = 24f;
    [SerializeField] private float dashAttackSpeed = 24f;
    private bool isFacingRight = true;
    private bool isAttacking = false;
    private bool isDashing = false;
    private bool isDashingAttack = false;
    private bool isSliding = false;
    private bool canDoubleJump = true; // New variable for double jump
    private int extraJumps = 1; // Set the number of extra jumps allowed
    private bool isCollidingWithLadder = false;
    private float defaultGravityScale;
    public void Awake(){
        animator = GetComponent<Animator>();
        gestionPv = GetComponent<GestionPv>();
        defaultGravityScale = rb.gravityScale;
    }
    void Update()
    {
        if (! gestionPv.GetIsAlive()){
            animator.SetBool("isAlive", false);
            return;
        }

        if (!IsGrounded() && !isDashing && !isDashingAttack && !animator.GetBool("inLadder")) {
            if (rb.velocity.y > 0) 
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isJumping", true); 
                animator.SetBool("isFalling", false);
            }
            else {
                animator.SetBool("isRunning", false);
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", true);
            }
        }
        else {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
            if (IsGrounded() && Mathf.Abs(horizontal) > 0 && !isDashing && !isSliding && !isDashingAttack)
            {
                animator.SetBool("isRunning", true);
            }
            else
            {
                animator.SetBool("isRunning", false);
            }

        }
        
        if (!isFacingRight && horizontal > 0f && !isDashing)
        {
            Flip();
        }
        else if (isFacingRight && horizontal < 0f && !isDashing)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        if (!isDashing && !isSliding)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
        if (isCollidingWithLadder){
            rb.velocity = new Vector2(rb.velocity.x, vertical * ladderClimbSpeed);
            animator.SetBool("isRunning", false);
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (! gestionPv.GetIsAlive()){
            return;
        }
        if (IsGrounded())
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isFalling", true);
            canDoubleJump = true; // Reset double jump when grounded
            extraJumps = 1; // Reset extra jumps when grounded
        }

        if (context.performed)
        {
            if (IsGrounded() || (canDoubleJump && extraJumps > 0))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                
                if (!IsGrounded())
                {
                    extraJumps--; // Decrease the available extra jumps
                    canDoubleJump = false;
                }
            }
        }
    }

    public void Attack(InputAction.CallbackContext context){
        if (! gestionPv.GetIsAlive()){
            return;
        }        
        if (context.performed) //&& IsGrounded()
        {
            StartCoroutine(AttackCoroutine());
        }        
    }

    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        // animator.SetBool("isDashing", true); // Démarre l'animation de dash
        animator.SetBool("isAttacking", true);

        float AttackTime = 0.5f;
        float startTime = Time.time;

        GetComponent<Player>().Attack();
       while (Time.time < startTime + AttackTime)
        {
            // rb.velocity = new Vector2((isFacingRight ? 1 : -1) * dashSpeed, rb.velocity.y);
            yield return null;
        }

        animator.SetBool("isAttacking", false); // Arrête l'animation de dash
        // gestionPv.canTakeDamage = true;
        isAttacking = false;
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (! gestionPv.GetIsAlive()){
            return;
        }
        if (context.performed) //&& IsGrounded()
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        isDashing = true;
        gestionPv.canTakeDamage = false;
        animator.SetBool("isRunning", false);
        animator.SetBool("isDashing", true); // Démarre l'animation de dash

        float dashTime = 0.2f;
        float startTime = Time.time;

        while (Time.time < startTime + dashTime)
        {
            rb.velocity = new Vector2((isFacingRight ? 1 : -1) * dashSpeed, rb.velocity.y);
            yield return null;
        }

        animator.SetBool("isDashing", false); // Arrête l'animation de dash
        gestionPv.canTakeDamage = true;
        isDashing = false;
    }

    public void DashAttack(InputAction.CallbackContext context)
    {
        if (! gestionPv.GetIsAlive()){
            return;
        }
        if (useMana(1)){
            if (context.performed)
            {
                StartCoroutine(DashAttackCoroutine());
            }
        }
    }

    private IEnumerator DashAttackCoroutine()
    {
        isDashing = true;
        gestionPv.canTakeDamage = false;
        animator.SetBool("isRunning", false);
        animator.SetBool("isDashing", false);
        animator.SetBool("isDashingAttacking", true);

        float dashTime = 0.25f;
        float startTime = Time.time;

        while (Time.time < startTime + dashTime)
        {
            rb.velocity = new Vector2((isFacingRight ? 1 : -1) * dashAttackSpeed, rb.velocity.y);
            yield return null;
        }

        animator.SetBool("isDashingAttacking", false); // Arrête l'animation de dash
        isDashing = false;
        gestionPv.canTakeDamage = true;

    }

   public void Slide(InputAction.CallbackContext context)
    {
        if (! gestionPv.GetIsAlive()){
            return;
        }
        if (context.performed && IsGrounded()) 
        {
            StartCoroutine(SlideCoroutine());
        }
    }

    private IEnumerator SlideCoroutine()
    {
        isSliding = true;
        animator.SetBool("isRunning", false);
        animator.SetBool("isDashing", false);
        animator.SetBool("isSliding", true);

        float slideTime = 0.4f;
        float startTime = Time.time;
        while (Time.time < startTime + slideTime)
        {
            rb.velocity = new Vector2((isFacingRight ? 1 : -1) * slideSpeed, rb.velocity.y);
            yield return null;
        }

        animator.SetBool("isSliding", false);
        isSliding = false;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (! gestionPv.GetIsAlive()){
            return;
        }
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void Ladder(InputAction.CallbackContext context) {
        if (! gestionPv.GetIsAlive()){
            return;
        }
        float verticalLadder; // Ajoutez cette ligne pour déclarer la variable vertical
        vertical = context.ReadValue<Vector2>().y;

        if (isCollidingWithLadder){
            rb.gravityScale = 0f;
            animator.SetBool("isClimbing", true);
        if (vertical != 0 && !isDashing) {
            animator.SetBool("isDashing", false); // Arrête l'animation de dash
            animator.SetBool("isRunning", false);
            animator.SetBool("isClimbing", true);
        }
        else {
            animator.SetBool("isClimbing", false);
        }


            // rb.velocity = new Vector2(rb.velocity.x, ladderClimbSpeed);
        }
        //    animator.SetBool("isRunning", false);
        //    animator.SetBool("isFalling", true);

    }

    public void Move(InputAction.CallbackContext context)
    {
        if (! gestionPv.GetIsAlive()){
            animator.SetBool("isRunning", false);
            return;
        }
        horizontal = context.ReadValue<Vector2>().x;
        
        if (horizontal != 0 && IsGrounded() && !isDashing)
        {
            animator.SetBool("isDashing", false); // Arrête l'animation de dash
            animator.SetBool("isRunning", true);
        }
        else
        {
            // Sinon, arrêter l'animation de course.
            animator.SetBool("isRunning", false);
        }
    }

    public bool useMana(int mana) {
        if (entityMana >= mana) {
            entityMana -= mana;
            manaBar.fillAmount = entityMana / 100f;
            return true;
        }
        return false;
    }

    public void refillMana(int mana){
        entityMana += mana;
        if (entityMana > 100) {
            entityMana = 100;
        }
        manaBar.fillAmount = entityMana / 100f;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Ladder")) {
            isCollidingWithLadder = true;
            rb.gravityScale = 0f;
            animator.SetBool("inLadder", true);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Ladder")) {
            isCollidingWithLadder = false;
            rb.gravityScale = defaultGravityScale;
            animator.SetBool("isClimbing", false);
            animator.SetBool("inLadder", false);

        }
    }
}
