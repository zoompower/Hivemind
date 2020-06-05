using System.Collections;
using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using System.Linq;

namespace Tests.PlayModeTests
{
    public class BaseBuildingTest
    {
        private GameObject gameUI;
        private UnitController unitControl;

        [UnitySetUp]
        public IEnumerator Init()
        {
            AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync("BaseBuildingTestScene", LoadSceneMode.Single);
            while (!asyncLoadLevel.isDone)
            {
                yield return null;
            }
            gameUI = GameObject.FindObjectsOfType<GameObject>().FirstOrDefault(x => x.name == "IngameUI");
            unitControl = gameUI.GetComponent<UnitController>();
        }

        [TearDown]
        public void Dispose()
        {
            GameObject.Destroy(gameUI);
        }

        [UnityTest]
        [TestCase(new string[] { "BasePlate (41)" }, 5, ExpectedResult = "waitforseconds")]
        [TestCase(new string[] { "BasePlate (24)" }, 4, ExpectedResult = "waitforseconds")]
        [TestCase(new string[] { "BasePlate (41)", "BasePlate (27)" }, 6, ExpectedResult = "waitforseconds")]
        [TestCase(new string[] { "BasePlate (41)", "BasePlate (40)" }, 6, ExpectedResult = "waitforseconds")]
        [TestCase(new string[] { "BasePlate (40)", "BasePlate (27)" }, 5, ExpectedResult = "waitforseconds")]

        public IEnumerator MergeAntRooms(string[] baseTileNames, int expectedMaxUnits)
        {
            bool valid = false;
            List<BaseTile> baseTiles = GameObject.FindObjectsOfType<BaseTile>().ToList();
            baseTiles = GetBaseTilesFromNames(baseTiles, baseTileNames.ToList());
            foreach (BaseTile baseTile in baseTiles)
            {
                baseTile.AntDoesAction(BaseBuildingTool.AntRoom);
            }

            yield return new WaitForSeconds(1.3f);
            foreach (MindGroup mindGroup in unitControl.MindGroupList.mindGroupList)
            {
                foreach (UnitGroup unitGroup in mindGroup.unitGroupList)
                {
                    if (unitGroup.MaxUnits >= expectedMaxUnits)
                    {
                        valid = true;
                    }
                }
            }
            Assert.IsTrue(valid);
        }

        [UnityTest]
        [TestCase(new string[] { "BasePlate (24)" }, 3, ExpectedResult = "waitforseconds")]
        [TestCase(new string[] { "BasePlate (26)" }, 1, ExpectedResult = "waitforseconds")]
        [TestCase(new string[] { "BasePlate (26)", "BasePlate (24)" }, 2, ExpectedResult = "waitforseconds")]
        public IEnumerator SplitAntRooms(string[] baseTileNames, int expectedUnitGroups)
        {
            int uniqueUnitGroups = 0;
            List<BaseTile> baseTiles = GameObject.FindObjectsOfType<BaseTile>().ToList();
            baseTiles = GetBaseTilesFromNames(baseTiles, baseTileNames.ToList());

            yield return new WaitForSeconds(1.3f);
            foreach (BaseTile baseTile in baseTiles)
            {
                baseTile.AntDoesAction(BaseBuildingTool.DestroyRoom);
            }

            foreach (MindGroup mindGroup in unitControl.MindGroupList.mindGroupList)
            {
                foreach (UnitGroup unitGroup in mindGroup.unitGroupList)
                {
                    uniqueUnitGroups++;
                }
            }
            Assert.AreEqual(expectedUnitGroups, uniqueUnitGroups);
        }

        private List<BaseTile> GetBaseTilesFromNames(List<BaseTile> objects, List<String> names)
        {
            List<BaseTile> returnObjects = new List<BaseTile>();
            foreach (string name in names)
            {
                returnObjects.AddRange(objects.Where(x => x.name == name));
            }
            return returnObjects;
        }
    }
}
