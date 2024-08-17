// Decompiled with JetBrains decompiler
// Type: ResearchHistory.Patch_PawnDialogInfo
// Assembly: ResearchHistory, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 538D0BD4-7E6B-47F4-AD54-96E249EB7830
// Assembly location: F:\Jeux\Steam\steamapps\workshop\content\294100\2688698217\1.3\Assemblies\ResearchHistory.dll

using HarmonyLib;
using UnityEngine;
using Verse;

#nullable disable
namespace ResearchHistory
{
  [HarmonyPatch]
  public class Patch_PawnDialogInfo
  {
    [HarmonyPatch(typeof (Dialog_InfoCard), "DoWindowContents")]
    [HarmonyPostfix]
    public static void PawnResearchHistory(Rect inRect, Thing ___thing)
    {
      if (___thing == null || !(___thing is Pawn pawn) || !pawn.IsColonist || !Widgets.ButtonText(new Rect(inRect.xMax - 150f, 18f, 120f, 30f), "History", true, true, true))
        return;
      if (Window_ResearchHistory.researchers.Contains(pawn.LabelShort))
        Window_ResearchHistory.selPawn = pawn.LabelShort;
      Find.WindowStack.Add((Window) new Window_ResearchHistory());
    }
  }
}
