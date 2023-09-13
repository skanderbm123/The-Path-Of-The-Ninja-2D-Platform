using UnityEngine;
using Cinemachine;

public class ParallaxBackground : MonoBehaviour
{
    CinemachineVirtualCamera cam;
    float length;
    float camStartPos;
    float distance;

    [SerializeField] public float parallaxEffect;

    private void Start()
    {
        cam = FindFirstObjectByType<CinemachineVirtualCamera>();
        camStartPos = cam.transform.position.x;
        length = gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float distance = (cam.transform.position.x * parallaxEffect);

        transform.position = new Vector3(camStartPos + distance, transform.position.y, transform.position.z);

        if (temp > camStartPos + length)
        {
            camStartPos += length;
        }
        else if (temp < camStartPos - length)
        {
            camStartPos -= length;
        }

    }

}