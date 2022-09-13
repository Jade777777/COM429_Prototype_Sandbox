using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JaredGoronkinPrototype2
{
    public interface IPawn
    {
        public void OnMainPhaseStart();
        public void OnMainPhaseEnd();
        public void OnCombatPhaseStart();
        public void OnCombatPhaseEnd();
    }
}