using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeSystem : MonoBehaviour
{
    // Array of Nodes
    [SerializeField] private Transform[] endNodes;

    // Get a random end node
    public Transform getEndNode()
    {
        int randomNodeIndicator = Random.Range(0, endNodes.Length);
        return endNodes[randomNodeIndicator];
    }

    // Get a random end node excluding the current node specified
    public Transform getNextEndNode(Transform currentNode)
    {
        // Make a new array of nodes without the current node
        Transform[] endNodeList = new Transform[endNodes.Length - 1];
        int nodeListIndicator = 0;
        for (int i = 0; i < endNodes.Length; i++)
        {
            if (!endNodes[i].position.Equals(currentNode.position))
            {
                endNodeList[nodeListIndicator] = endNodes[i];
                nodeListIndicator++;
            }
        }

        // Now get the random end node
        int randomNodeIndicator = Random.Range(0, endNodeList.Length);
        return endNodeList[randomNodeIndicator];
    }
}
