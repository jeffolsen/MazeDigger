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

    public class DropData
    {
        public Transform prefab;
        public Vector3 position;

        public DropData(Transform prefab, Vector3 position)
        {
            this.prefab = prefab;
            this.position = position;
        }
    }

    public enum DropLocation { Anywhere, Hallway, DeadEnd }

    public DropLocation dropLocation;

    public List<DropTableItem> dropTable;

    List<DropData> dropHistory = new List<DropData>();

    void Start()
    {
        dropTable = dropTable.OrderBy(_ => _.rate).ToList();

        ReceivesMazeEvents mazeEvents = transform.Require<ReceivesMazeEvents>();
        WaitFor<Vector3> waitFor = mazeEvents.waitForRoom;
        WaitFor<Transform> waitForRepop = mazeEvents.waitForRepop;

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
                    dropHistory.Add(new DropData(drop.prefab, position));
                    Instantiate(drop.prefab, position, transform.rotation);
                    break;
                }
            }
        });

        waitForRepop.ThenAlways(prefab =>
        {
            foreach (DropData drop in dropHistory)
            {
                if (drop.prefab == prefab)
                {
                    Instantiate(drop.prefab, drop.position, transform.rotation);
                }
            }
        });
    }
}