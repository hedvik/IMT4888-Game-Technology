using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[System.Serializable]
public class EcsAgentSpawnerComponent : MonoBehaviour
{
    public int _numberOfAgents;
    public GameObject _spawnArea;
    public Transform _goal;
    public float movementSpeed;
}