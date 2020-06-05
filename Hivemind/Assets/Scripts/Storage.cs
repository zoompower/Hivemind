﻿using UnityEngine;

public class Storage : MonoBehaviour
{
    private void Awake()
    {
        GameWorld.Instance.SetStorage(this);
    }

    public Vector3 GetPosition()
    {
        return gameObject.transform.position;
    }
}