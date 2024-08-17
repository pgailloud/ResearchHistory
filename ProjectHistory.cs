// Decompiled with JetBrains decompiler
// Type: ResearchHistory.ProjectHistory
// Assembly: ResearchHistory, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 538D0BD4-7E6B-47F4-AD54-96E249EB7830
// Assembly location: F:\Jeux\Steam\steamapps\workshop\content\294100\2688698217\1.3\Assemblies\ResearchHistory.dll

using System.Collections.Generic;
using Verse;

#nullable disable
namespace ResearchHistory
{
  public class ProjectHistory : IExposable
  {
    public string date;
    public bool startingTech = false;
    public string finalResearcher;
    public HashSet<string> contributors = new HashSet<string>();

    public void ExposeData()
    {
      Scribe_Values.Look<string>(ref this.date, "date");
      Scribe_Values.Look<bool>(ref this.startingTech, "startingTech");
      Scribe_Values.Look<string>(ref this.finalResearcher, "finalResearcher");
      Scribe_Collections.Look<string>(ref this.contributors, "contributors");
    }
  }
}
