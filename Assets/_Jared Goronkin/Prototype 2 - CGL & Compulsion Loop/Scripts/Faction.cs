using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JaredGoronkinPrototype2
{
    public class Faction
    {
        private static List<Faction> factions = new();
        public static IList<Faction> Factions => factions.AsReadOnly();
        public string Name { get; private set; }
        public Faction(string name)
        {
            Name = name;
            factions.Add(this);
        }
    }
}