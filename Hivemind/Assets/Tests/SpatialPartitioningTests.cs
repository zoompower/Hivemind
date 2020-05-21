using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class SpatialPartitioningTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void SpatialPartitioningCheckIf00Reachable()
        {
            GameObject Floor = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Floor (With Collision boxes)"));

            var Testing = Floor.transform.Find("CollisionBox(0,0)");

            SpatialPartitioning NeighCollider = Testing.GetComponent<SpatialPartitioning>();

            Assert.True(NeighCollider.height == 0);
            Assert.True(NeighCollider.width == 0);
            // Use the Assert class to test conditions
        }

        [Test]
        public void SpatialPartitioningCheckIfCanSelect2EntitiesSelfAndNeigbors()
        {
            GameObject Floor = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Floor (With Collision boxes)"));

            var GoFrom = Floor.transform.Find("CollisionBox(3,3)");
            var GoTo = Floor.transform.Find("CollisionBox(3,5)");

            SpatialPartitioning GoFromSelected = GoFrom.GetComponent<SpatialPartitioning>();
            SpatialPartitioning GoToSelected = GoTo.GetComponent<SpatialPartitioning>();

            GoFromSelected.Entities.Add(new GameObject());
            GoToSelected.Entities.Add(new GameObject());

            List<GameObject> Entities;

            Entities = GoFromSelected.GetEntitiesWithExtraNeighbors(1);
            Assert.True(Entities.Count == 1);

            Entities = GoFromSelected.GetEntitiesWithExtraNeighbors(2);
            Assert.True(Entities.Count == 2);
        }

        [Test]
        public void SpatialPartitioningCheckIfCanSelect3EntitiesSelfAndNeigbors()
        {
            GameObject Floor = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Floor (With Collision boxes)"));

            var GoFrom = Floor.transform.Find("CollisionBox(3,3)");
            var GoTo = Floor.transform.Find("CollisionBox(3,5)");

            SpatialPartitioning GoFromSelected = GoFrom.GetComponent<SpatialPartitioning>();
            SpatialPartitioning GoToSelected = GoTo.GetComponent<SpatialPartitioning>();

            GoFromSelected.Entities.Add(new GameObject());
            GoToSelected.Entities.Add(new GameObject());
            GoToSelected.Entities.Add(new GameObject());

            List<GameObject> Entities;

            Entities = GoFromSelected.GetEntitiesWithExtraNeighbors(1);
            Assert.True(Entities.Count == 1);

            Entities = GoFromSelected.GetEntitiesWithExtraNeighbors(2);
            Assert.True(Entities.Count == 3);
        }

        [Test]
        public void SpatialPartitioningCheckIfOutOfRangeDoesNotGetSelected()
        {
            GameObject Floor = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Floor (With Collision boxes)"));

            var GoFrom = Floor.transform.Find("CollisionBox(3,3)");
            var GoTo = Floor.transform.Find("CollisionBox(3,5)");
            var DontGoTo = Floor.transform.Find("CollisionBox(3,7)");

            SpatialPartitioning GoFromSelected = GoFrom.GetComponent<SpatialPartitioning>();
            SpatialPartitioning GoToSelected = GoTo.GetComponent<SpatialPartitioning>();
            SpatialPartitioning DontGoToSelected = DontGoTo.GetComponent<SpatialPartitioning>();

            GoFromSelected.Entities.Add(new GameObject());
            GoToSelected.Entities.Add(new GameObject());
            DontGoToSelected.Entities.Add(new GameObject());

            List<GameObject> Entities;

            Entities = GoFromSelected.GetEntitiesWithExtraNeighbors(1);
            Assert.True(Entities.Count == 1);

            Entities = GoFromSelected.GetEntitiesWithExtraNeighbors(2);
            Assert.True(Entities.Count == 2);
        }
    }
}
