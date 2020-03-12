using UnityModManagerNet;
using System;
using System.Reflection;
using System.Linq;
using Kingmaker.Blueprints;
using Kingmaker;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Localization;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.Localization.Shared;
using UnityEngine;
using System.IO;
using Harmony12;
using Kingmaker.Visual.CharacterSystem;
using System.Collections.Generic;
using static Harmony12.AccessTools;
using System.Reflection.Emit;

namespace CommunityPatch
{
#if DEBUG
    [EnableReloading]
#endif
    public class Main
    { 
         

        static bool Load(UnityModManager.ModEntry modEntry)
        {


            try
            {
                Main.logger = modEntry.Logger;
                Main.modEnabled = modEntry.Active;
                Main.settings = UnityModManager.ModSettings.Load<Settings>(modEntry);

                CommonModmakingGotchas.isAssetLoaderRunning = true;
                modEntry.OnUpdate = new Action<UnityModManager.ModEntry, float>(CommonModmakingGotchas.AssetLoader_Compatibility);
                
                modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(Main.OnToggle);
                modEntry.OnGUI = new Action<UnityModManager.ModEntry>(Main.OnGUI);


                Main.harmonyInstance = HarmonyInstance.Create(modEntry.Info.Id);
                modEntry.OnUnload = new Func<UnityModManager.ModEntry, bool>(Main.Unload);
                //    var harmony = Harmony12.HarmonyInstance.Create(modEntry.Info.Id);
                //    harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                DebugError(ex);
                throw ex;
            }


            if (!Main.ApplyPatch(typeof(LeaderState_Leader_GetPortrait_Patch), "Quarter Sized Fullength Custom Portraits on Kingdom Management Screen Fix"))
            {
                DebugLog("Failed to patch LeaderState_Leader_GetPortrait");
            }
            if (!Main.ApplyPatch(typeof(DialogController_HandleOnCueShow_Patch), "Not Updating Dialog Listener Name at OnSpeakerNameChange Fix"))
            {
                DebugLog("Failed to patch DialogController_HandleOnCueShow");
            }
            if (!Main.ApplyPatch(typeof(DialogController_HandleOnCueShow_Prefix_Patch), "Missing Custom Portrait when Dialog Listener is Default Companion Fix"))
            {
                DebugLog("Failed to patch DialogController_HandleOnCueShow_Prefix");
            }
        /*    if (!Main.ApplyPatch(typeof(CommunityPatch.Examples.LibraryScriptableObject_LoadDictionary_Patch), "Run once startup hook for Startup AssetLoader Example1 "))
            {
                DebugLog("Failed to patch LibraryScriptableObject_LoadDictionary");
            }*/

            
            return true;
        }
        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayoutOption[] noExpandwith = new GUILayoutOption[]
             {
                GUILayout.ExpandWidth(false)
             };




            if (Main.okPatches.Count > 0)
            {
                GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());

                GUILayout.Label("<b>OK: Some patches apply:</b>", noExpandwith);

                foreach (string str in Main.okPatches)
                {
                    GUILayout.Label("  • <b>" + str + "</b>", noExpandwith);
                }
                GUILayout.EndVertical();
            }
            if (Main.failedPatches.Count > 0)
            {
                GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
                GUILayout.Label("<b>Error: Some patches failed to apply. These features may not work:</b>", noExpandwith);
                foreach (string str2 in Main.failedPatches)
                {
                    GUILayout.Label("  • <b>" + str2 + "</b>", noExpandwith);
                }
                GUILayout.EndVertical();
            }
            if (Main.okLoading.Count > 0)
            {
                GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
                GUILayout.Label("<b>OK: Some assets loaded:</b>", noExpandwith);
                foreach (string str3 in Main.okLoading)
                {
                    GUILayout.Label("  • <b>" + str3 + "</b>", noExpandwith);
                }
                GUILayout.EndVertical();
            }
            if (Main.failedLoading.Count > 0)
            {
                GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
                GUILayout.Label("<b>Error: Some assets failed to load. Saves using these features won't work:</b>", noExpandwith);
                foreach (string str4 in Main.failedLoading)
                {
                    GUILayout.Label("  • <b>" + str4 + "</b>", noExpandwith);
                }
                GUILayout.EndVertical();
            }
        } 
        internal static bool ApplyPatch(Type type, string featureName)
        {
            bool result;
            try
            {
                if (Main.typesPatched.ContainsKey(type))
                {
                    result = Main.typesPatched[type];
                }
                else
                {
                    List<HarmonyMethod> harmonyMethods = type.GetHarmonyMethods();
                    if (harmonyMethods == null || harmonyMethods.Count<HarmonyMethod>() == 0)
                    {

                        DebugLog("Failed to apply patch " + featureName + ": could not find Harmony attributes.");
                        Main.failedPatches.Add(featureName);
                        Main.typesPatched.Add(type, false);
                        result = false;
                    }
                    else if (new PatchProcessor(Main.harmonyInstance, type, HarmonyMethod.Merge(harmonyMethods)).Patch().FirstOrDefault<DynamicMethod>() == null)
                    {
                        DebugLog("Failed to apply patch " + featureName + ": no dynamic method generated");

                        Main.failedPatches.Add(featureName);
                        Main.typesPatched.Add(type, false);
                        result = false;
                    }
                    else
                    {
                        Main.okPatches.Add(featureName);
                        Main.typesPatched.Add(type, true);
                        result = true;
                    }
                }
            }
            catch (Exception arg)
            {
                DebugLog("Failed to apply patch " + featureName + ": " + arg + ", type: " + type);
                Main.failedPatches.Add(featureName);
                Main.typesPatched.Add(type, false);
                result = false;
            }
            return result;
        }
        internal static void SafeLoad(Action load, string name)
        {
            try
            {
                load();
                Main.okLoading.Add(name);
            }
            catch (Exception e)
            {
                Main.okLoading.Remove(name);
                Main.failedLoading.Add(name);
                DebugError(e);
            }
        }


        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            Main.modEnabled = value;
            if(Main.modEnabled) Main.settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            return true;
        }
        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            Main.settings.Save(modEntry);
        }

        // Token: 0x06000026 RID: 38 RVA: 0x000021B3 File Offset: 0x000003B3
        private static bool Unload(UnityModManager.ModEntry modEntry)
        {
            Main.settings.Save(modEntry);
            if (Main.okPatches.Count > 0)
            {
                /*if (Main.spawner != null)
                {
                    Main.spawner.Stop();
                    Main.spawner.StopAllCoroutines();
                }*/
            harmonyInstance.UnpatchAll(modEntry.Info.Id);

                //HarmonyInstance.Create(modEntry.Info.Id).UnpatchAll(null);
                return true;
            }
            else { UnityModManager.Logger.Log("couldn't find patches to unload!"); return true; }
        }





        

        public static void DebugLog(string msg)
        {
            if (logger != null) logger.Log(msg);
        }
        public static void DebugError(Exception ex)
        {
            if (logger != null) logger.Log(ex.ToString() + "\n" + ex.StackTrace);
        }


        public static UnityModManagerNet.UnityModManager.ModEntry.ModLogger logger;
        public static bool modEnabled;
        private static HarmonyInstance harmonyInstance;
        internal static Settings settings;
        private static readonly Dictionary<Type, bool> typesPatched = new Dictionary<Type, bool>();
        private static readonly List<string> failedPatches = new List<string>();
        private static readonly List<string> okPatches = new List<string>();
        private static readonly List<string> okLoading = new List<string>();
        private static readonly List<string> failedLoading = new List<string>();

    }
}
