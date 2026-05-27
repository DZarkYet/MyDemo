using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLua;

public class LuaComboDataLoader : MonoBehaviour
{
    public ComboDataSO comboData;

    void Start()
    {
        if (LuaManager.Instance == null || comboData == null) return;

        LuaTable table = LuaManager.Instance.DoFile("logic/combo_data");
        if (table == null) return;

        for(int i = 0; i < comboData.combos.Length; i++)
        {
            LuaTable son = table.Get<int, LuaTable>(i);
            if (son == null) continue;
            var combo = comboData.combos[i];

            LuaTable centerTable = son.Get<string, LuaTable>("hitboxCenter");
            if(centerTable != null)
            {
                combo.hitboxCenter = new Vector3(centerTable.Get<float>("x"),
                                             centerTable.Get<float>("y"),
                                             centerTable.Get<float>("z"));
                centerTable.Dispose();
            }

            LuaTable sizeTable = son.Get<string, LuaTable>("hitboxSize");
            if(sizeTable != null)
            {
                combo.hitboxSize = new Vector3(sizeTable.Get<float>("x"),
                                             sizeTable.Get<float>("y"),
                                             sizeTable.Get<float>("z"));
                sizeTable.Dispose();
            }

            combo.activeDuration = son.Get<float>("activeDuration");
            combo.damage = son.Get<int>("damage");

            comboData.combos[i] = combo;
            son.Dispose();
        }

        Debug.Log("Combo数据热更新完成");
        table.Dispose();
    }


}
