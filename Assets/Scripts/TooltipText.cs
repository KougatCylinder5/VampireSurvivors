using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipText : MonoBehaviour
{
    public void UpdateTooltip(Upgrade upgrade)
    {
        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();

        text.text = upgrade.name;
        text.text += "\n";
        text.text += upgrade.description;
    }

}
