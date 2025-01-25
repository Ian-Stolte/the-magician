using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public string mode = "IDLE";

    private Vector3 moveTarget;
    private Vector3[] path;
    public bool pathReady;
    int waypointIndex;
    [SerializeField] private float speed;
    [SerializeField] private float turnSpeed;

    private Transform player;
    private Pathfinding pathfinding;

    void Start()
    {
        pathfinding = GameObject.Find("Pathfinding Grid").GetComponent<Pathfinding>();
        player = GameObject.Find("Player").transform;
        //RequestManager.RequestPath(transform.position, player.position, false, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool successful)
    {
        if (successful)
        {
            path = newPath;
            pathReady = true;
            if (path.Length > 0)
                transform.rotation = RotateToward(path[0], transform.position);
            waypointIndex = 0;
        }
    }

    void Update()
    {
        if (mode == "MOVE")
        {
            if (Vector3.Magnitude(player.position - moveTarget) > 0.5f)
            {
                moveTarget = player.position;
                StartCoroutine(pathfinding.FindPath(transform.position, player.position, false, OnPathFound, false));
            }
            if (pathReady)
            {
                FollowPath();
            }
        }
    }

    private void FollowPath()
    {
        if (path.Length > 0)
        {
            if (Vector2.Distance(transform.position, path[waypointIndex]) < 0.5f)
            {
                waypointIndex++;
            }
            if (waypointIndex >= path.Length)
            {
                Debug.Log("Reached player!!");
                //mode = "IDLE";
                pathReady = false;
            }
            else
            {
                Quaternion rotateDir = RotateToward(path[waypointIndex], transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotateDir, Time.deltaTime * turnSpeed * Mathf.Abs(rotateDir.eulerAngles.z - transform.rotation.eulerAngles.z)/90);
                transform.Translate(Vector3.up * Time.deltaTime * speed);
            }
        }
    }


    private Quaternion RotateToward(Vector3 target, Vector3 startingPos)
    {
        Vector3 dir = target - startingPos;
        dir = new Vector3(dir.x, dir.y, 0);
        dir = Vector3.Normalize(dir);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(new Vector3(0, 0, angle-90));
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = waypointIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one * 0.4f);

                if (i == waypointIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i-1], path[i]);
                }
            }
        }
        Grid gridScript = GameObject.Find("Pathfinding Grid").GetComponent<Grid>();
        if (gridScript.grid != null)
        {
            Node playerNode = gridScript.NodeFromWorldPoint(transform.position);
            foreach (Node n in gridScript.grid)
            {
                if (n == playerNode)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawCube(n.position, Vector3.one*(2*gridScript.nodeRadius - 0.1f));
                }
            }
        }
    }
}