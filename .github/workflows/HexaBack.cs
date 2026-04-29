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
        private const float JumpVelocityDivider = 1.5f;
        private const float GripMultiplier = 0.1f;

        private static bool _modEnabled = true;

        public override void OnInitializeMelon()
        {
            try
            {
                LoggerInstance.Msg("Initialized HexaBack.");
                Hooking.OnLevelLoaded += OnLevelLoaded;
                CreateBoneMenu();
            }
            catch (System.Exception e)
            {
                LoggerInstance.Error("HexaBack failed to init: " + e.Message);
            }
        }

        private void CreateBoneMenu()
        {
            var category = Menu.CreateCategory("HexaBack", Color.green);
            category.CreateBoolElement("Enabled", Color.white, _modEnabled, (val) =>
            {
                _modEnabled = val;
                if (_modEnabled)
                    ApplyRig();
                else
                    RevertRig();
            });
        }

        private static void OnLevelLoaded(LevelInfo levelInfo)
        {
            if (_modEnabled)
                ApplyRig();
        }

        private static void ApplyRig()
        {
            try
            {
                var rm = Player.RigManager;
                if (rm == null) return;

                var physRig = rm.physicsRig;
                if (physRig == null) return;

                // Disable arm colliders (make triggers) — Boneworks style
                SetHandTrigger(physRig.leftHand, true);
                SetHandTrigger(physRig.rightHand, true);
                SetTransformTrigger(physRig.m_shoulderLf, true);
                SetTransformTrigger(physRig.m_shoulderRt, true);

                // Adjust jump velocity
                if (physRig.remapHeptaRig != null)
                {
                    physRig.remapHeptaRig.jumpVelocity =
                        physRig.remapHeptaRig.jumpVelocity / JumpVelocityDivider;
                }

                // Adjust grip on hands (safe null checks)
                AdjustHandGrip(physRig.leftHand);
                AdjustHandGrip(physRig.rightHand);

                MelonLogger.Msg("[HexaBack] Rig applied.");
            }
            catch (System.Exception e)
            {
                MelonLogger.Error("[HexaBack] ApplyRig error: " + e.Message);
            }
        }

        private static void RevertRig()
        {
            try
            {
                var rm = Player.RigManager;
                if (rm == null) return;

                var physRig = rm.physicsRig;
                if (physRig == null) return;

                SetHandTrigger(physRig.leftHand, false);
                SetHandTrigger(physRig.rightHand, false);
                SetTransformTrigger(physRig.m_shoulderLf, false);
                SetTransformTrigger(physRig.m_shoulderRt, false);

                MelonLogger.Msg("[HexaBack] Rig reverted.");
            }
            catch (System.Exception e)
            {
                MelonLogger.Error("[HexaBack] RevertRig error: " + e.Message);
            }
        }

        private static void SetHandTrigger(PhysHand hand, bool isTrigger)
        {
            if (hand == null) return;
            foreach (var col in hand.GetComponents<Collider>())
                if (col != null) col.isTrigger = isTrigger;
            for (int i = 0; i < hand.transform.childCount; i++)
            {
                var child = hand.transform.GetChild(i);
                foreach (var col in child.GetComponents<Collider>())
                    if (col != null) col.isTrigger = isTrigger;
            }
        }

        private static void SetTransformTrigger(Transform t, bool isTrigger)
        {
            if (t == null) return;
            foreach (var col in t.GetComponents<Collider>())
                if (col != null) col.isTrigger = isTrigger;
        }

        private static void AdjustHandGrip(PhysHand hand)
        {
            if (hand == null) return;
            try
            {
                hand.gripMult = GripMultiplier;
            }
            catch { /* gripMult may not exist on all rigs, safe to skip */ }
        }
    }
}
