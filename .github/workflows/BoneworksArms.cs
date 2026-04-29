using MelonLoader;
using BoneLib;
using BoneLib.BoneMenu;
using BoneLib.Notifications;
using UnityEngine;
using SLZ.Marrow.Warehouse;
using SLZ.VRMK;
using SLZ.Rig;
using System.Collections.Generic;

[assembly: MelonInfo(typeof(BoneworksArms.BoneworksArmsMod), "BoneworksArms", "1.0.0", "YourName")]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]

namespace BoneworksArms
{
    public class BoneworksArmsMod : MelonMod
    {
        private static bool _modEnabled = true;
        private static RigManager _cachedRigManager;
        private static readonly List<Collider> _disabledColliders = new List<Collider>();

        // Arm bone names used in BONELAB's rig
        private static readonly string[] ArmBoneNames = new string[]
        {
            "l_upperarm", "l_forearm", "l_hand",
            "r_upperarm", "r_forearm", "r_hand",
            "l_shoulder", "r_shoulder"
        };

        public override void OnInitializeMelon()
{
    try
    {
        LoggerInstance.Msg("Pony's BW Rig loaded!");
        Hooking.OnLevelLoaded += OnLevelLoaded;
        SetupBoneMenu();
        LoggerInstance.Msg("Pony's BW Rig initialized successfully!");
    }
    catch (System.Exception e)
    {
        LoggerInstance.Error("Pony's BW Rig failed to initialize: " + e.Message);
        LoggerInstance.Error(e.StackTrace);
    }
}
        }

        private void SetupBoneMenu()
        {
            var category = Menu.CreateCategory("Pony's BW Rig", Color.cyan);
            category.CreateBoolElement("Disable Arm Colliders", Color.white, _modEnabled, (val) =>
            {
                _modEnabled = val;
                if (_modEnabled)
                {
                    DisableArmColliders();
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
                _disabledColliders.Clear();
                if (_modEnabled)
                {
                    DisableArmColliders();
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

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            // Reset cached state on scene change
            _cachedRigManager = null;
            _disabledColliders.Clear();
        }

        public override void OnUpdate()
        {
            // Wait until RigManager is available, then apply
            if (_modEnabled && _cachedRigManager == null)
            {
                var rig = RigData.RigReferences.RigManager;
                if (rig != null)
                {
                    _cachedRigManager = rig;
                    DisableArmColliders();
                }
            }
        }

        private static void DisableArmColliders()
        {
            if (_cachedRigManager == null) return;

            _disabledColliders.Clear();

            // Get the physical rig root — this is where body colliders live
            var physicsRoot = _cachedRigManager.physicsRig?.transform;
            if (physicsRoot == null) return;

            foreach (var boneName in ArmBoneNames)
            {
                // Case-insensitive search through all children
                var bone = FindChildContains(physicsRoot, boneName);
                if (bone == null) continue;

                // Disable colliders on the bone itself
                foreach (var col in bone.GetComponents<Collider>())
                {
                    if (col == null) continue;
                    col.enabled = false;
                    _disabledColliders.Add(col);
                }
            }

            MelonLogger.Msg($"[BoneworksArms] Disabled {_disabledColliders.Count} arm collider(s).");
        }

        private static void RestoreArmColliders()
        {
            foreach (var col in _disabledColliders)
            {
                if (col != null)
                    col.enabled = true;
            }
            _disabledColliders.Clear();
            MelonLogger.Msg("[BoneworksArms] Arm colliders restored.");
        }

        /// <summary>
        /// Searches all children of a transform for a name containing the keyword (case-insensitive).
        /// Safe for IL2CPP / Quest.
        /// </summary>
        private static Transform FindChildContains(Transform root, string keyword)
        {
            for (int i = 0; i < root.childCount; i++)
            {
                var child = root.GetChild(i);
                if (child.name.ToLower().Contains(keyword.ToLower()))
                    return child;

                var found = FindChildContains(child, keyword);
                if (found != null)
                    return found;
            }
            return null;
        }
    }
}
