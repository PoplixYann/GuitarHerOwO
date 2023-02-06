using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main_UI : MonoBehaviour
{
    [SerializeField] Text[] scoreTxt = new Text[4];
    [SerializeField] GameObject end;

    void Start()
    {
        GameManager.Instance.endEvent += UpdateEnd;
    }

    void Update()
    {
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreTxt[(int)NoteType.RED].text = $"Score Red : {GameManager.Instance.Score[(int)NoteType.RED]}";
        scoreTxt[(int)NoteType.YELLOW].text = $"Score Yellow : {GameManager.Instance.Score[(int)NoteType.YELLOW]}";
        scoreTxt[(int)NoteType.GREEN].text = $"Score Green : {GameManager.Instance.Score[(int)NoteType.GREEN]}";
        scoreTxt[(int)NoteType.BLUE].text = $"Score Blue : {GameManager.Instance.Score[(int)NoteType.BLUE]}";
    }

    void UpdateEnd()
    {
        if (end != null)
            end.SetActive(true);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void PlayGame()
    {
        GameManager.Instance.StopSong();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
