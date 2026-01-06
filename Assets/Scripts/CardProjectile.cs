using UnityEngine;

public class CardProjectile : Bullet
{
    private Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        float scaleFactor = Mathf.Lerp(1f, 0.4f, distTraveled/maxDist);
        transform.localScale = startScale * scaleFactor;
    }

    override protected void BubbleCollision(GameObject bubble)
    {
        Destroy(bubble);
        Destroy(gameObject);
    }
}