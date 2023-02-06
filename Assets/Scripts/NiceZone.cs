using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NiceZone : MonoBehaviour
{
    [SerializeField] NoteType type;
    [SerializeField] GameObject lum;

    private void Update()
    {
        if (!lum.activeSelf && GameManager.Instance.KeyPressedList[(int)type])
        {
            lum.SetActive(true);
        }
        else if (lum.activeSelf && !GameManager.Instance.KeyPressedList[(int)type])
        {
            lum.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Note")
        {
            GameManager.Instance.AddNoteNiceZone(other.gameObject, type);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Note")
        {
            GameManager.Instance.RemoveNoteNiceZone(type);
        }
    }
}
