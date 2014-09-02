using UnityEngine;
using System.Collections;

public class ReceivesMazeEvents : MonoBehaviour
{
    public WaitFor<Vector3> waitForRoom = new WaitFor<Vector3>();
    public WaitFor<Vector3> waitForDeadEnd = new WaitFor<Vector3>();
    public WaitFor<Vector3> waitForHallway = new WaitFor<Vector3>();
}