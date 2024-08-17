// Decompiled with JetBrains decompiler
// Type: ResearchHistory.ResearchHistory
// Assembly: ResearchHistory, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 538D0BD4-7E6B-47F4-AD54-96E249EB7830
// Assembly location: F:\Jeux\Steam\steamapps\workshop\content\294100\2688698217\1.3\Assemblies\ResearchHistory.dll

using System.Collections.Generic;
using Verse;

#nullable disable
namespace ResearchHistory
{
  public class ResearchHistory : GameComponent
  {
    public static Dictionary<string, ProjectHistory> projectsCompleted;
    public static Dictionary<string, ProjectHistory> projectsStarted;

    public ResearchHistory(Game game)
    {
      ResearchHistory.projectsCompleted = new Dictionary<string, ProjectHistory>();
      ResearchHistory.projectsStarted = new Dictionary<string, ProjectHistory>();
    }

    public override void FinalizeInit()
    {
      base.FinalizeInit();
      Window_ResearchHistory.ResetHistory();
    }

    public override void ExposeData()
    {
      base.ExposeData();
      Scribe_Collections.Look<string, ProjectHistory>(ref ResearchHistory.projectsCompleted, "projectsCompleted", LookMode.Value, LookMode.Deep);
      Scribe_Collections.Look<string, ProjectHistory>(ref ResearchHistory.projectsStarted, "projectsStarted", LookMode.Value, LookMode.Deep);
    }
  }
}
