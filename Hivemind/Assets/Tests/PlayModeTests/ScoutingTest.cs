using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.PlayModeTests
{
    public class ScoutingTest
    {
        private Ant ant;
        private UiController uiController;
        private UnitController unitControl;

        [UnitySetUp]
        public IEnumerator Init()
        {
            AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync("TestScene", LoadSceneMode.Single);
            while (!asyncLoadLevel.isDone)
            {
                yield return null;
            }
            uiController = GameObject.FindObjectOfType<UiController>();
            unitControl = GameObject.FindObjectOfType<UnitController>();

            GameObject gameObjectAnt = MonoBehaviour.Instantiate(Resources.Load("Prefabs/WorkerAnt") as GameObject);
            ant = gameObjectAnt.GetComponent<Ant>();
            yield return null;
            Gathering gather = new Gathering(ResourceType.Crystal, 1, Gathering.Direction.South, true)
            {
                state = Gathering.State.Scouting,
                busy = true
            };
            ant.SetMinds(new List<IMind>() { gather, new CombatMind() });
            foreach (IMind mind in ant.GetMinds())
            {
                mind.Initiate(ant);
            }
            MindGroup mindGroup = unitControl.MindGroupList.GetMindGroupFromIndex(1);
            mindGroup.SetMinds(new List<IMind>() { gather, new CombatMind() });
            var Id = mindGroup.AddUnit(new UnitGroup(uiController.unitIconBase));
            ant.SetunitGroupID(Id);
        }

        [UnityTest]
        public IEnumerator AntIsWanderingWithScoutingBehaviour()
        {
            ant.GetAgent().enabled = true;
            Vector3 initialPosition = new Vector3(ant.transform.position.x, 0, ant.transform.position.z);
            yield return new WaitForSeconds(0.1f);
            Assert.AreNotEqual(initialPosition, new Vector3(ant.transform.position.x, 0, ant.transform.position.z));
        }

        [UnityTest]
        public IEnumerator AntHasReturnedToBase()
        {
            ant.GetAgent().enabled = true;
            yield return new WaitForSeconds(41f);
            Gathering gather = (Gathering)ant.GetMinds()[0];
            Assert.AreNotEqual(Gathering.State.Scouting, gather.state);
        }

        [UnityTest]
        public IEnumerator AntFoundResource()
        {
            ant.GetAgent().enabled = true;
            GameObject resource = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Resources/crystal") as GameObject);
            resource.transform.position = new Vector3(0, 0.6f, 0);
            ant.SetAtBase(true);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(1, AmountOfKnownResources());
        }

        public int AmountOfKnownResources()
        {
            int amount = 0;
            foreach (ResourceNode resource in GameWorld.Instance.ResourceList)
            {
                if (resource.TeamIsKnown > 0)
                {
                    amount++;
                }
            }
            return amount;
        }
    }
}