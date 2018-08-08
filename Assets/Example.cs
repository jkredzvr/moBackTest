using UnityEngine;

public class Example : MonoBehaviour
{
    public float speed = 75.0f;

    void Update()
    {
        Vector3 v3 =  new Vector3(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), 0.0f);
        transform.Rotate (v3 * speed * Time.deltaTime, Space.World);
    }
}