using UnityEngine;
using Cinemachine;
using System.ComponentModel;
using DG.Tweening; // Import the DOTween namespace

public class CameraSwitcher : MonoBehaviour
{
    public GameObject virtualCamera;
    public GameObject mainVirtualCamera;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            virtualCamera.SetActive(true);
            mainVirtualCamera.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            virtualCamera.SetActive(true);
            mainVirtualCamera.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            virtualCamera.SetActive(false);
            //mainVirtualCamera.SetActive(true);
        }
    }
}
