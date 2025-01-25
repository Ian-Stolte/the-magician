using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public string mode = "IDLE";
    [SerializeField] private Color idleColor;
    [SerializeField] private Color attackColor;

    private Vector3 moveTarget;
    private Vector3[] path;
    public bool pathReady;
    int waypointIndex;
    
    //Movement
    [SerializeField] private float speed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float followDist;

    //Attack
    private float attackTimer;
    [SerializeField] private float attackDelay;
    [SerializeField] private float attackDist;
    [SerializeField] private float attackChargeTime;
    [SerializeField] private GameObject bulletPrefab;

    private Transform player;
    private Pathfinding pathfinding;

    public bool lineOfSight;



    void Start()
    {
        pathfinding = GameObject.Find("Pathfinding Grid").GetComponent<Pathfinding>();
        player = GameObject.Find("Player").transform;
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
        if (mode != "IDLE")
        {
            Vector3 dir = player.position - transform.position;
            lineOfSight = !Physics2D.Raycast(transform.position, dir.normalized, dir.magnitude, LayerMask.GetMask("Obstacle"));

            //Attack
            attackTimer = Mathf.Max(0, attackTimer - Time.deltaTime);
            if (Vector3.Distance(player.position, transform.position) < attackDist && lineOfSight) //if in position to attack
            {
                if (attackTimer == 0 && mode == "MOVE")
                {
                    mode = "ATTACK";
                    StartCoroutine(Attack());
                }
            }
            //Move
            else if (mode == "MOVE" && Vector3.Distance(player.position, transform.position) < followDist)
            {
                if (Vector3.Distance(player.position, moveTarget) > 0.5f)
                {
                    moveTarget = player.position;
                    StartCoroutine(pathfinding.FindPath(transform.position, player.position, false, OnPathFound, false));
                    //RequestManager.RequestPath(transform.position, player.position, false, OnPathFound);
                }
                if (pathReady)
                {
                    FollowPath();
                }
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


    private IEnumerator Attack()
    {
        Vector3 bulletDir = Vector3.zero;
        transform.GetChild(1).gameObject.SetActive(true);
        for (float i = 0; i < attackChargeTime; i += 0.01f)
        {
            if (!lineOfSight || mode == "IDLE")
            {
                if (mode == "ATTACK")
                    mode = "MOVE";
                transform.GetChild(1).gameObject.SetActive(false);
                GetComponent<SpriteRenderer>().color = idleColor;
                yield break;
            }
            yield return new WaitForSeconds(0.01f);
            GetComponent<SpriteRenderer>().color = Color.Lerp(idleColor, attackColor, i/attackChargeTime);
            //Rotate gun
            bulletDir = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(bulletDir.y, bulletDir.x) * Mathf.Rad2Deg;
            float angleChange = (angle-90) - transform.GetChild(1).transform.rotation.eulerAngles.z;
            transform.GetChild(1).RotateAround(transform.position, new Vector3(0, 0, 1), angleChange);
        }
        GameObject bullet = Instantiate(bulletPrefab, transform.position + bulletDir, Quaternion.identity, GameObject.Find("Bullets").transform);
        bullet.GetComponent<Bullet>().direction = bulletDir;
        bullet.transform.rotation = RotateToward(player.position, transform.position);

        GetComponent<SpriteRenderer>().color = idleColor;
        attackTimer = attackDelay;
        mode = "MOVE";
        transform.GetChild(1).gameObject.SetActive(false);
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