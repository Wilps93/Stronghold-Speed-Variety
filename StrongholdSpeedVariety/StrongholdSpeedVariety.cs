using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MelonLoader;

namespace StrongholdSpeedVariety
{
    class StrongholdSpeedVarietyMod : MelonMod
    {
        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("StrongholdSpeedVarietyMod has been loaded!");
        }
    }

    [HarmonyPatch(typeof(Director))]
    [HarmonyPatch("IncreaseFrameRate")]
    public static class GameSpeedPatchIncrease
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var found90 = false;
            var list = new List<CodeInstruction>();
            foreach (var instruction in instructions)
            {
                if (!found90 && instruction.opcode == OpCodes.Ldc_I4_S && (sbyte)instruction.operand == 90)
                {
                    list.Add(new CodeInstruction(OpCodes.Ldc_I4, 1000)); // Изменяем тип операнда на int
                    found90 = true;
                    MelonLogger.Msg("IncreaseFrameRate patched!");
                }
                else
                {
                    list.Add(instruction);
                }
            }
            if (!found90)
                throw new ArgumentException("Cannot find upper limit in Director.IncreaseFrameRate");
            return list;
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> InjectStep(IEnumerable<CodeInstruction> instructions)
        {
            var found5 = false;
            var list = new List<CodeInstruction>();
            foreach (var instruction in instructions)
            {
                if (!found5 && instruction.opcode == OpCodes.Ldc_R8 && (double)instruction.operand == 5.0)
                {
                    list.Add(new CodeInstruction(OpCodes.Ldc_R8, 5.0));
                    found5 = true;
                    MelonLogger.Msg("SetEngineFrameRate patched!");
                }
                else
                {
                    list.Add(instruction);
                }
            }
            if (!found5)
                throw new ArgumentException("Cannot find upper limit in Director.SetEngineFrameRate");
            return list;
        }
    }

    [HarmonyPatch(typeof(Director))]
    [HarmonyPatch("DecreaseFrameRate")]
    public static class GameSpeedPatchDecrease
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Decrease(IEnumerable<CodeInstruction> instructions)
        {
            var found5 = false;
            var list = new List<CodeInstruction>();
            foreach (var instruction in instructions)
            {
                if (!found5 && instruction.opcode == OpCodes.Ldc_R8 && (double)instruction.operand == 5.0)
                {
                    list.Add(new CodeInstruction(OpCodes.Ldc_R8, 5.0));
                    found5 = true;
                    MelonLogger.Msg("SetEngineFrameRate patched!");
                }
                else
                {
                    list.Add(instruction);
                }
            }
            if (!found5)
                throw new ArgumentException("Cannot find upper limit in Director.SetEngineFrameRate");
            return list;
        }   
    }

    [HarmonyPatch(typeof(Director))]
    [HarmonyPatch("SetEngineFrameRate")]
    public static class GameSpeedPatchSet
    {
        // Новый метод для вставки в IL-код
        private static IEnumerable<CodeInstruction> InjectEngineFrameRateLog(IEnumerable<CodeInstruction> instructions)
        {
            var list = new List<CodeInstruction>();
            foreach (var instruction in instructions)
            {
                list.Add(instruction);

                // Вставляем инструкцию для вывода в консоль после сохранения engineFrameTime
                if (instruction.opcode == OpCodes.Stfld && ((FieldInfo)instruction.operand).Name == "engineFrameTime")
                {
                    // Загружаем ссылку на текущий объект Director
                    list.Add(new CodeInstruction(OpCodes.Ldarg_0));
                    // Вызываем геттер EngineFrameRate
                    list.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(Director), "EngineFrameRate").GetGetMethod()));
                    // Преобразуем в строку
                    list.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Convert), "ToString", new Type[] { typeof(int) })));
                    // Вызываем MelonLogger.Msg для вывода в консоль
                    list.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MelonLogger), "Msg", new Type[] { typeof(string) })));
                }
            }
            return list;
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var found = false;
            var list = new List<CodeInstruction>();
            foreach (var instruction in instructions)
            {
                if (!found && instruction.opcode == OpCodes.Ldc_R8 && (double)instruction.operand == 90.0)
                {
                    list.Add(new CodeInstruction(OpCodes.Ldc_R8, 1000.0));
                    found = true;
                    MelonLogger.Msg("SetEngineFrameRate patched!");
                }
                else
                {
                    list.Add(instruction);
                }
            }
            if (!found)
                throw new ArgumentException("Cannot find upper limit in Director.SetEngineFrameRate");

            // Вставляем код для вывода engineFrameTime в консоль
            return InjectEngineFrameRateLog(list);
        }
    }
}
