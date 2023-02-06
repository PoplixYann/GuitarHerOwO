using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public enum NoteType
{
    RED,
    YELLOW,
    GREEN,
    BLUE
}

public class GameManager : MonoBehaviour
{
    #region singleton

    static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("GameManager");
                instance = go.AddComponent<GameManager>();
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    #endregion

    [Header("Song")]
    [Space]
    [SerializeField] AK.Wwise.Event songEvent;
    float songTime = 0.0f;
    float songTimeMax;
    bool songIsStart = false;
    float songStartTime;

    [Header("Note")]
    [Space]
    [SerializeField] float noteSpeed = 2.0f;
    public float NoteSpeed { get => noteSpeed; }

    GameObject[] notePrefabList = new GameObject[4];
    Transform[] spawnList = new Transform[4];
    Transform[] niceZoneList = new Transform[4];
    List<GameObject>[] noteList = new List<GameObject>[4];
    GameObject[] noteNiceZoneList = new GameObject[4];

    [Header("Note Red")]
    [Space]
    [SerializeField] GameObject noteRed;
    [SerializeField] Transform spawnRed;
    [SerializeField] Transform niceZoneRed;

    [Header("Note Yellow")]
    [Space]
    [SerializeField] GameObject noteYellow;
    [SerializeField] Transform spawnYellow;
    [SerializeField] Transform niceZoneYellow;

    [Header("Note Green")]
    [Space]
    [SerializeField] GameObject noteGreen;
    [SerializeField] Transform spawnGreen;
    [SerializeField] Transform niceZoneGreen;

    [Header("Note Blue")]
    [Space]
    [SerializeField] GameObject noteBlue;
    [SerializeField] Transform spawnBlue;
    [SerializeField] Transform niceZoneBlue;

    [Header("Nice Zone")]
    [Space]
    [SerializeField] GameObject niceZoneEffect;

    [Header("Post Process")]
    [Space]
    [SerializeField] Volume pp;
    Vignette vignettePP;

    bool[] keyPressedList = new bool[4];
    public bool[] KeyPressedList { get => keyPressedList; }

    float spawnTimer = 0.5f;

    int[] score = new int[4];
    public int[] Score { get => score; }

    float pisteLength;

    bool isEnd = false;

    public delegate void EndDelegate();
    public event EndDelegate endEvent;

    void Start()
    {
        pp.profile.TryGet<Vignette>(out vignettePP);

        for (int i = 0; i < noteList.Length; i++)
        {
            noteList[i] = new List<GameObject>();
        }

        notePrefabList[(int)NoteType.RED] = noteRed;
        spawnList[(int)NoteType.RED] = spawnRed;
        niceZoneList[(int)NoteType.RED] = niceZoneRed;
        notePrefabList[(int)NoteType.YELLOW] = noteYellow;
        spawnList[(int)NoteType.YELLOW] = spawnYellow;
        niceZoneList[(int)NoteType.YELLOW] = niceZoneYellow;
        notePrefabList[(int)NoteType.GREEN] = noteGreen;
        spawnList[(int)NoteType.GREEN] = spawnGreen;
        niceZoneList[(int)NoteType.GREEN] = niceZoneGreen;
        notePrefabList[(int)NoteType.BLUE] = noteBlue;
        spawnList[(int)NoteType.BLUE] = spawnBlue;
        niceZoneList[(int)NoteType.BLUE] = niceZoneBlue;

        for (int i = 0; i < score.Length; i++)
        {
            score[i] = 900;
        }

        pisteLength = Mathf.Abs(spawnList[(int)NoteType.YELLOW].transform.position.z - niceZoneList[(int)NoteType.YELLOW].position.z);
        songStartTime = (pisteLength / noteSpeed) + 0.3f;

        Invoke("SpawnNote", spawnTimer);
        Invoke("StartSong", songStartTime);
    }

    void Update()
    {
        if (!isEnd)
        {
            PlayerInputDown();
            PlayerInputUp();

            CountCurSongTime();

            ChangePPVignette();
        }
    }

    void PlayerInputDown()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (songIsStart)
            {
                CheckNoteNiceZone(NoteType.RED);
            }
            keyPressedList[(int)NoteType.RED] = true;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (songIsStart)
            {
                CheckNoteNiceZone(NoteType.YELLOW);
            }
            keyPressedList[(int)NoteType.YELLOW] = true;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (songIsStart)
            {
                CheckNoteNiceZone(NoteType.GREEN);
            }
            keyPressedList[(int)NoteType.GREEN] = true;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (songIsStart)
            {
                CheckNoteNiceZone(NoteType.BLUE);
            }
            keyPressedList[(int)NoteType.BLUE] = true;
        }
    }

    void PlayerInputUp()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            keyPressedList[(int)NoteType.RED] = false;
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            keyPressedList[(int)NoteType.YELLOW] = false;
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            keyPressedList[(int)NoteType.GREEN] = false;
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            keyPressedList[(int)NoteType.BLUE] = false;
        }
    }

    void CheckNoteNiceZone(NoteType type)
    {
        if (noteNiceZoneList[(int)type] == null && noteList[(int)type].Count > 0)
        {
            GameObject closestNote = null;
            float maxDistance = 0.0f;
            foreach (GameObject note in noteList[(int)type])
            {
                float distance = Vector3.Distance(spawnList[(int)type].transform.position, note.transform.position);
                if (distance > maxDistance)
                {
                    closestNote = note;
                    maxDistance = distance;
                }
            }

            if (closestNote != null)
            {
                RemoveNoteInList(closestNote);
                Destroy(closestNote);
            }

            AddScore(type, -10);
        }
        else if (noteNiceZoneList[(int)type] != null)
        {
            GameObject note = noteNiceZoneList[(int)type];
            RemoveNoteInList(note);
            RemoveNoteNiceZone(type);
            Destroy(note);

            SpawnNiceZoneEffect(type);

            AddScore(type, 10);
        }
        else
        {
            AddScore(type, -10);
        }
    }

    void SpawnNiceZoneEffect(NoteType type)
    {
        Instantiate(niceZoneEffect, niceZoneList[(int)type].position, Quaternion.identity);
    }

    void SpawnNote()
    {
        if (!songIsStart || songTime < songTimeMax - (pisteLength / noteSpeed))
        {
            NoteType type = (NoteType)Random.Range(0, 4);
            GameObject note = Instantiate(notePrefabList[(int)type], spawnList[(int)type].transform.position, Quaternion.identity);
            note.GetComponent<Note>().type = type;

            AddNoteInList(note);
        }

        Invoke("SpawnNote", spawnTimer);
    }

    void AddNoteInList(GameObject note)
    {
        noteList[(int)note.GetComponent<Note>().type].Add(note);
    }

    public void RemoveNoteInList(GameObject note)
    {
        noteList[(int)note.GetComponent<Note>().type].Remove(note);
    }

    public void AddNoteNiceZone(GameObject note, NoteType type)
    {
        noteNiceZoneList[(int)type] = note;
    }

    public void RemoveNoteNiceZone(NoteType type)
    {
        noteNiceZoneList[(int)type] = null;
    }

    public void AddScore(NoteType type, int count)
    {
        score[(int)type] += count;
        UpdateWwiseRTPC();
    }

    void UpdateWwiseRTPC()
    {
        int scoreRed = Mathf.Clamp(score[(int)NoteType.RED], 0, 1000);
        int scoreYellow = Mathf.Clamp(score[(int)NoteType.YELLOW], 0, 1000);
        int scoreGreen = Mathf.Clamp(score[(int)NoteType.GREEN], 0, 1000);
        int scoreBlue = Mathf.Clamp(score[(int)NoteType.BLUE], 0, 1000);

        AkSoundEngine.SetRTPCValue("Score_Red", scoreRed);
        AkSoundEngine.SetRTPCValue("Score_Yellow", scoreYellow);
        AkSoundEngine.SetRTPCValue("Score_Green", scoreGreen);
        AkSoundEngine.SetRTPCValue("Score_Blue", scoreBlue);
    }

    void StartSong()
    {
        songEvent.Post(gameObject, (uint)AkCallbackType.AK_EndOfEvent | (uint)AkCallbackType.AK_Duration, CallBackMainSong);
        songIsStart = true;
    }

    public void StopSong()
    {
        songEvent.Stop(gameObject);
    }

    void CallBackMainSong(object in_cookie, AkCallbackType in_type, object in_callbackInfo)
    {
        if (in_type == AkCallbackType.AK_Duration)
        {
            AkDurationCallbackInfo info = in_callbackInfo as AkDurationCallbackInfo;
            songTimeMax = info.fDuration / 1000.0f;
        }
        else if (in_type == AkCallbackType.AK_EndOfEvent)
        {
            isEnd = true;
            endEvent?.Invoke();
        }
    }

    void CountCurSongTime()
    {
        if (songIsStart)
        {
            songTime += Time.deltaTime;
        }
    }

    void ChangePPVignette()
    {
        if (songTime < songTimeMax && songTime != 0.0f)
        {
            vignettePP.intensity.value = (songTime / songTimeMax) * 0.6f;
        }
    }
}
