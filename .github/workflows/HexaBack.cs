using MelonLoader;
using BoneLib;
using BoneLib.BoneMenu;
using UnityEngine;
using Il2CppSLZ.Marrow;

[assembly: MelonInfo(typeof(HexaBack.Core), "HexaBack", "1.0.2", "freakycheesy")]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]

namespace HexaBack
{
    public class Core : MelonMod
    {
        private static bool _modEnabled = true;

        public override void OnInitializeMelon()
        {
            try
            {
                LoggerInstance.Msg("Initialized HexaBack.");
                SetupMenu();
            }
            catch (System.Exception e)
            {
                LoggerInstance.Error("HexaBack init error: " + e.Message);
                LoggerInstance.Error(e.StackTrace);
            }
        }

        private void SetupMenu()
        {
            var category = Menu.CreateCategory("HexaBack", Color.green);
            category.CreateBoolElement("Enabled", Color.white, _modEnabled, (val) =>
            {
                _modEnabled = val;
                MelonLogger.Msg("[HexaBack] " + (val ? "Enabled" : "Disabled"));
            });
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (!_modEnabled) return;
            MelonLogger.Msg("[HexaBack] Scene loaded: " + sceneName);
            ApplyRig();
        }

        private static void ApplyRig()
        {
            try
            {
                var rm = Player.RigManager;
                if (rm == null)
                {
                    MelonLogger.Msg("[HexaBack] RigManager is null, skipping.");
                    return;
                }

                var physRig = rm.physicsRig;
                if (physRig == null)
                {
                    MelonLogger.Msg("[HexaBack] physicsRig is null, skipping.");
                    return;
                }

                SetHandTrigger(physRig.leftHand, true);
                SetHandTrigger(physRig.rightHand, true);

                MelonLogger.Msg("[HexaBack] Rig applied successfully!");
            }
            catch (System.Exception e)
            {
                MelonLogger.Error("[HexaBack] ApplyRig error: " + e.Message);
                MelonLogger.Error(e.StackTrace);
            }
        }

        private static void SetHandTrigger(PhysHand hand, bool isTrigger)
        {
            if (hand == null) return;
            try
            {
                var colliders = hand.GetComponents<Collider>();
                foreach (var col in colliders)
                    if (col != null) col.isTrigger = isTrigger;
            }
            catch (System.Exception e)
            {
                MelonLogger.Error("[HexaBack] SetHandTrigger error: " + e.Message);
            }
        }
    }
}
