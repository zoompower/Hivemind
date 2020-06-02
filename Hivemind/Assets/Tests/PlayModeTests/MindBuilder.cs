using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static Gathering;

namespace Tests.PlayModeTests
{
    public class MindBuilder
    {
        private GameObject gameUI;
        private UiController uiController;
        private UnitController unitControl;

        [SetUp]
        public void Init()
        {
            gameUI = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/UI/IngameUI"));
             uiController = gameUI.GetComponent<UiController>();
             unitControl = gameUI.GetComponent<UnitController>();
        }

        [TearDown]
        public void Dispose()
        {
            Object.DestroyImmediate(gameUI);
        }

        [TestCase(ResourceType.Crystal, 2, Direction.East, true)]
        [TestCase(ResourceType.Rock, 2, Direction.East, true)]
        [TestCase(ResourceType.Unknown, 10, Direction.South, false)]
        [TestCase(ResourceType.Rock, 4, Direction.West, true)]
        public void UpdateResourceMindCorrectly(ResourceType resType, int carryweight, Direction exploreDirection, bool isScout = false)
        {
            MindGroup mindGroup = unitControl.MindGroupList.GetMindGroupFromIndex(1);
            Gathering gather = new Gathering();

            for (int i = 0; i < mindGroup.Minds.Count; i++)
            {
                if (mindGroup.Minds[i].GetType() == typeof(Gathering))
                {
                    gather = (Gathering)mindGroup.Minds[i];
                }
            }
            uiController.UI_OpenMindBuilder(1);
            MindBuilderTabbed mbTabbed = uiController.GetComponentInChildren<MindBuilderTabbed>();
            mbTabbed.UpdateResourceValues(carryweight, isScout, resType, exploreDirection);
            mbTabbed.UpdateMind();

            Assert.True(MindEquals(gather, new Gathering(resType, carryweight, exploreDirection, isScout)));
        }

        [UnityTest]
        public IEnumerator GetsCorrectMind()
        {
            bool valid = true;
            MindGroup mindGroup = unitControl.MindGroupList.GetMindGroupFromIndex(1);
           var Id =  mindGroup.AddUnit(new UnitGroup(uiController.unitIconBase));
            GameObject gameObjectAnt =  MonoBehaviour.Instantiate(Resources.Load("Prefabs/WorkerAnt") as GameObject);
            yield return new WaitForSeconds(0.01f);
            Ant ant = gameObjectAnt.GetComponent<Ant>();
            ant.SetunitGroupID(Id);
            ant.isAtBase = true;
            yield return new WaitForSeconds(0.01f);
            for(int i = 0; i < mindGroup.Minds.Count; i++)
            {
               if(!MindEquals(mindGroup.Minds[i], ant.GetMinds()[i]))
                {
                    valid = false;
                }
            }
            Assert.True(valid);
        }

        public bool MindEquals(IMind mind1, IMind mind2)
        {
            if(mind1.GetType() == mind2.GetType())
            {
                bool returnvalue = true;
                if(mind1.GetType() == typeof(Gathering))
                {
                    Gathering gather = (Gathering)mind1;
                    Gathering gather2 = (Gathering)mind2;
                    if(!(gather.prefferedType == gather2.prefferedType &&
                        gather.prefferedDirection == gather2.prefferedDirection &&
                        gather.carryWeight == gather2.carryWeight &&
                        gather.IsScout == gather2.IsScout))
                    {
                        returnvalue = false;
                    }
                }
                if (mind1.GetType() == typeof(CombatMind))
                {
                    CombatMind combat = (CombatMind)mind1;
                    CombatMind combat2 = (CombatMind)mind2;
                    if (!(combat.GetMinEstimatedDifference() == combat2.GetMinEstimatedDifference() &&
                        combat.GetPrefferedHealth() == combat2.GetPrefferedHealth()))
                    {
                        returnvalue =  false;
                    }
                }
                return returnvalue;
            }
            return false;
        }
    }
}
