using TMPro;
using UnityEngine;

public class AmmoCounter : MonoBehaviour
{
    private AmmoManager ammoManager;

    private TextMeshProUGUI textMeshPro;

    private void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        Invoke("LateStart", 1f);
    }

    private void LateStart()
    {
        ammoManager = GameObject.FindAnyObjectByType<AmmoManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (textMeshPro != null && ammoManager != null)
        {
            textMeshPro.text = ""+ammoManager.GetCurrentAmmo();
        }
    }
}
