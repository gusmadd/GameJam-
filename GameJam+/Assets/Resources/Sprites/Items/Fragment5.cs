using System.Collections;
using UnityEngine;

public class Fragment5 : MonoBehaviour
{
    public Sprite itemIcon;
    private string itemName = "Fragment5";

    public float floatHeight = 0.15f;
    public bool picked = false;

    private Vector3 startPos;
    private Vector3 initialScale;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        initialScale = transform.localScale;
        startPos = transform.position;
    }

    void Start()
    {
        StartCoroutine(SpawnAndFloat());
    }

    IEnumerator SpawnAndFloat()
    {
        float duration = 0.5f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;

            transform.localScale = Vector3.Lerp(Vector3.zero, initialScale, p);

            float flicker = Mathf.Sin(t * 20f) * 0.5f + 0.5f;
            sr.color = new Color(flicker, flicker, flicker, 1f);

            yield return null;
        }

        sr.color = Color.white;
        transform.localScale = initialScale;

        StartCoroutine(FloatingIdle());
    }

    IEnumerator FloatingIdle()
    {
        float t = 0;

        while (!picked)
        {
            t += Time.deltaTime;

            transform.position = startPos + new Vector3(0, Mathf.Sin(t * 2f) * floatHeight, 0);

            float scalePulse = 1f + Mathf.Sin(t * 4f) * 0.05f;
            transform.localScale = initialScale * scalePulse;

            float flicker = Mathf.Sin(t * 10f) * 0.5f + 0.5f;
            sr.color = new Color(flicker, flicker, flicker, 1f);

            yield return null;
        }
    }

    private void OnMouseDown()
    {
        if (!picked)
        {
            picked = true;
            StopAllCoroutines();
            StartCoroutine(PickupSequence());
        }
    }

    IEnumerator PickupSequence()
    {
        float duration = 0.8f;
        float t = 0;

        Vector3 targetCenter = Camera.main.ScreenToWorldPoint(
            new Vector3(Screen.width / 2, Screen.height / 2, 1));

        Vector3 start = transform.position;
        Vector3 peakScale = initialScale * 2f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0, 1, t / duration);

            transform.position = Vector3.Lerp(start, targetCenter, p);
            transform.localScale = Vector3.Lerp(initialScale, peakScale, p);

            transform.Rotate(0, 0, 360 * Time.deltaTime);

            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        float hideDuration = 0.4f;
        t = 0;

        while (t < hideDuration)
        {
            t += Time.deltaTime;
            float p = t / hideDuration;

            transform.position = targetCenter + new Vector3(0, Mathf.Lerp(0, -0.5f, p), 0);
            transform.localScale = Vector3.Lerp(peakScale, Vector3.zero, p);

            transform.Rotate(0, 0, 180 * Time.deltaTime);

            yield return null;
        }

        // === KODE YANG DIREVISI DENGAN NULL CHECK ===
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.AddItem(itemName, itemIcon);
        }
        else
        {
            Debug.LogError("ERROR: InventorySystem.Instance tidak ditemukan!");
        }

        if (Area5Controller.Instance != null)
        {
            Area5Controller.Instance.OnFragmentCollected();
        }
        else
        {
            Debug.LogError("ERROR: Area5Controller.Instance tidak ditemukan!");
        }
        // === AKHIR REVISI ===

        Destroy(gameObject);
    }
}
