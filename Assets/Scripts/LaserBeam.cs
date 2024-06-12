using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        // Move the laser in a specified direction
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.ShowLosePanel();
        }
    }
}
