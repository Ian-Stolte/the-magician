using UnityEngine;

public class CardProjectile : MonoBehaviour
{
    public Vector3 direction;
    [SerializeField] float speed;
    
    [SerializeField] float dist;
    private float currentDist;

    private Vector3 startScale;


    void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        transform.position += direction.normalized * speed * Time.deltaTime;
        //dist -= speed * Time.deltaTime;
        currentDist += speed * Time.deltaTime;

        float scaleFactor = Mathf.Lerp(1f, 0.4f, currentDist/dist);
        transform.localScale = startScale * scaleFactor;

        if (currentDist > dist)
            Destroy(gameObject);
    }
}