using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour{

	//VAR PUB
	public Transform targetPosition;
    public Transform groundCheck;
    public float jumpForce = 5f;
    public GameObject rightCollider;
    public string tagCible = "Ground";
    public LayerMask groundLayer;
    public GameObject monster;
	//VAR PRIV
	private Node[] allNodes;
    private Stack<Node> currentPath = new Stack<Node>();
	public float distanceToNode = 1.0f;
	public int moveSpeed = 5;
    private bool targetIsLeft = false;
    private bool lastFacingLeft = false;
    public GameObject contactPlayerLogic;
    private bool enSaut = false;
    private GameObject player;
    private Rigidbody2D rb;
	private void Start() {
        player = GameObject.FindWithTag("Player");
        rb = monster.GetComponent<Rigidbody2D>();

		GameObject[] auxAllNodes = GameObject.FindGameObjectsWithTag("Node");
		allNodes = new Node[auxAllNodes.Length];
		for (int i = 0; i < auxAllNodes.Length; i++) {
			allNodes[i] = auxAllNodes[i].GetComponent<Node>();
		}
    }

    private bool IsGrounded() {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
    private void Jump() {
        Debug.Log("jumping");
        enSaut = true;

        float direction = (!targetIsLeft) ? 1 : -1;
        Vector2 sautDirection = new Vector2(direction, 2).normalized;
        monster.GetComponent<Rigidbody2D>().velocity = sautDirection * jumpForce;
    }

	void Update(){
		foreach (Node node in allNodes) {
			node.IsWay(false);
		}

		Stack<Node> stack = NavigateTo(targetPosition.position);
		while (stack.Count > 0) {
			Node node = stack.Pop();
			node.IsWay(true);
		}
        currentPath = NavigateTo(targetPosition.position);

        float distanceX = player.transform.position.x - monster.transform.position.x;

        if (contactPlayerLogic.GetComponent<GestionPv>().GetIsAlive()) {
            // animator.SetBool("isRunning", true);
            targetIsLeft = (distanceX < 0) ? true : false;
            OrientedVisuel();
		    MoveAlongPath();
        }

	}

    private void MoveAlongPath()
    {
        Vector2 direction;
       if (CheckCollisionWithTag(rightCollider) && !enSaut && IsGrounded()) {
            Jump();
        }
        if (enSaut){
            if (IsGrounded()) {
                enSaut = false;
            }
            else{
                Debug.Log("en l aire");
                direction = (player.transform.position - monster.transform.position).normalized;
                monster.transform.Translate(direction * moveSpeed * Time.deltaTime);
                return;
            }
        }
        
        //else{
        //    direction = new Vector2((player.transform.position - transform.position).normalized.x, 0.0f);
         //   monster.transform.Translate(direction * moveSpeed * Time.deltaTime);
        //}


        // Obtenez le prochain nœud à suivre
        direction = (targetPosition.position - monster.transform.position).normalized;
        if (currentPath.Count > 0){
            Node nextNode = currentPath.Peek();

            // Calculez la direction vers le prochain nœud
            direction = (nextNode.transform.position - monster.transform.position).normalized;

            // Vérifiez si l'objet est assez proche du nœud pour passer au suivant
            if (Vector2.Distance(monster.transform.position, nextNode.transform.position) < distanceToNode)
            {
                Debug.Log("changer de noeud");
                // Retirez le nœud actuel du chemin
                currentPath.Pop();

                // Vérifiez s'il reste des nœuds dans le chemin
                if (currentPath.Count == 0) {
                    float distanceToTarget = Vector2.Distance(monster.transform.position, targetPosition.position);

                    // Si la distance est supérieure à 4 unités, continuez à avancer vers la cible
                    if (distanceToTarget > 0.3f)
                    {
                        Debug.Log("bouger");
                        // Calculez la nouvelle direction vers la cible
                        direction = (targetPosition.position - monster.transform.position).normalized;
                    }
                    else {
                        // Si la distance est inférieure à 4 unités, vous êtes arrivé à destination
                        Debug.Log("Arrivé à destination !");
                        return;
                    }
                }
                else
                {
                    Debug.Log("next node");

                    // Obtenez le nouveau prochain nœud
                    nextNode = currentPath.Peek();
                    direction = (nextNode.transform.position - monster.transform.position).normalized;
                }
            }else{
                Debug.Log("pas le prochain noeud" + nextNode.transform.position);
            direction = (nextNode.transform.position - monster.transform.position).normalized;

            }
        }else{
            Debug.Log("arrivé?");
        }

        // Déplacez l'objet dans la direction
        monster.transform.Translate(direction * Time.deltaTime * moveSpeed);
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
        return false;

    }

    /**************/
    /** ALGO STAR**/
    /**************/
    public Stack<Node> NavigateTo(Vector2 destination){

		//1. Inicializacion
		Stack<Node> path = new Stack<Node>();
		Node currentNode = FindClosestNode(monster.transform.position); //Inicialmente el nodo más cercano al inicio
		Node endNode = FindClosestNode(destination);

		//Verificar existencia de inicio, final y diferencia entre ambos
		if (currentNode == null || endNode == null || currentNode == endNode)
			return path;
		
		//Lista abierta, almacena nodos para analizar (busqueda del vecino mas optimo)
		SortedList<float, Node> openList = new SortedList<float, Node>();
		//Lista cerrada, almacena nodos ya analizados
		List<Node> closedList = new List<Node> ();

		//Parametros del primer nodo
		openList.Add (0, currentNode);
		currentNode.SetParent(null);
		currentNode.SetDistance(0f);

		//2. Analisis de nodos hasta llegar al final
		while (openList.Count > 0){

			//Obtener el de la lista ordenada el valor mas cercano
			currentNode = openList.Values[0];
			openList.RemoveAt (0);			
			float dist = currentNode.GetDistance();
			closedList.Add (currentNode);

			//Finalizar al llegar al objetivo
			if (currentNode == endNode)
				break;
			
			//Recorrer nodos vecinos
			foreach (Node neighbor in currentNode.GetNeighbors()){

				//Ignorar analisis si ya han sido analizado (lista cerrada) o esta pendiente (lista abierta) 
				if (closedList.Contains (neighbor) || openList.ContainsValue (neighbor))
					continue;

				//Almacenar el nodo vecino en la lista abierta ordenado por distancia
				neighbor.SetParent(currentNode);
				neighbor.SetDistance(dist + Vector2.Distance(neighbor.transform.position, currentNode.transform.position));
				float distanceToTarget = Vector2.Distance(neighbor.transform.position, endNode.transform.position);
				openList.Add (neighbor.GetDistance() + distanceToTarget, neighbor);
			}
		}

		if (currentNode == endNode){
			while (currentNode.GetParent() != null){
				path.Push (currentNode);
				currentNode = currentNode.GetParent();
			}
		}

		return path;
	}


	private Node FindClosestNode(Vector2 targetPosition){
		Node closest = null;
		float minDist = float.MaxValue;

		for (int i = 0; i < allNodes.Length; i++) {
			float dist = Vector2.Distance(allNodes[i].transform.position, targetPosition);
			if (dist < minDist) {
				minDist = dist;
				closest = allNodes[i];
			}
		}

		return closest;
	}

}