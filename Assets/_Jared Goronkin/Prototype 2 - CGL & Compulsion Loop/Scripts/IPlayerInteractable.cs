using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JaredGoronkinPrototype2
{
    public interface IPlayerInteractable
    {
        public void HoverEnd(Faction faction);
        public void HoverStart(Faction faction);
        public void OpenInteractionMenu(Faction faction);
        public void Select(Faction faction);
    }
}