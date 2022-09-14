using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class MonitorUI 
{
    public static string PlayerHpUI(int currentHp,int maxHp)
    {
        var sb = new StringBuilder();
        sb.Append("Ammo: ");
        sb.Append(currentHp.ToString("00"));
        sb.Append('/');
        sb.Append(maxHp.ToString("00"));
        sb.Append(' ');

        var color = new Color(.25f, .25f, .3f);
        sb.Append('▐', currentHp);
        sb.Append("<color=#");
        sb.Append(ColorUtility.ToHtmlStringRGB(color));
        sb.Append('>');
        sb.Append('▐', maxHp - currentHp);
        sb.Append("</color>");

        return sb.ToString();
    }
}
