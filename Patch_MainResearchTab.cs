// Decompiled with JetBrains decompiler
// Type: ResearchHistory.Patch_MainResearchTab
// Assembly: ResearchHistory, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 538D0BD4-7E6B-47F4-AD54-96E249EB7830
// Assembly location: F:\Jeux\Steam\steamapps\workshop\content\294100\2688698217\1.3\Assemblies\ResearchHistory.dll

using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

#nullable disable
namespace ResearchHistory
{
  [HarmonyPatch]
  public class Patch_MainResearchTab
  {
    [HarmonyPatch(typeof (MainTabWindow_Research), "DrawLeftRect")]
    [HarmonyPostfix]
    public static void OpenResearchHistory(Rect leftOutRect)
    {
      if (!Widgets.ButtonText(new Rect(leftOutRect.xMax - 120f, 0.0f, 120f, 30f), "History", true, true, true))
        return;
      Window_ResearchHistory.selPawn = (string) "ResTime_AllResearchers".Translate();
      Find.WindowStack.Add((Window) new Window_ResearchHistory());
    }

    [HarmonyPatch(typeof (MainTabWindow_Research), "DrawLeftRect")]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
      FieldInfo selField = AccessTools.Field(typeof (MainTabWindow_Research), "selectedProject");
      List<CodeInstruction> list = instructions.ToList<CodeInstruction>();
      int i = 0;
      while (i < list.Count)
      {
        CodeInstruction code = list[i];
        if (code.Is(OpCodes.Ldstr, (object) "Finished") && CodeInstructionExtensions.Is(list[i - 3], OpCodes.Ldfld, (MemberInfo) selField))
        {
          yield return new CodeInstruction(OpCodes.Ldarg_0);
          yield return new CodeInstruction(OpCodes.Ldfld, (object) selField);
          yield return new CodeInstruction(OpCodes.Callvirt, (object) AccessTools.Method(typeof (Patch_MainResearchTab), "NewFinishedString"));
          i += 2;
        }
        else
          yield return code;
        ++i;
        code = (CodeInstruction) null;
      }
    }

    public static string NewFinishedString(ResearchProjectDef project)
    {
      string str = (string) "Finished".Translate();
      if (ResearchHistory.projectsCompleted.ContainsKey((string) project.LabelCap))
      {
        ProjectHistory projectHistory = ResearchHistory.projectsCompleted[(string) project.LabelCap];
        if (projectHistory.startingTech)
          str = (string) "ResTime_Starting".Translate();
        else if (projectHistory.finalResearcher != null)
          str = string.Format((string) "ResTime_Finished".Translate(), (object) projectHistory.finalResearcher);
      }
      return str;
    }
  }
}
