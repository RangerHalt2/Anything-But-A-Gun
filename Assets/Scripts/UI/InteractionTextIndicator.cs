using TMPro;
using UnityEngine;

public class InteractionTextIndicator : MonoBehaviour
{   
    
    private BoundTextDisplayModifier textMod;
    private TextMeshProUGUI textMeshPro;

    private void Start()
    {
        textMod = GetComponent<BoundTextDisplayModifier>();
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void OnEnable()
    {
        if(textMod != null && textMeshPro != null)
            textMeshPro.text = "Press " + textMod.ReturnRelevantText() + " to interact";
    }
}
