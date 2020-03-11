using Harmony12;
using System.Reflection;
using Kingmaker.Blueprints;
using Kingmaker.Kingdom;
using UnityEngine;
using Kingmaker.UI.Dialog;
using TMPro;
using Kingmaker;
using Kingmaker.UI.Common;
using System.Collections.Generic;
using Kingmaker.EntitySystem.Entities;

namespace CommunityPatch
{
    // Token: 0x02000015 RID: 21
    [HarmonyPatch(typeof(Kingmaker.UI.Dialog.DialogController), "HandleOnCueShow", MethodType.Normal)]
    public static class DialogController_HandleOnCueShow_Patch
    {

        public static void Postfix(DialogController __instance)
        {
            if (!Main.modEnabled)
            {
                return;
            }
            if (!Main.settings.useDialogListenerOnSpeakerNameChangeFix)
            {
                return;
            }
            if(Game.Instance.DialogController.CurrentCue.Listener?.CharacterName == Game.Instance.Player.MainCharacter.Value.CharacterName)
            {
                return;
            }
            if(Game.Instance.Player.AllCharacters.Find(x => x.CharacterName == Game.Instance.DialogController.CurrentCue.Listener?.CharacterName) != null)
            {
                return;
            }

            foreach (UnitEntityData u in Game.Instance.DialogController.InvolvedUnits)
            {
                if (Game.Instance.DialogController.CurrentCue.Listener?.name == u.Blueprint.name &&
                    u.CharacterName != u.Blueprint.CharacterName)
                {
       

                    typeof(TextMeshProUGUI).GetField("m_text", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(__instance.AnswererName, UIUtility.GetSaberBookFormat(u.CharacterName, default(Color), 140, null));

                    __instance.AnswererName.gameObject.SetActive(!__instance.AnswererName.gameObject.activeSelf);
                    __instance.AnswererName.gameObject.SetActive(!__instance.AnswererName.gameObject.activeSelf);
                }

            }


            return;

        }
    }
}
