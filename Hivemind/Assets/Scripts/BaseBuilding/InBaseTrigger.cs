﻿using UnityEngine;

public class InBaseTrigger : MonoBehaviour
{
    [SerializeField]
    private float inBaseAntScale = 0.1f;

    [SerializeField]
    private float inBaseMinimapScale = 50f;

    [SerializeField]
    private float outBaseAntScale = 0.1f;

    [SerializeField]
    private float outBaseMinimapScale = 150f;

    private void OnTriggerEnter(Collider other)
    {
        var ant = other.GetComponent<Ant>();
        if (ant != null)
        {
            ant.currentSpeed *= 1.8f;
            if (GetComponentInParent<BaseController>().TeamID == ant.TeamID)
            {
                ant.isAtBase = true;
                ant.currentSpeed *= 2;
            }
            ant.ChangeScale(inBaseAntScale, inBaseMinimapScale);
            ant.UpdateSpeed();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var ant = other.GetComponent<Ant>();
        if (ant != null)
        {
            if (GetComponentInParent<BaseController>().TeamID == ant.TeamID)
            {
                ant.isAtBase = false;
            }
            ant.ChangeScale(outBaseAntScale, outBaseMinimapScale);
            ant.currentSpeed = ant.baseSpeed;
            ant.UpdateSpeed();
        }
    }
}