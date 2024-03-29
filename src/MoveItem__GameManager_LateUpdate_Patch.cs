﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace LoadTravois
{
    [HarmonyPatch(typeof(GameManager), "LateUpdate")]
    public static class MoveItem__GameManager_LateUpdate_Patch
    {
        private static MethodInfo DropInInventoryInfo;

        static MoveItem__GameManager_LateUpdate_Patch()
        {
            DropInInventoryInfo = AccessTools.Method(typeof(InGameCardBase), "DropInInventory");
        }

        public static void Postfix(GameManager __instance)
        {
            if (Plugin.HotKey.Value.IsPressed() && !GraphicsManager.Instance.HasPopup)
            {
                CardData currentEnvironment = GameManager.Instance.CurrentEnvironment;

                //Find a Travois
                //If there are multiple, only use the first one found.
                InGameCardBase travois = __instance.ItemCards.FirstOrDefault(x =>
                    x.CardModel.CardName.DefaultText == "Travois" && x.Environment == currentEnvironment);
                
                if (travois == null)
                {
                    return;
                }

				List<InGameCardBase> environmentCards = __instance.ItemCards
                    .Where(x => x.Environment == currentEnvironment).ToList();

				List<InGameCardBase> containers = Plugin.CardMoveList
                    .Join(environmentCards, filter => filter.Value,
                        card => card.CardModel.CardName.DefaultText, (containerFilter, card) => new {containerFilter, card})
                    .Where(x => x.card.CurrentContainer?.CardModel.CardName.DefaultText != "Travois")
                    .OrderBy(x => x.containerFilter.Key)
                    .Select(x => x.card)
                    .ToList();

                foreach (InGameCardBase container in containers)
                {
                    DropInInventoryInfo.Invoke(travois, new object[] { container });
                }
            }
        }
    }
}