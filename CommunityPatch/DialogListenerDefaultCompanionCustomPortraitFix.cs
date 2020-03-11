using System;
using System.Reflection;
using Harmony12;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.GameModes;
using Kingmaker.UI.Dialog;
using Kingmaker.UI.UnitSettings;

namespace CommunityPatch
{
    // Token: 0x02000010 RID: 16
    [HarmonyPatch(typeof(Kingmaker.UI.Dialog.DialogController), "HandleOnCueShow", MethodType.Normal)]
    public static class DialogController_HandleOnCueShow_Prefix_Patch
    {

        public static void Prefix(DialogController __instance)
        {
            if (!Main.modEnabled)
            {
                return;
            }
            if (!Main.settings.useDialogListenerDefaultCompanionCustomPortraitFix)
            {
                return;
            }
            if (Game.Instance.DialogController.CurrentCue.Listener?.CharacterName == Game.Instance.Player.MainCharacter.Value.CharacterName)
            {
                return;
            }
            if (Game.Instance.Player.AllCharacters.Find(x => x.CharacterName == Game.Instance.DialogController.CurrentCue.Listener?.CharacterName) == null)
            {
                return;
            }

  

            foreach (UnitEntityData u in Game.Instance.DialogController.InvolvedUnits)
            {
                if (Game.Instance.DialogController.CurrentCue.Listener.name == u.Blueprint.name &&
                    u.Portrait.IsCustom)
                {

                    BlueprintPortrait blueprintPortrait = new BlueprintPortrait();
                    blueprintPortrait.Data = u.Portrait;
                    typeof(BlueprintUnit).GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(Game.Instance.DialogController.CurrentCue.Listener, blueprintPortrait);


                }

            }


            return;

        }
    }
}
