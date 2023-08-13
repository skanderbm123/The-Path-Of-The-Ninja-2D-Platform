using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{

    Material mat;
    float distance;


    [SerializeField] private float speed;
    [SerializeField] private bool right;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        distance += Time.deltaTime * speed;
        if (right)
        {
            mat.SetTextureOffset("_MainTex", Vector2.right * distance);
        } else {
            mat.SetTextureOffset("_MainTex", -Vector2.right * distance);
        }
    }
}
