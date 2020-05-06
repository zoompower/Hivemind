using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CombatBehaviour;
using UnityEngine.AI;
using Assets.Scripts.Minds;
using System;

namespace Assets.Scripts
{
    public class Ant : MonoBehaviour, IHasMind
    {
        enum AntType
        {
            Worker,
            Soldier,
        }

        private int health;
        private int damage;
        private int unitgroupID;
        public float baseSpeed;
        public float currentSpeed;


        private IAntBehaviour behaviour;
        private ICombatAntBehaviour combatBehaviour;

        private NavMeshAgent agent;
        private Ant closestEnemy;
        private Storage storage;

        private CombatMind combatMind;
        private ResourceMind resMind;


        void Awake()
        {
            agent = gameObject.GetComponent<NavMeshAgent>();
            baseSpeed = agent.speed;
            currentSpeed = baseSpeed;
            behaviour = new Gathering();
            behaviour.Initiate(this);
        }
        // Start is called before the first frame update
        void Start()
        {
            storage = GameWorld.GetStorage();
            resMind = new ResourceMind(ResourceType.Unknown ,3);
        }

        // Update is called once per frame
        void Update()
        {
            if (this.InCombat())
            {
                combatBehaviour = DecideCombatBehavior();
                combatBehaviour.CombatMode(this, closestEnemy);
                return;
            }

            if (this.AtBase())
            {
                //combatMind = GetUnitGroup(unitgroupID).GetCombatMind();
                //resmind = GetUnitGroup(unitgroupID).GetResMind();
            }

            behaviour.Execute();
            agent.speed = currentSpeed;
        }

      public  bool AtBase()
        {
            if (Vector3.Distance(transform.position, storage.GetPosition()) < 2f)
            {
                return true;
            }
            return false;
        }

     public   bool InCombat()
        {
            return false;
        }
        
        ICombatAntBehaviour DecideCombatBehavior()
        {
            if (combatMind == null)
            {
                return new CombatFight();
            }

            float healthPercantageDifference = ((float) health / (float) closestEnemy.health);
            float damagePercantageDifference = ((float) damage / (float) closestEnemy.damage);
            float strengthDifference = (healthPercantageDifference * 1 + damagePercantageDifference * 2) / 3;
            if (strengthDifference >= combatMind.GetMinEstimetedDifference())
            {
                return new CombatFight();
            }
            return new CombatFlee();
        }

        public NavMeshAgent GetAgent()
        {
            return this.agent;
        }

        public Storage GetStorage()
        {
            return this.storage;
        }

        public ResourceMind GetResourceMind()
        {
            return resMind;
        }

        public CombatMind GetCombatMind()
        {
            return combatMind;
        }

        internal void UpdateSpeed()
        {
            agent.speed = currentSpeed;
        }
    }
}
