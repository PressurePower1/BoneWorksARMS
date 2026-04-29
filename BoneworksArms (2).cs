using MelonLoader;
using BoneLib;
using BoneLib.BoneMenu;
using BoneLib.Notifications;
using UnityEngine;
using Il2CppSLZ.Marrow;
using HarmonyLib;

[assembly: MelonInfo(typeof(BoneworksArms.BoneworksArmsMod), "Pony's BW Rig", "1.0.0", "Pony")]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]

namespace BoneworksArms
{
    public class BoneworksArmsMod : MelonMod
    {
        private static bool _modEnabled = true;

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Pony's BW Rig loaded!");
            Hooking.OnLevelLoaded += OnLevelLoaded;
            SetupBoneMenu();
        }

        private void SetupBoneMenu()
        {
            var category = Menu.CreateCategory("Pony's BW Rig", Color.cyan);
            category.CreateBoolElement("BW Arm Colliders", Color.white, _modEnabled, (val) =>
            {
                _modEnabled = val;
                if (_modEnabled)
                {
                    ApplyArmColliders();
                    Notifier.Send(new Notification()
                    {
                        Title = NotificationText.ToNotificationText("Pony's BW Rig"),
                        Message = NotificationText.ToNotificationText("BW Rig ENABLED — Arm colliders removed!"),
                        Type = NotificationType.Success,
                        PopupLength = 2.5f,
                        ShowTitleOnPopup = true
                    });
                }
                else
                {
                    RestoreArmColliders();
                    Notifier.Send(new Notification()
                    {
                        Title = NotificationText.ToNotificationText("Pony's BW Rig"),
                        Message = NotificationText.ToNotificationText("BW Rig DISABLED — Arm colliders restored."),
                        Type = NotificationType.Warning,
                        PopupLength = 2.5f,
                        ShowTitleOnPopup = true
                    });
                }
            });

            category.CreateFunctionElement("Re-Apply", Color.green, () =>
            {
                if (_modEnabled)
                {
                    ApplyArmColliders();
                    Notifier.Send(new Notification()
                    {
                        Title = NotificationText.ToNotificationText("Pony's BW Rig"),
                        Message = NotificationText.ToNotificationText("Re-applied! Arm colliders removed."),
                        Type = NotificationType.Success,
                        PopupLength = 2f,
                        ShowTitleOnPopup = true
                    });
                }
            });
        }

        private static void OnLevelLoaded(LevelInfo levelInfo)
        {
            if (_modEnabled)
                ApplyArmColliders();
        }

        private static void ApplyArmColliders()
        {
            var rigManager = Player.RigManager;
            if (rigManager == null) return;

            var physicsRig = rigManager.physicsRig;
            if (physicsRig == null) return;

            DisableHandColliders(physicsRig.leftHand);
            DisableHandColliders(physicsRig.rightHand);
            DisableTransformColliders(physicsRig.m_shoulderLf);
            DisableTransformColliders(physicsRig.m_shoulderRt);

            MelonLogger.Msg("[Pony's BW Rig] Arm colliders disabled.");
        }

        private static void DisableHandColliders(PhysHand hand)
        {
            if (hand == null) return;
            var colliders = hand.GetComponents<Collider>();
            foreach (var col in colliders)
            {
                if (col != null)
                    col.isTrigger = true;
            }
            for (int i = 0; i < hand.transform.childCount; i++)
            {
                var child = hand.transform.GetChild(i);
                var childColliders = child.GetComponents<Collider>();
                foreach (var col in childColliders)
                {
                    if (col != null)
                        col.isTrigger = true;
                }
            }
        }

        private static void DisableTransformColliders(Transform t)
        {
            if (t == null) return;
            var colliders = t.GetComponents<Collider>();
            foreach (var col in colliders)
            {
                if (col != null)
                    col.isTrigger = true;
            }
        }

        private static void RestoreArmColliders()
        {
            var rigManager = Player.RigManager;
            if (rigManager == null) return;

            var physicsRig = rigManager.physicsRig;
            if (physicsRig == null) return;

            RestoreHandColliders(physicsRig.leftHand);
            RestoreHandColliders(physicsRig.rightHand);
            RestoreTransformColliders(physicsRig.m_shoulderLf);
            RestoreTransformColliders(physicsRig.m_shoulderRt);

            MelonLogger.Msg("[Pony's BW Rig] Arm colliders restored.");
        }

        private static void RestoreHandColliders(PhysHand hand)
        {
            if (hand == null) return;
            var colliders = hand.GetComponents<Collider>();
            foreach (var col in colliders)
            {
                if (col != null)
                    col.isTrigger = false;
            }
            for (int i = 0; i < hand.transform.childCount; i++)
            {
                var child = hand.transform.GetChild(i);
                var childColliders = child.GetComponents<Collider>();
                foreach (var col in childColliders)
                {
                    if (col != null)
                        col.isTrigger = false;
                }
            }
        }

        private static void RestoreTransformColliders(Transform t)
        {
            if (t == null) return;
            var colliders = t.GetComponents<Collider>();
            foreach (var col in colliders)
            {
                if (col != null)
                    col.isTrigger = false;
            }
        }
    }
}
