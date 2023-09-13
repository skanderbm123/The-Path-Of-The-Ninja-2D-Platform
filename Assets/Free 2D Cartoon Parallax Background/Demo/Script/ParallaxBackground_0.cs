using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground_0 : MonoBehaviour
{
    public bool Camera_Move;
    public float Camera_MoveSpeed = 1.5f;
    [Header("Layer Setting")]
    public float[] Layer_Speed = new float[7];
    public GameObject[] Layer_Objects = new GameObject[7];

    private Transform _camera;
    private float[] startPosX = new float[7];
    private float[] startY = new float[7]; // Array to store initial Y positions
    private float boundSizeX;
    private float sizeX;

    void Start()
    {
        _camera = Camera.main.transform;
        sizeX = Layer_Objects[0].transform.localScale.x;
        boundSizeX = Layer_Objects[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x;

        // Store the initial positions for all layers
        for (int i = 0; i < Layer_Objects.Length; i++)
        {
            startPosX[i] = Layer_Objects[i].transform.position.x;
            startY[i] = Layer_Objects[i].transform.position.y;
        }
    }

    void Update()
    {
        // Moving camera
        if (Camera_Move)
        {
            _camera.position += Vector3.right * Time.deltaTime * Camera_MoveSpeed;
        }

        for (int i = 0; i < Layer_Objects.Length; i++)
        {
            float tempX = (_camera.position.x * (1 - Layer_Speed[i]));
            float distanceX = _camera.position.x * Layer_Speed[i];

            // Use the stored initial Y position for each layer
            Layer_Objects[i].transform.position = new Vector2(startPosX[i] + distanceX, startY[i]);

            if (tempX > startPosX[i] + boundSizeX * sizeX)
            {
                startPosX[i] += boundSizeX * sizeX;
            }
            else if (tempX < startPosX[i] - boundSizeX * sizeX)
            {
                startPosX[i] -= boundSizeX * sizeX;
            }
        }
    }
}
