using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.PlayModeTests
{
    public class GatherResourceTest
    {
        private Ant ant;
        private GameObject gameUI;
        private UiController uiController;
        private UnitController unitControl;
        private GameObject resource;
        private ResourceNode resourceNode;

        [UnitySetUp]
        public IEnumerator Init()
        {
            AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync("TestScene", LoadSceneMode.Single);
            while (!asyncLoadLevel.isDone)
            {
                yield return null;
            }
            gameUI = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/UI/IngameUI"));
            uiController = gameUI.GetComponent<UiController>();
            unitControl = gameUI.GetComponent<UnitController>();

            GameObject gameObjectAnt = MonoBehaviour.Instantiate(Resources.Load("Prefabs/WorkerAnt") as GameObject);
            ant = gameObjectAnt.GetComponent<Ant>();
            Gathering gather = new Gathering(ResourceType.Crystal, 1, Gathering.Direction.South, true);
            gather.state = Gathering.State.Idle;
            ant.SetMinds(new List<IMind>() { gather, new CombatMind() });
            foreach (IMind mind in ant.GetMinds())
            {
                mind.Initiate(ant);
            }
            MindGroup mindGroup = unitControl.MindGroupList.GetMindGroupFromIndex(1);
            mindGroup.Minds = new List<IMind>() { gather, new CombatMind() };
            var Id = mindGroup.AddUnit(new UnitGroup(uiController.unitIconBase));
            ant.SetunitGroupID(Id);
            resource = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Resources/crystal") as GameObject);
            resource.transform.position = new Vector3(0, 0.6f, 0);
            resource.GetComponent<ResourceNode>().Discover(ant.TeamID);
            resourceNode = resource.GetComponent<ResourceNode>();
        }


        [UnityTest]
        public IEnumerator AntpicksUpResourceCorrectly()
        {
            ant.SetAtBase(false);
            ant.GetAgent().enabled = true;
            yield return new WaitForSeconds(0.1f);
            Gathering gather = (Gathering)ant.GetMinds()[0];
            Assert.AreEqual(1, gather.carryingObjects.Count);
        }

        [UnityTest]
        public IEnumerator ResourceHasCorrectAmount()
        {
            ant.SetAtBase(false);
            ant.GetAgent().enabled = true;
            yield return new WaitForSeconds(0.1f);
            Gathering gather = (Gathering)ant.GetMinds()[0];
            Assert.AreEqual(9, resourceNode.GetResources());
        }

        [UnityTest]
        public IEnumerator ResourceHasCorrectFutureAmount()
        {
            ant.SetAtBase(false);
            resource.transform.position = new Vector3(10, 0.6f, 10);
            ant.GetAgent().enabled = true;
            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual(9, resourceNode.GetResourcesFuture());
        }

        [UnityTest]
        public IEnumerator AntDeliversResource()
        {
            ant.GetAgent().enabled = true;
            ant.SetAtBase(true);
            yield return new WaitForSeconds(0.1f);
            Assert.LessOrEqual(1, ant.GetBaseController().GetGameResources().GetResourceAmount(ResourceType.Crystal));
        }
    }
}
