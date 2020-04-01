using System;
using System.Reflection;
using Harmony;
using UnityEngine;

namespace ServiceVehicleSelector2.Util
{
    public static class PatchUtil
    {
        private const string HarmonyId = "github.com/bloodypenguin/Skylines-ServiceVehicleSelector";
        private static HarmonyInstance _harmonyInstance = null;

        private static HarmonyInstance HarmonyInstance =>
            _harmonyInstance ?? (_harmonyInstance = HarmonyInstance.Create(HarmonyId));

        public static void Patch(
            MethodDefinition original,
            MethodDefinition prefix = null,
            MethodDefinition postfix = null,
            MethodDefinition transpiler = null)
        {
            if (prefix == null && postfix == null && transpiler == null)
            {
                throw new Exception(
                    $"SVS2: prefix, postfix and transpiler are null for method {original.Type.FullName}.{original.MethodName}");
            }

            try
            {
                Debug.Log($"SVS2: Patching method {original.Type.FullName}.{original.MethodName}");
                var methodInfo = GetOriginal(original);
                HarmonyInstance.Patch(methodInfo,
                    prefix: prefix == null ? null : new HarmonyMethod(GetPatch(prefix)),
                    postfix: postfix == null ? null : new HarmonyMethod(GetPatch(postfix)),
                    transpiler == null ? null : new HarmonyMethod(GetPatch(transpiler))
                );
            }
            catch (Exception e)
            {
                Debug.LogError($"SVS2: Failed to patch method {original.Type.FullName}.{original.MethodName}");
                Debug.LogException(e);
            }
        }

        public static void Unpatch(MethodDefinition original)
        {
            Debug.Log($"SVS2: Unpatching method {original.Type.FullName}.{original.MethodName}");
            HarmonyInstance.Unpatch(GetOriginal(original), HarmonyPatchType.All, HarmonyId);
        }

        private static MethodInfo GetOriginal(MethodDefinition original)
        {
            var bindingFlags = original.BindingFlags == BindingFlags.Default
                ? BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static
                : original.BindingFlags;
            var methodInfo = original.Type.GetMethod(original.MethodName,
                bindingFlags);
            if (methodInfo == null)
            {
                throw new Exception(
                    $"SVS2: Failed to find original method {original.Type.FullName}.{original.MethodName}");
            }

            return methodInfo;
        }

        private static MethodInfo GetPatch(MethodDefinition patch)
        {
            var bindingFlags = patch.BindingFlags == BindingFlags.Default
                ? BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static
                : patch.BindingFlags;
            var methodInfo = patch.Type.GetMethod(patch.MethodName,
                bindingFlags);
            if (methodInfo == null)
            {
                throw new Exception($"SVS2: Failed to find patch method {patch.Type.FullName}.{patch.MethodName}");
            }

            return methodInfo;
        }

        public class MethodDefinition
        {
            public MethodDefinition(Type type, string methodName, BindingFlags bindingFlags = BindingFlags.Default)
            {
                Type = type;
                MethodName = methodName;
                BindingFlags = bindingFlags;
            }

            public Type Type { get; }
            public string MethodName { get; }

            public BindingFlags BindingFlags { get; }
        }
    }
}