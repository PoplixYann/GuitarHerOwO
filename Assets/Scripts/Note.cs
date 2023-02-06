using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    float speed;
    public NoteType type = NoteType.RED;

    void Start()
    {
        speed = GameManager.Instance.NoteSpeed;
    }

    void Update()
    {
        transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime));
    }
}
