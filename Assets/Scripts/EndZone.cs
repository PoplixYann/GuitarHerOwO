using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Note")
        {
            GameManager.Instance.RemoveNoteInList(other.gameObject);

            GameManager.Instance.AddScore(other.gameObject.GetComponent<Note>().type, -10);

            Destroy(other.gameObject);
        }
    }
}
