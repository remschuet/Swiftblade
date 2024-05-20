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
    public GameObject rockPrefab;
public float throwSpeed = 5f; // Ajustez la vitesse de lancer selon vos besoins

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

    public void ThrowRock() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null) {
            // Créer une instance de la préfabriquée
            GameObject rockInstance = Instantiate(rockPrefab, transform.position, Quaternion.identity);

            // Calculer la direction vers le joueur
            Vector3 direction = player.transform.position - transform.position;
            direction.y += 10f; // Ajouter 2 unités à la hauteur

            // Normaliser la direction pour obtenir une direction unitaire
            direction.Normalize();

            // Calculer l'angle de tir (en radians)
            float angle = Mathf.Atan2(direction.y, direction.x);

            // Appliquer une force initiale pour un lancer droit avec une certaine force
            rockInstance.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * throwSpeed;

            // Déclencher l'animation de lancer sur le Cyclope
            monster.GetComponent<Cyclops>().animationThrow();

            // Appeler CancelThrowAnimation après 1 seconde
            Invoke("CancelThrowAnimation", 1f);

            Debug.Log("throw rock");
        } else {
            Debug.LogWarning("Player not found");
        }
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



/*public void ThrowRock() {
    GameObject player = GameObject.FindGameObjectWithTag("Player");

    if (player != null) {
        // Créer une instance de la préfabriquée
        GameObject rockInstance = Instantiate(rockPrefab, transform.position, Quaternion.identity);

        // Calculer la direction vers le joueur
        Vector3 direction = player.transform.position - transform.position;
        direction.Normalize();

        // Calculer l'angle de tir pour une trajectoire parabolique
        float angle = Mathf.Atan2(direction.y, direction.x);
        angle = angle * Mathf.Rad2Deg;

        // Appliquer une force initiale pour suivre une trajectoire parabolique
        rockInstance.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * throwSpeed;

        // Déclencher l'animation de lancer sur le Cyclope
        monster.GetComponent<Cyclops>().animationThrow();

        // Appeler CancelThrowAnimation après 1 seconde
        Invoke("CancelThrowAnimation", 1f);

        Debug.Log("throw rock");
    } else {
        Debug.LogWarning("Player not found");
    }
}*/