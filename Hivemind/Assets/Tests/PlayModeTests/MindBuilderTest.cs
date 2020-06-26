using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using static Gathering;

namespace Tests.PlayModeTests
{
    public class MindBuilderTest
    {
        private GameObject gameUI;
        private UiController uiController;
        private GameObject baseContainer;
        private UnitController unitControl;
        private GameObject Gameworld;
        private GameObject MainCamera;

        [SetUp]
        public void Init()
        {
            Gameworld = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/UI/GameWorld"));
            MainCamera = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Main Camera"));
            gameUI = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/UI/IngameUI"));
            uiController = gameUI.GetComponent<UiController>();
            baseContainer = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/BaseBuilding/BaseContainer"));
            baseContainer.transform.position = new Vector3(100, 100, 100);
            unitControl = baseContainer.GetComponent<UnitController>();
            uiController.RegisterUnitController(unitControl);
        }

        [TearDown]
        public void Dispose()
        {
            Object.DestroyImmediate(gameUI);
            Object.DestroyImmediate(baseContainer);
            Object.DestroyImmediate(Gameworld);
            Object.DestroyImmediate(MainCamera);
        }

        [TestCase(ResourceType.Crystal, 2, Direction.East, true)]
        [TestCase(ResourceType.Rock, 2, Direction.East, true)]
        [TestCase(ResourceType.None, 5, Direction.South, false)]
        [TestCase(ResourceType.Rock, 4, Direction.West, true)]
        public void UpdateResourceMindCorrectly(ResourceType resType, int carryweight, Direction exploreDirection, bool isScout = false)
        {
            MindGroup mindGroup = unitControl.MindGroupList.GetMindGroupFromIndex(1);
            Gathering gather = new Gathering();
            for (int i = 0; i < mindGroup.GetMinds().Count; i++)
            {
                if (mindGroup.GetMinds()[i].GetType() == typeof(Gathering))
                {
                    gather = (Gathering)mindGroup.GetMinds()[i];
                }
            }
            uiController.UI_OpenMindBuilder(1);
            MindBuilderTabbed mbTabbed = uiController.GetComponentInChildren<MindBuilderTabbed>();
            mbTabbed.UpdateResourceValues(carryweight, isScout, resType, exploreDirection);
            mbTabbed.UpdateMind();

            Assert.True(MindEquals(gather, new Gathering(resType, carryweight, exploreDirection, isScout)));
        }

        [UnityTest]
        [TestCase(true, true, ExpectedResult = "hdafjd")]
        [TestCase(false, false, ExpectedResult = "hdafjd")]
        public IEnumerator GetsCorrectMind(bool atBase, bool updatedMinds)
        {
            bool valid = true;
            MindGroup mindGroup = unitControl.MindGroupList.GetMindGroupFromIndex(1);
            var Id = mindGroup.AddUnit(new UnitGroup(uiController.unitIconBase));
            GameObject gameObjectAnt = MonoBehaviour.Instantiate(Resources.Load("Prefabs/WorkerAnt") as GameObject);
            Ant ant = gameObjectAnt.GetComponent<Ant>();
            ant.SetunitGroupID(Id);
            ant.SetAtBase(atBase);
            yield return new WaitForSeconds(0.01f);
            if (ant.GetMinds().Count < 1)
            {
                valid = false;
            }
            if (valid)
            {
                for (int i = 0; i < mindGroup.GetMinds().Count; i++)
                {
                    if (!MindEquals(mindGroup.GetMinds()[i], ant.GetMinds()[i]))
                    {
                        valid = false;
                    }
                }
            }
            //put atbase at false so it does not try to access a disposed unitcontroller
            ant.SetAtBase(false);
            Assert.True(valid == updatedMinds);
        }

        public bool MindEquals(IMind mind1, IMind mind2)
        {
            if (mind1.GetType() == mind2.GetType())
            {
                bool returnvalue = true;
                if (mind1.GetType() == typeof(Gathering))
                {
                    Gathering gather = (Gathering)mind1;
                    Gathering gather2 = (Gathering)mind2;
                    if (!(gather.ExclusiveType == gather2.ExclusiveType &&
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
                    if (!(combat.AttackingQueen == combat2.AttackingQueen &&
                        combat.EngageRange == combat2.EngageRange))
                    {
                        returnvalue = false;
                    }
                }
                return returnvalue;
            }
            return false;
        }
    }
}