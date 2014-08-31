using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Require;
using Listless;

public class DigsAMaze : MonoBehaviour
{
    public LayerMask wallLayers;
    public Transform floorPrefab;
    public Transform deadEndPrefab;
    public List<Vector3> directions;

    int floorLayer;
    Stack<Vector3> trail = new Stack<Vector3>();
    bool backtracking;

    void Start()
    {
        floorLayer = 1 << floorPrefab.gameObject.layer;
        (Instantiate(floorPrefab, transform.position, Quaternion.identity) as Transform).parent = transform;
        trail.Push(transform.position);

        while (trail.Count > 0)
        {
            Vector3 cursor = trail.Peek();
            List<Vector3> possibleDirections = directions.Where(direction => !Dug(cursor + direction)).ToList();

            if (possibleDirections.Count > 0)
            {
                Dig(cursor, possibleDirections.Random());
                backtracking = false;
            }
            else
            {
                if (!backtracking && deadEndPrefab != null)
                {
                    (Instantiate(deadEndPrefab, cursor, Quaternion.identity) as Transform).parent = transform;
                }

                backtracking = true;
                trail.Pop();
            }
        }
    }

    void Dig(Vector3 start, Vector3 direction)
    {
        foreach (RaycastHit hit in Physics.RaycastAll(start, direction, direction.magnitude, wallLayers))
        {
            Destroy(hit.collider.gameObject);
        }

        (Instantiate(floorPrefab, start + direction, Quaternion.identity) as Transform).parent = transform;
        trail.Push(start + direction);
    }

    bool Dug(Vector3 position)
    {
        return Physics.CheckSphere(position, 0.1f, floorLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "maze", true);
    }
}