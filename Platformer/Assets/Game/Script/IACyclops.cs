using UnityEngine;

public class IACyclops : MonoBehaviour
{
    public GameObject monster;
    public string tagCible = "Ground";
    public GameObject player;
    public GameObject contactPlayerLogic;
    public GameObject topRightCollider;
    public GameObject rightCollider;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float puissanceSaut = 10f;
    public float viewDistance = 15f;
    public float vitesseDeDeplacement = 5f;
    public float throwDistance = 12f;
    public float moveDistance = 5f;    

    private GameObject[] vosObjets;
    private bool enSaut = false;
    private bool estAProximite;
    private bool targetIsLeft = false;
    private bool lastFacingLeft = false;
    private Animator animator;
    private float timeSinceLastThrow = 0f;

    void Start(){
        player = GameObject.FindWithTag("Player");
        vosObjets = new GameObject[] { topRightCollider, rightCollider};
        animator = monster.GetComponent<Animator>();
    }
    
    void Update() {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        float distanceX = player.transform.position.x - transform.position.x;
        timeSinceLastThrow += Time.deltaTime;

        estAProximite = distanceToPlayer < viewDistance;

        if (estAProximite && contactPlayerLogic != null) {                      // if cyclops see player
            if (contactPlayerLogic.GetComponent<GestionPv>().GetIsAlive()) {    // if we are alive
                targetIsLeft = (distanceX < 0) ? true : false;                  // if is left
                OrientedVisuel();
                
                if (distanceToPlayer <= moveDistance){
                    Move();
                }else if (distanceToPlayer <= throwDistance){
                    if (timeSinceLastThrow >= 2f) {
                        ThrowRock();
                        timeSinceLastThrow = 0f;
                    }
                }else{
                    Move();
                }
            }
        }
    }

    public void ThrowRock(){
        monster.GetComponent<Cyclops>().animationThrow();
        Invoke("CancelThrowAnimation", 1f);
        Debug.Log("throw rock");
    }
    private void CancelThrowAnimation()
    {
        monster.GetComponent<Cyclops>().animationThrowCancel();
    }

    private void Flip()
    {
        lastFacingLeft = targetIsLeft;
        Vector3 localScale = monster.transform.localScale;
        localScale.x *= -1f;
        monster.transform.localScale = localScale;
    }

    private void OrientedVisuel(){
        if (targetIsLeft != lastFacingLeft){
            Flip();
        }
    }
    private void Move()
    {
        if (CheckCollisionWithTag(topRightCollider)){
            return;
        }
        if (enSaut) {
            if (IsGrounded()) {
                    enSaut = false;
            }
            else{
                if (!CheckCollisionWithTag(rightCollider)){
                    Vector2 direction = new Vector2((player.transform.position - transform.position).normalized.x, 0.2f);
                    monster.transform.Translate(direction * vitesseDeDeplacement * Time.deltaTime);
                }

            }
        }
        else {            
            if (CheckCollisionWithTag(rightCollider)) {
                    Jump();
                }
            if (enSaut){
                Vector2 direction = (player.transform.position - transform.position).normalized;
                monster.transform.Translate(direction * vitesseDeDeplacement * Time.deltaTime);
            }
            else{
                Vector2 direction = new Vector2((player.transform.position - transform.position).normalized.x, 0);
                monster.transform.Translate(direction * vitesseDeDeplacement * Time.deltaTime);
            }
        }
    }

    private bool IsGrounded() {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Jump() {
        enSaut = true;

        float direction = (!targetIsLeft) ? -1 : 1;
        Vector2 sautDirection = new Vector2(direction, 2).normalized;
        monster.GetComponent<Rigidbody2D>().velocity = sautDirection * puissanceSaut;
    }


    private bool CheckCollisionWithTag(GameObject objet)
    {
        Collider2D collider = objet.GetComponent<Collider2D>();

        if (collider != null)
        {
            // Utilisez OverlapBox pour vérifier la collision avec le tag cible
            Collider2D[] collisions = Physics2D.OverlapBoxAll(objet.transform.position, collider.bounds.size, 0f);

            // Parcourez les collisions pour voir si l'une d'elles a le tag cible
            foreach (Collider2D collision in collisions)
            {
                if (collision.CompareTag(tagCible))
                {
                    return true;
                }
            }
        }

        // Si aucune collision n'est détectée, retournez false
        return false;
    }
}
