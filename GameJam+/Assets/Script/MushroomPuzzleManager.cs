using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomPuzzleManager : MonoBehaviour
{
    public static MushroomPuzzleManager Instance;

    // Urutan benar: 0 = tengah, 1 = kanan, 2 = kiri
    private int[] correctOrder = { 0, 1, 2 };
    private int index = 0; // index tetap dan TIDAK di-reset saat salah

    public GameObject fragmentPrefab;
    public Transform fragmentSpawnPoint;

    public WindowInteract exitDoor;
    private bool fragmentSpawned = false;
    private bool fragmentCollected = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (exitDoor != null)
            exitDoor.canClick = false;
    }

    public void OnMushroomClicked(int mushroomID, MushroomPuzzleItem item)
    {
        if (fragmentSpawned) return; // puzzle tidak bisa diulang

        if (mushroomID == correctOrder[index])
        {
            index++;
            item.PlayCorrectAnimation(); // item BISA diklik lagi jika solved=false dihilangkan
                                         // Tapi kita ingin item yang sudah benar TIDAK bisa diklik lagi (solved = true)

            if (index >= correctOrder.Length)
            {
                Debug.Log("Puzzle Selesai!");
                StartCoroutine(SpawnFragment());
            }
        }
        else
        {
            item.PlayWrongAnimation();
            // index TIDAK di-reset. Pemain harus mencoba lagi dengan item yang benar (correctOrder[index]).
        }
    }

    IEnumerator SpawnFragment()
    {
        fragmentSpawned = true;

        yield return new WaitForSeconds(0.4f);
        if (this == null) yield break; // PROTEKSI

        GameObject f = Instantiate(fragmentPrefab, fragmentSpawnPoint.position, Quaternion.identity);

        // Dapatkan komponen FragmentFloating
        FragmentFloating fragmentItem = f.GetComponent<FragmentFloating>();

        if (fragmentItem != null)
        {
            // Panggil animasi spawn yang baru
            fragmentItem.PlaySpawnAnimation();
        }
    }

    IEnumerator FloatFragment(GameObject obj)
    {
        float t = 0;
        Vector3 start = obj.transform.position;

        while (!fragmentCollected)
        {
            if (this == null) yield break; // PROTEKSI
            t += Time.deltaTime;
            obj.transform.position = start + new Vector3(0, Mathf.Sin(t * 2f) * 0.2f, 0);
            obj.transform.Rotate(0, 0, 40 * Time.deltaTime);
            yield return null;
        }
    }

    public void OnFragmentCollected()
    {
        fragmentCollected = true;

        Debug.Log("Fragment dikumpulkan!");

        // SIMPAN ke GameManager
        // GameManager.Instance.SetPuzzleArea2Solved(); 

        // aktifkan pintu
        if (exitDoor != null)
            exitDoor.canClick = true;
    }

    // HAPUS METODE ResetPuzzle() dan ExecuteResetAfterWrongAnimation()
}