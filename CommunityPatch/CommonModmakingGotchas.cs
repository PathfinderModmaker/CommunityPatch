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
using Kingmaker.EntitySystem.Entities;
using Kingmaker;
using Kingmaker.DialogSystem.Blueprints;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace CommunityPatch
{
    public static class CommonModmakingGotchas
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
                Main.SafeLoad(new Action(CommonModmakingGotchas.Init), "Example1 of Startup Asset Load Here");
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


            Main.SafeLoad(new Action(CommonModmakingGotchas.Init), "Example2 of Asset Load Here");


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


        public static void GameFlagAndConversationHappenedExample()
        {
            foreach (UnitEntityData unitEntityData in Game.Instance.State.PlayerState.AllCharacters)
            {
                string characterName = unitEntityData.CharacterName;
                string characterCondition = "";
                if (characterName == "Valerie")
                {
                    //Scar
                    BlueprintDialog blueprintDialog = Game.Instance.Player.Dialog.ShownDialogs.ToList<BlueprintDialog>().Find(x => x.AssetGuid == "3fb2516a55a21684aac00eb4f4015a77");
                    if (blueprintDialog != null)
                    {
                        characterCondition = "ValerieScar";

                    }
                    //Scar Healed
                    BlueprintCueBase blueprintCueBase = Game.Instance.Player.Dialog.ShownCues.ToList<BlueprintCueBase>().Find(x => x.AssetGuid == "3bc0f984c248897479bc30b16d91ffc5");
                    if (blueprintCueBase != null)
                    {

                        characterCondition = "ValerieHealed";

                    }

                }
                if (characterName == "Tristian")
                {
                    //Blinded
                    if (Game.Instance.Player.UnlockableFlags.UnlockedFlags.Keys.FirstOrDefault(x => x.name.Equals("OculusShattered")))
                    {
                        characterCondition = "TristianBlind";

                    }

                }
                Main.DebugLog(characterName + " - " + characterCondition);

            }
        }

        //THESE ONWARDS ARE NOT TESTED YET!!!!
        public static object getPrivateField(object fieldsObject, string fieldName)
        {
            return fieldsObject.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(fieldsObject);
        }

        //NOT TESTED YET!!!!
        public static bool setPrivateField(object fieldsObject, string fieldName, object value)
        {
            bool result = true;
            try
            {
                fieldsObject.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(fieldsObject, value);
            }
            catch (Exception ex)
            {
                Main.DebugError(ex);
                throw ex;

            }
            return result;
        }

        //NOT TESTED YET!!!!
        public static object getComponentOfType(GameObject gameObject, string partialTypeName)
        {
            return gameObject.GetComponentsInChildren<Component>().ToList().Find(x => x.GetType().ToString().Contains(partialTypeName));
        }


        //NOT TESTED YET!!!!
        public static void logAllComponentsOfType(GameObject gameObject, string partialTypeName)
        {
            gameObject.GetComponentsInChildren<Component>().ToList().ForEach(x => Main.DebugLog(x.GetType().ToString()));
        }


        //NOT TESTED YET!!!!
        public static void startCoroutineExample()
        {

            List<string> guids = new List<string>();
            guids.Add("18a65db9e92055240bea7c7a5783587a");
            guids.Add("06d447a763b392c438a93e145a95e2d1");
            guids.Add("a92e3099b69e41e41818b4853a432118");

            List<BlueprintUnit> blueprintUnits = new List<BlueprintUnit>();

            foreach (BlueprintUnit blueprintUnit in ResourcesLibrary.GetBlueprints<BlueprintUnit>().Where(x => guids.Contains(x.AssetGuid)))
            { blueprintUnits.Add(blueprintUnit); }

            coroutineExample = new GameObject("coroutineExample").AddComponent<CoroutineExample>();

            coroutineExample.Run(blueprintUnits);

        }

        //NOT TESTED YET!!!!
        public static void stopCoroutineExample()
        {
            if (coroutineExample != null)
            {
                coroutineExample.Stop();
                coroutineExample.StopAllCoroutines();
            }

        }

        //NOT TESTED YET!!!!
        private class CoroutineExample : MonoBehaviour
        {
            private IEnumerator coroutine;
            // Token: 0x0600074B RID: 1867 RVA: 0x00038467 File Offset: 0x00036667
            internal void Run(List<BlueprintUnit> units)
            {
                //  UnityModManager.Logger.Log("Run()");
                UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
                base.gameObject.hideFlags = HideFlags.HideAndDontSave;

                coroutine = this.Load(units);

                base.StartCoroutine(coroutine);
            }

            public void Stop()
            {
                StopCoroutine(coroutine);

                Main.DebugLog("Stopped coroutine" + Time.time);
            }

            // Token: 0x0600074C RID: 1868 RVA: 0x00038492 File Offset: 0x00036692
            private IEnumerator<object> Load(List<BlueprintUnit> units)
            {



                foreach (BlueprintUnit unit in units)
                {

                    Main.DebugLog(unit.name);
                    //wait 2 seconds
                    yield return new WaitForSeconds(2f);

                    Main.DebugLog(unit.CharacterName);
                    //wait 0.5 seconds
                    yield return new WaitForSeconds(0.5f);


                }

                coroutineExample = null;
                UnityEngine.Object.Destroy(base.gameObject);

                yield break;
            }
        }

        private static CoroutineExample coroutineExample;


    }







}
