
namespace DungeonForge.Editor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using DungeonForge.AlgoScript;

    [Serializable]
    public class LSystemRuleObj : ScriptableObject
    {
        [Header("L-System ruleset\nA = move 10 blocks\nB = move 15 blocks\nC = move 20 blocks\nS = ave current position in stack\nL = load last position from stack\n+ = turn clockwise\n- = turn anti-clockwise\n")]

        [Space(20)]
        public int A_Length = 10;
        public int B_Length = 20;
        public int C_Length = 30;

        [Space(10)]
        public List<string> A_RuleSet = new List<string>();
        public List<string> B_RuleSet = new List<string>();
        public List<string> C_RuleSet = new List<string>();
        public List<string> S_RuleSet = new List<string>();
        public List<string> L_RuleSet = new List<string>();
        public List<string> positiveSignRuleSet = new List<string>();
        public List<string> negativeSignRuleSet = new List<string>();
        [Space(10)]
        public List<LSystem.LSystemMacrosBuildings> roomGenerationMacros = new List<LSystem.LSystemMacrosBuildings>();
    }

}