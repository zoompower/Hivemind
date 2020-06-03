﻿using System;
using UnityEngine;

public class CombatMind : IMind
{
    private float minEstimatedDifference;
    private int prefferedHealth;
    private Ant ant;
    private bool busy;

    public int EngageRange;

    public CombatMind() : this(0, 0) { }

    public CombatMind(float minEstimated, int prefHealth)
    {
        minEstimatedDifference = minEstimated;
        prefferedHealth = prefHealth;
    }

    public IMind Clone()
    {
        return new CombatMind(minEstimatedDifference, prefferedHealth);
    }

    public void Execute()
    {
        if (this == null) new CombatFight().Execute(ant);

        var healthDifference = ant.health / (float) ant.closestEnemy.health;
        var damageDifference = ant.damage / (float) ant.closestEnemy.damage;
        var strengthDifference = (healthDifference * 1 + damageDifference * 2) / 3;

        if (strengthDifference >= minEstimatedDifference)
            new CombatFight().Execute(ant);
        else
            new CombatFlee().Execute(ant);
    }

    public void GenerateUI()
    {
        throw new NotImplementedException();
    }

    public void Initiate(Ant ant)
    {
        this.ant = ant;
    }

    public double Likelihood()
    {
        var NearbyEntitities = ant.SpatialPosition.GetEntitiesWithNeigbors();
        busy = false;
        foreach (GameObject a in NearbyEntitities)
        {
            if (a.GetComponent<Ant>())
            {
                if(a.GetComponent<Ant>().GetStorage() != this.ant.GetStorage())
                {
                    if(Vector3.Distance(ant.transform.position, a.transform.position) < Vector3.Distance(ant.transform.position, ant.closestEnemy.transform.position))
                    {
                        this.ant.SetClosestEnemy(a.GetComponent<Ant>());
                    }
                    busy = true;
                }
            }
        }
        if (busy)
        {
            return 100;
        }
        else
        {
            return 0;
        }
    }

    public float GetMinEstimatedDifference()
    {
        return minEstimatedDifference;
    }

    public void SetMinEstimatedDifference(float estDiff)
    {
        minEstimatedDifference = estDiff;
    }

    public int GetPrefferedHealth()
    {
        return prefferedHealth;
    }

    public void SetPrefferedHealth(int prefHealth)
    {
        prefferedHealth = prefHealth;
    }

    public bool IsBusy()
    {
        return busy;
    }
}