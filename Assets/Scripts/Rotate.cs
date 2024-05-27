using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 direction;
    public float speed;

    private void Update()
    {
        transform.Rotate(direction * speed * Time.deltaTime);
    }
}