using System;
using UnityEngine;
using static CombatMind;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class CombatData : MindData
    {
        public int PrefferedHealth;
        public string AntGuid;
        public bool Busy;
        public State State;
        public bool LeavingBase;
        public State NextState;
        public bool EnterBase;
        public float TeleporterEntranceX;
        public float TeleporterEntranceY;
        public float TeleporterEntranceZ;
        public bool AttackingQueen;
        public string TargetGuid;
        public int EngageRange;
        public bool EnteredEnemyBase;

        public CombatData(int prefferedHealth, Ant ant, bool busy, State state, bool leavingBase, State nextState, bool enterBase, Vector3 teleporterEntrance, bool attackingQueen, Ant target, int engageRange, bool enteredEnemyBase)
        {
            PrefferedHealth = prefferedHealth;
            if (ant != null)
            {
                AntGuid = ant.myGuid.ToString();
            }
            Busy = busy;
            State = state;
            LeavingBase = leavingBase;
            NextState = nextState;
            EnterBase = enterBase;
            TeleporterEntranceX = teleporterEntrance.x;
            TeleporterEntranceY = teleporterEntrance.y;
            TeleporterEntranceZ = teleporterEntrance.z;
            AttackingQueen = attackingQueen;
            if (target != null)
            {
                TargetGuid = target.myGuid.ToString();
            }
            EngageRange = engageRange;
            EnteredEnemyBase = enteredEnemyBase;
        }
    }
}