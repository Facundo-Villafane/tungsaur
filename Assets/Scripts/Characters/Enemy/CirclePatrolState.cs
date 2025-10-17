using UnityEngine;

public class CirclePatrolState : EnemyState
{
    private Vector3 targetSlotPosition;
    private float slotChangeTimer;
    private float slotChangeInterval = 3f;
    private Transform player; // referencia al jugador

    public CirclePatrolState(EnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        // Obtener player desde SlotManager si existe, si no buscar por tag
        player = SlotManager.Instance != null ? SlotManager.Instance.Player : GameObject.FindWithTag("Player")?.transform;

        // Solicita un slot libre al SlotManager (si no hay SlotManager queda en su posición)
        if (SlotManager.Instance != null)
            targetSlotPosition = SlotManager.Instance.RequestSlot(enemy);
        else
            targetSlotPosition = enemy.transform.position;

        slotChangeTimer = 0f;
    }

    public override void Update()
    {
        if (enemy.IsDead) return;

        // Si no tenemos player, intentar recuperarlo (no forzamos cambio de estado aquí)
        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;

        // --- NUEVO: cambio inmediato a AttackState si el jugador está dentro de AttackRange ---
        if (player != null )
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            float distToPlayer = Vector3.Distance(enemy.transform.position, player.position);
            if (distToPlayer <= enemy.AttackRange && !playerController.IsDead)
            {
                Debug.Log("asd");
                // Cambiamos a AttackState (salimos inmediatamente de patrulla)
                enemy.ChangeState(new AttackState(enemy, player));
                return;
            }
        }
        // -------------------------------------------------------------------------------

        // lógica de cambio de slot
        slotChangeTimer += Time.deltaTime;
        if (slotChangeTimer >= slotChangeInterval)
        {
            if (enemy.AssignedSlot >= 0 && SlotManager.Instance != null)
                SlotManager.Instance.ReleaseSlot(enemy.AssignedSlot);

            if (SlotManager.Instance != null)
                targetSlotPosition = SlotManager.Instance.RequestSlot(enemy);
            else
                targetSlotPosition = enemy.transform.position;

            slotChangeTimer = 0f;
        }

        // Calcular dirección hacia el slot (solo XZ)
        Vector3 direction = targetSlotPosition - enemy.transform.position;
        direction.y = 0f;

        if (direction.magnitude > 0.01f)
            direction.Normalize();

        // Aplicar velocidad
        Vector3 velocity = direction * enemy.MoveSpeed;
        enemy.SetVelocity(velocity);

        // Si llegó al slot (puedes decidir conducta adicional aquí)
        float distance = Vector3.Distance(enemy.transform.position, targetSlotPosition);
        if (distance < 0.5f)
        {
            // Por ahora no hacemos nada más cuando llega; si luego querés atacar al llegar,
            // el chequeo al comienzo del Update() lo realizará (si el player está dentro de AttackRange).
        }
    }

    public override void Exit()
    {
        // Liberar slot para otros enemigos
        if (enemy.AssignedSlot >= 0 && SlotManager.Instance != null)
        {
            SlotManager.Instance.ReleaseSlot(enemy.AssignedSlot);
            enemy.AssignedSlot = -1;
        }

        enemy.SetVelocity(Vector3.zero);
    }
}
