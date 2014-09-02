using UnityEngine;
using System.Collections.Generic;
using Require;
using System.Linq;

public class PlacesPropsInRooms : MonoBehaviour
{
    [System.Serializable]
    public class DropTableItem
    {
        public float rate;
        public Transform prefab;
    }

    public enum DropLocation { Anywhere, Hallway, DeadEnd }

    public DropLocation dropLocation;

    public List<DropTableItem> dropTable;

    void Start()
    {
        dropTable = dropTable.OrderBy(_ => _.rate).ToList();

        ReceivesMazeEvents mazeEvents = transform.Require<ReceivesMazeEvents>();
        WaitFor<Vector3> waitFor = mazeEvents.waitForRoom;

        if (dropLocation == DropLocation.Hallway)
        {
            waitFor = mazeEvents.waitForHallway;
        }
        else if (dropLocation == DropLocation.DeadEnd)
        {
            waitFor = mazeEvents.waitForDeadEnd;
        }

        waitFor.ThenAlways(position =>
        {
            foreach (DropTableItem drop in dropTable)
            {
                if (Random.Range(0.0f, 1.0f) < drop.rate)
                {
                    (Instantiate(drop.prefab, position, transform.rotation) as Transform).parent = transform;
                    break;
                }
            }
        });
    }
}