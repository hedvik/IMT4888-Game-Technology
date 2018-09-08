using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSpawner : MonoBehaviour {
    public int _numAgents;
    public GameObject _spawnAreaObject;
    public GameObject _agentPrefab;
    public Transform _goal;

    private void Start()
    {
        var spawnAreaCollider = _spawnAreaObject.GetComponent<Collider>();

        for(int i = 0; i < _numAgents; i++)
        {
            var randomX = Random.Range(spawnAreaCollider.bounds.min.x, spawnAreaCollider.bounds.max.x);
            var randomZ = Random.Range(spawnAreaCollider.bounds.min.z, spawnAreaCollider.bounds.max.z);
            var newAgent = Instantiate(_agentPrefab, new Vector3(randomX, 0.5f, randomZ), Quaternion.identity);
            newAgent.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = _goal.position;
        }
    }
}
