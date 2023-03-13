using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonForge
{
    public interface IUndoInteraction
    {
        public void DeleteLastSavedRoom();
    }
}