﻿using UnityEngine;
using Oisio.Game;
using Oisio.Agent;

namespace Oisio.Agent.Component
{
    public class ConsumableRechargeComponent : AgentComponent
    {
        private float minCollectPercentage = Mathf.Epsilon;

        private ConsumableAgent agent;

        public ConsumableRechargeComponent(ConsumableAgent agent)
        {
            this.agent = agent;
        }

        #region AgentComponent implementation
        public void FrameFeed()
        {
            UpdateState();  
        }
        #endregion

        private void UpdateState()
        {
            if (agent.percentage < minCollectPercentage && agent.ConsumableState.Equals(ConsumableAgent.ChargeState.Charged))
            {
                agent.ConsumableState = ConsumableAgent.ChargeState.Charging;
            }
            else
            {
                agent.percentage += Time.deltaTime/agent.reloadTime;
                if (agent.percentage > 1) agent.ConsumableState = ConsumableAgent.ChargeState.Charged;
            }

            agent.percentage = Mathf.Clamp01(agent.percentage);
        }

        public void Consume(Consumer consumer)
        {
            if (agent.ConsumableState.Equals(ConsumableAgent.ChargeState.Charging)) return;

            agent.percentage -= Time.deltaTime/agent.collectionTime;

            if (agent.percentage < minCollectPercentage)
            {
                consumer.Collected(agent.Item, agent.CollectableAmount);
                PooledObjects.Instance.RequestGameObject(GameObjectPool.ResourcesPickup).transform.position = agent.WorlPos + new Vector3(0f, 1f, 0f);
            }
        }
    }
}