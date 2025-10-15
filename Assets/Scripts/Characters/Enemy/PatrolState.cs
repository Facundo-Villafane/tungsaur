using UnityEngine;

public class PatrolState : EnemyState
{
    private Transform player;
    private Transform[] patrolPoints;
    private int currentIndex;
    private float arrivalThreshold = 1.5f; // Aumentado de 0.5f a 1.5f
    private float waitTimeAtPoint = 0.5f; // Tiempo de espera en cada punto
    private float waitTimer = 0f;
    private bool isWaiting = false;

    public PatrolState(EnemyController enemy, Transform[] patrolPoints) : base(enemy)
    {
        this.patrolPoints = patrolPoints;
        this.currentIndex = 0;
    }

    public override void Enter()
    {
        Debug.Log("PatrolState ENTERED");

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (enemy.Animator != null)
        {
            enemy.Animator.SetFloat("xVelocity", 0.5f);
        }
        
        isWaiting = false;
        waitTimer = 0f;
    }

    public override void Update()
    {
        if (enemy.IsDead) return;

        // Si el jugador está dentro del radio de detección, cambiar a Chase
        if (player != null && Vector3.Distance(enemy.transform.position, player.position) <= enemy.DetectionRadius)
        {
            enemy.ChangeState(new ChaseState(enemy));
            return;
        }

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            enemy.SetVelocity(Vector3.zero);
            return;
        }

        // Si está esperando en un punto
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                // Avanzar al siguiente punto
                currentIndex = (currentIndex + 1) % patrolPoints.Length;
                isWaiting = false;
                waitTimer = 0f;
                Debug.Log($"Avanzando al punto {currentIndex}");
            }
            return;
        }
        
        Transform targetPoint = patrolPoints[currentIndex];
        Vector3 targetPosition = targetPoint.position;
        Vector3 currentPosition = enemy.transform.position;
        
        // Calcular distancia en el plano horizontal (ignorando Y)
        Vector3 horizontalTarget = new Vector3(targetPosition.x, currentPosition.y, targetPosition.z);
        Vector3 horizontalCurrent = new Vector3(currentPosition.x, currentPosition.y, currentPosition.z);
        float distance = Vector3.Distance(horizontalCurrent, horizontalTarget);
        
        Debug.Log($"Patrullando hacia punto {currentIndex} ({targetPoint.name}), distancia: {distance:F2}");
        
        // Si llegó al punto, iniciar espera
        if (distance < arrivalThreshold)
        {
            Debug.Log($"Llegó al punto {currentIndex}, esperando...");
            enemy.SetVelocity(Vector3.zero);
            isWaiting = true;
            waitTimer = 0f;
            return;
        }

        // Calcular dirección en el plano horizontal (X, Z)
        Vector3 direction = (horizontalTarget - horizontalCurrent).normalized;

        // Establecer velocidad objetivo (X y Z)
        Vector3 targetVelocity = direction * enemy.MoveSpeed;
        enemy.SetVelocity(targetVelocity);
    }

    public override void Exit()
    {
        Debug.Log("PatrolState EXIT");
        
        if (enemy.Animator != null)
        {
            enemy.Animator.SetFloat("xVelocity", 0f);
        }
        
        enemy.SetVelocity(Vector3.zero);
    }
}