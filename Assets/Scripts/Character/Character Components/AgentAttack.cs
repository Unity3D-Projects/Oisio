﻿using UnityEngine;

public class AgentAttack : AgentComponent
{
    private Projectile arrowInstance;
    private EnemyDirectionAim enemy;

    private float cursorDeltaX;

    public AgentAttack(CharacterAgent agent) : base(agent)
    {
        enemy = new EnemyDirectionAim();
    }

    #region implemented abstract members of AgentComponent

    public override void FrameFeed()
    {
        CollectableType arrow = CollectableType.Arrow;

        if (agent.characterInventory.HasItem(arrow))
        {
            if(InputConfig.Aim())
            {
                Aim();
            }
            else if (InputConfig.ActionUp())
            {
                ThrowArrow();
                agent.characterInventory.UseItem(arrow);
            }
            else
                ResetAim();
        }
    }

    #endregion

    public void Aim()
    {
        CheckArrow();
        RotateAimer();
    }

    private void CheckArrow()
    {
        if (!arrowInstance)
        {
            arrowInstance = (GameObject.Instantiate(agent.arrowPrefab) as GameObject).GetComponent<Projectile>();
            arrowInstance.transform.SetParent(agent.arrowParent, false);
            arrowInstance.transform.localPosition = Vector3.zero;
        }

        arrowInstance.gameObject.SetActive(true);
    }

    private void RotateAimer()
    {
        cursorDeltaX += InputConfig.GetCursorMovement().y * AimingDirection() * agent.sensibility;
        Vector3 targetPos = enemy.NearbyEnemyDir(agent.transform.position);
        agent.aimerPivot.rotation = Quaternion.LookRotation(targetPos);
        agent.aimerPivot.rotation = Quaternion.Euler(agent.aimerPivot.localEulerAngles.x + cursorDeltaX, 
                                                     agent.aimerPivot.localEulerAngles.y, 
                                                     agent.aimerPivot.localEulerAngles.z);
    }

    public void ResetAim()
    {
        if (arrowInstance) arrowInstance.gameObject.SetActive(false);
        agent.aimerPivot.localEulerAngles = Vector3.zero;
        cursorDeltaX = 0f;
    }

    private int AimingDirection()
    {
        return agent.invertAiming ? 1 : -1;
    }

    public void ThrowArrow()
    {
        if (!arrowInstance) return;

        EventTrigger attackEvent = new CustomEvent(agent.aimerPivot.position, EventSubject.Attack, GameConfig.projectilePriority);

        // TODO : fix the direction calculation, the base position should be the head of the character, or the end of the arrow, not the agent
        Vector3 shootDirection = agent.arrowParent.position - agent.aimerPivot.position;
        arrowInstance.Shoot(attackEvent, shootDirection * agent.shootForce);

        arrowInstance.transform.SetParent(null);
        arrowInstance = null;
    }
}

public class EnemyDirectionAim
{
    private GameObject[] enemies;
    private string enemyTag = "Enemy";

    public EnemyDirectionAim()
    {
        enemies = GameObject.FindGameObjectsWithTag(enemyTag);
    }

    public Vector3 NearbyEnemyDir(Vector3 currentPos)
    {
        Vector3? closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach(GameObject go in enemies)
        {
            float distance = Vector3.Distance(currentPos, go.transform.position);
            if (!closestTarget.HasValue || distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = go.transform.position;
            }
        }

        Vector3 direction = (closestTarget.Value - currentPos).normalized;
        direction.y = 0;

        return direction;
    }

    public Vector3 NearbyEnemyPos(Vector3 currentPos)
    {
        Vector3? closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach(GameObject go in enemies)
        {
            float distance = Vector3.Distance(currentPos, go.transform.position);
            if (!closestTarget.HasValue || distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = go.transform.position;
            }
        }

        return closestTarget.Value;
    }
}