using Harmony12;
using System.Reflection;
using Kingmaker.Blueprints;
using Kingmaker.Kingdom;
using UnityEngine;
using System;
using Kingmaker.UI.Kingdom;
using System.Collections.Generic;
using System.Linq;
using Kingmaker.EntitySystem.Entities;

namespace CommunityPatch
{

    // Token: 0x02000015 RID: 21
    [HarmonyPatch(typeof(LeaderState.Leader), "Portrait", MethodType.Getter)]
    public static class LeaderState_Leader_GetPortrait_Patch
    {
        // Token: 0x06000053 RID: 83 RVA: 0x00005694 File Offset: 0x00003894
        public static void Postfix(LeaderState.Leader __instance, PortraitData __result)
        {
            if (!Main.modEnabled)
            {
                return;
            }
            if (!Main.settings.useKingdomManagementCustomPortraitFix)
            {
                return;
            }



            if (__result.FullLengthPortrait!=null && !__result.FullLengthPortrait.texture.name.StartsWith("sactx") &&
                !__result.FullLengthPortrait.name.Equals("resized"))
            {


               Sprite fullLengthImage = (Sprite)typeof(PortraitData).GetField("m_FullLengthImage", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__result);



                Sprite sprite = Sprite.Create(fullLengthImage.texture, fullLengthImage.textureRect, new Vector2((float)0.5, (float)0.5), 100);

                sprite.name = "resized";


                typeof(PortraitData).GetField("m_FullLengthImage", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(__result, sprite);
                
            }
            
            return;

        }
    }


}
