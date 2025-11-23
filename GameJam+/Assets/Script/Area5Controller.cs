using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Tambahkan ini untuk SceneManager

public class Area5Controller : MonoBehaviour
{
    // === SINGLETON PATTERN ===
    public static Area5Controller Instance { get; private set; }

    [Header("Camera Settings")]
    public Camera mainCam;
    public float zoomSize = 3.5f;
    public float zoomSpeed = 2f;

    [Header("Puzzle UI Panel")]
    public GameObject puzzlePanel;
    public CanvasGroup puzzleCanvasGroup;

    [Header("Connected Scripts")]
    public Area3DialogueVN dialogueVN;
    public PuzzleManager puzzleManager;

    [Header("Fragment Objects")]
    // Referensi ke objek Game Fragment (bukan script)
    public GameObject fragment3Object;
    public GameObject fragment4Object;
    public GameObject fragment5Object;

    [Header("Area Exit")]
    // Referensi ke BlackPortal yang menggunakan SpriteRenderer
    public BlackPortal lightPortal;
    public string nextSceneName = "Area 8";

    private Vector3 camStartPos;
    private float camStartSize;
    private bool puzzleOpened = false;

    // Counter untuk memastikan semua fragment sudah dikumpulkan (Total 3)
    private int collectedFragmentsCount = 0;
    private const int totalFragments = 3;

    void Awake()
    {
        // Implementasi Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        camStartPos = mainCam.transform.position;
        camStartSize = mainCam.orthographicSize;

        puzzlePanel.SetActive(false);
        puzzleCanvasGroup.alpha = 0;
        puzzleCanvasGroup.blocksRaycasts = false;
        puzzleCanvasGroup.interactable = false;

        // Pastikan fragment non-aktif di awal
        if (fragment3Object != null) fragment3Object.SetActive(false);
        if (fragment4Object != null) fragment4Object.SetActive(false);
        if (fragment5Object != null) fragment5Object.SetActive(false);

        // Portal juga non-aktif
        if (lightPortal != null) lightPortal.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Panggil fungsi deteksi klik hanya jika dialog tidak aktif dan puzzle tidak aktif (jika ingin)
            // Namun, kita akan membiarkannya berjalan dan mengontrolnya di dalam DetectClick.
            DetectClick();
        }
    }

    // Dipanggil dari Fragment3/4/5.cs setelah animasi pickup selesai
    public void OnFragmentCollected()
    {
        collectedFragmentsCount++;
        Debug.Log("Fragment collected. Total: " + collectedFragmentsCount);

        if (collectedFragmentsCount >= totalFragments)
        {
            // Semua fragment sudah dikumpulkan, buka portal
            if (lightPortal != null)
            {
                lightPortal.OpenPortal();
            }
        }
    }

    // ... (ClickThrone, ClickQueen, ClickTable, OpenPuzzleSequence tetap sama) ...

    public void ClickThrone()
    {
        dialogueVN.StartDialogue(new Area3DialogueVN.DialogueLine[]
        {
            new Area3DialogueVN.DialogueLine {
                text = "Jangan sentuh tahtaku!",
                isPlayer = false,
                speaker = "Queen"
            }
        });
    }

    public void ClickQueen()
    {
        dialogueVN.StartDialogue(new Area3DialogueVN.DialogueLine[]
        {
            new Area3DialogueVN.DialogueLine {
                text = "Apakah kamu mau bermain denganku?",
                isPlayer = false,
                speaker = "Queen"
            }
        });
    }

    public void ClickTable()
    {
        if (puzzleOpened) return;

        puzzleOpened = true;
        StartCoroutine(OpenPuzzleSequence());
    }

    IEnumerator OpenPuzzleSequence()
    {
        // ... (Logika zoom dan fade in puzzle) ...
        Vector3 targetPos = camStartPos + new Vector3(0, -1f, 0);
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * zoomSpeed;
            mainCam.orthographicSize = Mathf.Lerp(camStartSize, zoomSize, t);
            mainCam.transform.position = Vector3.Lerp(camStartPos, targetPos, t);
            yield return null;
        }

        puzzlePanel.SetActive(true);

        puzzleCanvasGroup.alpha = 0;
        puzzleCanvasGroup.blocksRaycasts = true;
        puzzleCanvasGroup.interactable = true;

        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            puzzleCanvasGroup.alpha = t;
            yield return null;
        }

        puzzleManager.EnablePuzzle();
    }

    public void ClosePuzzleAndTalk()
    {
        StartCoroutine(ClosePuzzleSequence());
    }

    IEnumerator ClosePuzzleSequence()
    {
        float t = 0;

        Vector3 startPos = mainCam.transform.position;
        float startSize = mainCam.orthographicSize;

        // zoom out
        while (t < 1f)
        {
            t += Time.deltaTime * zoomSpeed;
            mainCam.orthographicSize = Mathf.Lerp(startSize, camStartSize, t);
            mainCam.transform.position = Vector3.Lerp(startPos, camStartPos, t);
            yield return null;
        }

        // fade out puzzle
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            puzzleCanvasGroup.alpha = 1f - t;
            yield return null;
        }

        puzzlePanel.SetActive(false);
        puzzleCanvasGroup.blocksRaycasts = false;
        puzzleCanvasGroup.interactable = false;

        // === AKTIFKAN FRAGMENT DI SINI ===
        ActivateFragments();

        dialogueVN.StartDialogue(new Area3DialogueVN.DialogueLine[] {
            new Area3DialogueVN.DialogueLine {
                text = "Hahaha! Sudah lama tidak ada yang mau bermain denganku! Aku akan memberimu hadiah dan akses ke ruang harta karunku. Pergilah!",
                isPlayer = false,
                speaker = "Queen"
            }
        });
    }

    void ActivateFragments()
    {
        // Setelah puzzle selesai, aktifkan objek fragment agar pemain bisa mengkliknya
        if (fragment3Object != null) fragment3Object.SetActive(true);
        if (fragment4Object != null) fragment4Object.SetActive(true);
        if (fragment5Object != null) fragment5Object.SetActive(true);
        Debug.Log("Fragments are now active and ready to be picked up.");
    }

    // Metode ClickPortal() tetap sama, tidak peduli BlackPortal pakai Sprite atau Image.
    public void ClickPortal()
    {
        if (collectedFragmentsCount >= totalFragments && lightPortal != null && lightPortal.gameObject.activeSelf)
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else if (collectedFragmentsCount < totalFragments)
        {
            dialogueVN.StartDialogue(new Area3DialogueVN.DialogueLine[] {
                new Area3DialogueVN.DialogueLine {
                    text = "Aku harus mengumpulkan semua fragmen sebelum bisa melewati portal ini...",
                    isPlayer = true,
                    speaker = "Player"
                }
            });
        }
        else
        {
            ClickQueen();
        }
    }

    void DetectClick()
    {
        // === SOLUSI 1: BLOKIR KLIK DUNIA SAAT PUZZLE AKTIF ===
        // Jika panel puzzle aktif, jangan lakukan Raycast dunia.
        // Asumsi: Klik UI Card Anda sudah ditangani oleh sistem EventSystem Unity.
        if (puzzlePanel.activeSelf)
        {
            return; // Hentikan fungsi agar tidak memproses klik pada objek 2D dunia
        }

        Vector2 worldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider == null) return;

        if (hit.collider.CompareTag("Throne"))
            ClickThrone();
        else if (hit.collider.CompareTag("Queen"))
            ClickQueen();
        else if (hit.collider.CompareTag("Table"))
            ClickTable();
        // === DETEKSI PINTU CAHAYA/PORTAL ===
        else if (hit.collider.CompareTag("LightDoor"))
            ClickPortal();
    }
    
}