using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (transform.position.magnitude > 50f) // Arbitrary distance to destroy laser
        {
            Destroy(gameObject);
        }
    }
}
