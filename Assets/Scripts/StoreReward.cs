using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreReward : MonoBehaviour
{
    public Upgrade upgradeInfo;


    public void setRewardInfo(Upgrade upgrade)
    {
        upgradeInfo = upgrade;
        gameObject.transform.GetChild(0).gameObject.GetComponent<TooltipText>().UpdateTooltip(upgrade);
    }
}
