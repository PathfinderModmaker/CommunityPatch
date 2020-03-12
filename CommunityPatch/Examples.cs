using Harmony12;
using System.Reflection;
using Kingmaker.Blueprints;
using Kingmaker.Kingdom;
using UnityEngine;
using System;
using UnityModManagerNet;
using Kingmaker.Localization;
using Kingmaker.Blueprints.Area;
using Kingmaker.Cheats;
using Kingmaker.Designers;

namespace CommunityPatch
{
    public static class Examples
    {
        public static void Init()
        {

            //Main.DebugLog("Init()");

        }
        // Token: 0x02000015 RID: 21

        [Harmony12.HarmonyPatch(typeof(LibraryScriptableObject), "LoadDictionary")]
        public static class LibraryScriptableObject_LoadDictionary_Patch
        {
            static void Postfix()
            {
                Main.SafeLoad(new Action(Examples.Init), "Example1 of Startup Asset Load Here");
            }
        }

        //this allows the mod to be enabled/disabled during runtime and preserve all functionality
        //this allows the mod to be reloaded during runtime debugging and preserve all functionality
        //this also allows the mod to work with cotw
        public static void AssetLoader_Compatibility(UnityModManager.ModEntry modEntry, float dt)
        {
            modEntry.OnUpdate = null;
            isAssetLoaderRunning = false;

            if (!Application.isPlaying || (LocalizationManager.CurrentPack == null) || (ResourcesLibrary.LibraryObject == null))
            {

                if (!isAssetLoaderRunning)
                {
                    isAssetLoaderRunning = true;
                    modEntry.OnUpdate = new Action<UnityModManager.ModEntry, float>(AssetLoader_Compatibility);

                }
                return;
            }


            Main.SafeLoad(new Action(Examples.Init), "Example2 of Asset Load Here");


        }

        public static bool isAssetLoaderRunning = false;





        internal static void xOnGUI()
        {
            Main.DebugLog("Fix for \"ArgumentException: GUILayout: Mismatched LayoutGroup.Repaint\"");

            GUILayoutOption[] noExpandwith = new GUILayoutOption[]
              {
                GUILayout.ExpandWidth(false)
              };


            if (Event.current.type == EventType.Repaint && handleRepaintErrors)
            {
                

                handleRepaintErrors = false;
                return;
            }
            if (somethingChangedOutsideOfGUI != prevSomethingChangedOutsideOfGUI)
            {
                guiElementToReactToSomethingChangedOutsideOfGUI = true;
                isSomeDataRefreshedBecauseOfSomethingChangedOutsideOfGUI = false;
                handleRepaintErrors = true;
            }
            if (guiElementToReactToSomethingChangedOutsideOfGUI)
            {

                GUILayout.Label("Pass result of RefreshData() to this Gui Element based on somethingChangedOutsideOfGUI", noExpandwith);

            }
            else
            {
                if (!isSomeDataRefreshedBecauseOfSomethingChangedOutsideOfGUI)
                {
                    somethingChangedOutsideOfGUI = prevSomethingChangedOutsideOfGUI;
                    isSomeDataRefreshedBecauseOfSomethingChangedOutsideOfGUI = true;
                    Main.DebugLog("Call here theoretical RefreshData()");
                }
                //  GUILayout.Label("No refreshed data", GUILayout.ExpandWidth(true));

            }
        }
        private static bool handleRepaintErrors = false;
        private static bool somethingChangedOutsideOfGUI = false;
        private static bool prevSomethingChangedOutsideOfGUI = false;
        private static bool isSomeDataRefreshedBecauseOfSomethingChangedOutsideOfGUI = false;
        private static bool guiElementToReactToSomethingChangedOutsideOfGUI = false;



        public static void Teleport(string blueprintAreaEnterPointAssetGuid)
        {
            BlueprintAreaEnterPoint bae = Utilities.GetBlueprint<BlueprintAreaEnterPoint>(blueprintAreaEnterPointAssetGuid);
            GameHelper.EnterToArea(bae, Kingmaker.EntitySystem.Persistence.AutoSaveMode.None);
        }
    }
}
