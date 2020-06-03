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
            SceneManager.LoadScene("TestScene");
            yield return new WaitForSeconds(1f);
            gameUI = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/UI/IngameUI"));
            uiController = gameUI.GetComponent<UiController>();
            unitControl = gameUI.GetComponent<UnitController>();

            GameObject gameObjectAnt = MonoBehaviour.Instantiate(Resources.Load("Prefabs/WorkerAnt") as GameObject);
            ant = gameObjectAnt.GetComponent<Ant>();
            Gathering gather = new Gathering(ResourceType.Crystal, 1, Gathering.Direction.South, true);
            gather.state = Gathering.State.Idle;
            ant.minds = new List<IMind>() { gather, new CombatMind() };
            foreach (IMind mind in ant.minds)
            {
                mind.Initiate(ant);
            }
            MindGroup mindGroup = unitControl.MindGroupList.GetMindGroupFromIndex(1);
            mindGroup.Minds = new List<IMind>() { gather, new CombatMind() };
            var Id = mindGroup.AddUnit(new UnitGroup(uiController.unitIconBase));
            ant.SetunitGroupID(Id);
            resource = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Resources/crystal") as GameObject);
            resource.transform.position = new Vector3(0, 0.6f, 0);
            resource.GetComponent<ResourceNode>().AddToKnownResourceList();
            resourceNode = resource.GetComponent<ResourceNode>();
        }


        [UnityTest]
        public IEnumerator AntpicksUpResourceCorrectly()
        {
            ant.GetAgent().enabled = true;
            yield return new WaitForSeconds(0.1f);
            Gathering gather = (Gathering)ant.minds[0];
            Assert.AreEqual(1, gather.carryingObjects.Count);
        }

        [UnityTest]
        public IEnumerator ResourceHasCorrectAmount()
        {
            ant.GetAgent().enabled = true;
            yield return new WaitForSeconds(0.1f);
            Gathering gather = (Gathering)ant.minds[0];
            Assert.AreEqual(9, resourceNode.GetResources());
        }

        [UnityTest]
        public IEnumerator ResourceHasCorrectFutureAmount()
        {
            resource.transform.position = new Vector3(10, 0.6f, 10);
            ant.GetAgent().enabled = true;
            yield return new WaitForSeconds(0.1f);
            Gathering gather = (Gathering)ant.minds[0];
            Assert.AreEqual(9, resourceNode.GetResourcesFuture());
        }

        [UnityTest]
        public IEnumerator AntDeliversResource()
        {
            ant.GetAgent().enabled = true;
            ant.isAtBase = true;
            yield return new WaitForSeconds(0.05f);
            Assert.LessOrEqual(1, GameResources.GetResourceAmount(ResourceType.Crystal));
        }
    }
}
