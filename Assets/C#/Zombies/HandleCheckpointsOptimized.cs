using System.Collections.Generic;
using UnityEngine;

public class HandleCheckpointsOptimized : MonoBehaviour
{
    private GameObject _player;

    private List<GameObject> _level1Checkpoints = new List<GameObject>();
    private List<GameObject> _level2Checkpoints = new List<GameObject>();
    private List<GameObject> _level3Checkpoints = new List<GameObject>();

    private enum LayerLevel { Level1 = 6, Level2 = 7, Level3 = 8 }

    private GameObject _currentTarget = null;
    private int _intermediateLayer = -1;

    private bool _doneTransition;
    private bool TransitionInProgress => _currentTarget != null;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        foreach (var checkPoint in GameObject.FindGameObjectsWithTag("Checkpoint"))
        {
            switch (checkPoint.layer)
            {
                case (int)LayerLevel.Level1:
                    _level1Checkpoints.Add(checkPoint);
                    break;
                case (int)LayerLevel.Level2:
                    _level2Checkpoints.Add(checkPoint);
                    break;
                case (int)LayerLevel.Level3:
                    _level3Checkpoints.Add(checkPoint);
                    break;
            }
        }
    }

    private void Update()
    {
        if (_player == null) return;
        
        int playerLayer = _player.layer;
        int currentLayer = gameObject.layer;
        
        if (currentLayer == playerLayer)
        {
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, 2.5f * Time.deltaTime);
            Debug.Log("Same Layer");
            return;
        }

        // If doing double transitions, step-by-step
        if (_intermediateLayer != -1 && currentLayer != playerLayer)
        {
            PerformLayerChange(_intermediateLayer);
            return;
        }

        switch (playerLayer)
        {
            case (int)LayerLevel.Level1:
                if (currentLayer == (int)LayerLevel.Level2)
                    PerformLayerChange((int)LayerLevel.Level1);
                else if (currentLayer == (int)LayerLevel.Level3)
                    SetIntermediateLayer((int)LayerLevel.Level2);
                break;

            case (int)LayerLevel.Level2:
                if (currentLayer == (int)LayerLevel.Level1)
                    PerformLayerChange((int)LayerLevel.Level2);
                else if (currentLayer == (int)LayerLevel.Level3)
                    PerformLayerChange((int)LayerLevel.Level2);
                break;

            case (int)LayerLevel.Level3:
                if (currentLayer == (int)LayerLevel.Level2)
                    PerformLayerChange((int)LayerLevel.Level3);
                else if (currentLayer == (int)LayerLevel.Level1)
                    SetIntermediateLayer((int)LayerLevel.Level2);
                break;
        }
    }

    private void SetIntermediateLayer(int targetLayer)
    {
        _intermediateLayer = targetLayer;
        PerformLayerChange(_intermediateLayer);
    }

    private void PerformLayerChange(int targetLayer)
    {
        Debug.Log(targetLayer);
        List<GameObject> targetCheckpoints = GetCheckpointsForLayer(targetLayer);
        if (targetCheckpoints.Count == 0)
        {
            Debug.Log("Checkpoints not found");
            return;
        }

        if (_currentTarget == null)
        {
            _currentTarget = FindClosestCheckpoint(targetCheckpoints);
            Debug.Log("Checkpoint found: " + _currentTarget.name);
        }

        Debug.Log("Moving towards checkpoint");
        float step = 2f * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _currentTarget.transform.position, step);
        
        float distance = Vector3.Distance(transform.position, _currentTarget.transform.position);

        if (distance < 0.5f && !_doneTransition)
        {
            Debug.Log("Closes enough to link checkpoint");
            var cp = _currentTarget.GetComponent<CheckPointInfo>();
            if (cp != null)
            {
                Debug.Log("Changing current checkpoint");
                _currentTarget = cp.link.gameObject;
                _doneTransition = true;
            }
        }

        // Reached final linked point
        if (_currentTarget != null && Vector3.Distance(transform.position, _currentTarget.transform.position) < 0.25f)
        {
            Debug.Log("Reached link checkpoint");
            gameObject.layer = targetLayer;
            GetComponent<SpriteRenderer>().sortingLayerName = GetSortingLayer(targetLayer);
            _currentTarget = null;

            // Important: Reset
            _doneTransition = false;

            if (_intermediateLayer != -1)
            {
                // Step to final layer now
                _intermediateLayer = -1;
            }
        }
    }

    private List<GameObject> GetCheckpointsForLayer(int layer)
    {
        switch (layer)
        {
            case (int)LayerLevel.Level1: 
                return _level1Checkpoints;
            
            case (int)LayerLevel.Level2: 
                return _level2Checkpoints;
            
            case (int)LayerLevel.Level3: 
                return _level3Checkpoints;
            
            default: 
                return new List<GameObject>();
        }
    }

    private GameObject FindClosestCheckpoint(List<GameObject> checkPoints)
    {
        GameObject closest = null;
        float shortestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject obj in checkPoints)
        {
            float distance = Vector3.Distance(currentPosition, obj.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closest = obj;
            }
        }

        return closest;
    }

    private string GetSortingLayer(int layer)
    {
        switch (layer)
        {
            case 6:
                return "LEVEL1";
            case 7:
                return "LEVEL2";
            case 8:
                return "LEVEL3";
            default:
                return "";
        }
    }
    
    private void OnDrawGizmos()
    {
        if (_currentTarget == null) return;
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, _currentTarget.transform.position);
    }
}
