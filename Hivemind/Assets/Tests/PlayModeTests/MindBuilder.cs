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

            Assert.True(gather.Equals(new Gathering(resType, carryweight, exploreDirection, isScout)));
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
            yield return new WaitForSeconds(1f);
            for(int i = 0; i < mindGroup.Minds.Count; i++)
            {
               if(!mindGroup.Minds[i].Equals(ant.GetMinds()[i]))
                {
                    valid = false;
                }
            }
            Assert.True(valid);
        }
    }
}
