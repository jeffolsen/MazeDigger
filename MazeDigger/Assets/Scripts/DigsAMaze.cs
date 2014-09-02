using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Require;
using Listless;
using System;
using Tap;

public class DigsAMaze : MonoBehaviour
{
    public LayerMask wallLayers;
    public List<Transform> directionMarkers;

    Stack<Vector3> trail = new Stack<Vector3>();
    bool backtracking;

    HashSet<GameObject> alreadyDugMarkers = new HashSet<GameObject>();

    void Start()
    {
        List<Vector3> directions = directionMarkers.Select(_ => _.position - transform.position).ToList();
        trail.Push(transform.position);

        while (trail.Count > 0)
        {
            Vector3 cursor = trail.Peek();
            List<Vector3> possibleDirections = directions.Where(direction => !Dug(cursor + direction)).ToList();

            if (possibleDirections.Count > 0)
            {
                if (!backtracking)
                {
                    EachZone(cursor, _ => _.waitForHallway.Happened(cursor));
                }

                Dig(cursor, possibleDirections.Random());
                backtracking = false;
            }
            else
            {
                if (!backtracking)
                {
                    EachZone(cursor, _ => _.waitForDeadEnd.Happened(cursor));
                    // (Instantiate(deadEndPrefab, cursor, transform.rotation) as Transform).parent = transform;
                }

                backtracking = true;
                trail.Pop();
            }
        }

        foreach (GameObject marker in alreadyDugMarkers)
        {
            Destroy(marker.gameObject);
        }
    }

    void Dig(Vector3 start, Vector3 direction)
    {
        foreach (RaycastHit hit in Physics.RaycastAll(start, direction, direction.magnitude, wallLayers))
        {
            Destroy(hit.collider.gameObject);
        }

        // if (UnityEngine.Random.Range(0.0f, 1.0f) < dropRate)
        // {
        //     (Instantiate(randomPropPrefab, start + direction, transform.rotation) as Transform).parent = transform;
        // }

        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        alreadyDugMarkers.Add(marker);
        marker.transform.position = start + direction;
        marker.transform.localScale = Vector3.one * direction.magnitude / 3.0f;
        marker.AddComponent<AlreadyDug>();

        EachZone(start + direction, _ => _.waitForRoom.Happened(start + direction));

        // (Instantiate(floorPrefab, start + direction, transform.rotation) as Transform).parent = transform;

        trail.Push(start + direction);
    }

    bool Dug(Vector3 position)
    {
        foreach (Collider collider in Physics.OverlapSphere(position, 0.1f))
        {
            if (collider.GetComponent<AlreadyDug>() != null)
            {
                return true;
            }
        }
        return false;
    }

    void EachZone(Vector3 position, Action<ReceivesMazeEvents> act)
    {
        foreach (Collider collider in Physics.OverlapSphere(position, 0.1f))
        {
            collider.GetComponent<ReceivesMazeEvents>().AndAnd(_ => act.Invoke(_));
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "packages/jeffolsen/MazeDigger/maze", true);

        foreach (Transform marker in directionMarkers)
        {
            Gizmos.DrawLine(transform.position, marker.position);
            Gizmos.DrawCube(marker.position, Vector3.one * 0.1f);
        }
    }
}