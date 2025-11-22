using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomPuzzleManager : MonoBehaviour
{
    public static MushroomPuzzleManager Instance;

    // Urutan benar: 0 = tengah, 1 = kanan, 2 = kiri
    private int[] correctOrder = { 0, 1, 2 };
    private int index = 0;

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
            item.PlayCorrectAnimation();

            if (index >= correctOrder.Length)
            {
                Debug.Log("Puzzle Selesai!");
                StartCoroutine(SpawnFragment());
            }
        }
        else
        {
            item.PlayWrongAnimation();
            index = 0;
        }
    }

    IEnumerator SpawnFragment()
    {
        fragmentSpawned = true;

        yield return new WaitForSeconds(0.4f);

        GameObject f = Instantiate(fragmentPrefab, fragmentSpawnPoint.position, Quaternion.identity);

        // animasi melayang pelan
        StartCoroutine(FloatFragment(f));
    }

    IEnumerator FloatFragment(GameObject obj)
    {
        float t = 0;
        Vector3 start = obj.transform.position;

        while (!fragmentCollected)
        {
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
        GameManager.Instance.SetPuzzleArea2Solved();

        // aktifkan pintu
        if (exitDoor != null)
            exitDoor.canClick = true;
    }
}
