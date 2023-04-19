using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;


namespace DungeonForge.Utils
{
    public static class DFAlgoBank
    {
        #region marching Cubes Rule

        //http://paulbourke.net/geometry/polygonise/
        /// <summary>
        /// ruleSet used by the marhcing cubes algorithm
        /// </summary>
        public static int[,] triTable = new int[256, 16]
    {{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 8, 3, 9, 8, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 2, 10, 0, 2, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{2, 8, 3, 2, 10, 8, 10, 9, 8, -1, -1, -1, -1, -1, -1, -1},
{3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 11, 2, 8, 11, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 9, 0, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 11, 2, 1, 9, 11, 9, 8, 11, -1, -1, -1, -1, -1, -1, -1},
{3, 10, 1, 11, 10, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 10, 1, 0, 8, 10, 8, 11, 10, -1, -1, -1, -1, -1, -1, -1},
{3, 9, 0, 3, 11, 9, 11, 10, 9, -1, -1, -1, -1, -1, -1, -1},
{9, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 3, 0, 7, 3, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 9, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 1, 9, 4, 7, 1, 7, 3, 1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 4, 7, 3, 0, 4, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1},
{9, 2, 10, 9, 0, 2, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
{2, 10, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4, -1, -1, -1, -1},
{8, 4, 7, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{11, 4, 7, 11, 2, 4, 2, 0, 4, -1, -1, -1, -1, -1, -1, -1},
{9, 0, 1, 8, 4, 7, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
{4, 7, 11, 9, 4, 11, 9, 11, 2, 9, 2, 1, -1, -1, -1, -1},
{3, 10, 1, 3, 11, 10, 7, 8, 4, -1, -1, -1, -1, -1, -1, -1},
{1, 11, 10, 1, 4, 11, 1, 0, 4, 7, 11, 4, -1, -1, -1, -1},
{4, 7, 8, 9, 0, 11, 9, 11, 10, 11, 0, 3, -1, -1, -1, -1},
{4, 7, 11, 4, 11, 9, 9, 11, 10, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 4, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 5, 4, 1, 5, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{8, 5, 4, 8, 3, 5, 3, 1, 5, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 8, 1, 2, 10, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
{5, 2, 10, 5, 4, 2, 4, 0, 2, -1, -1, -1, -1, -1, -1, -1},
{2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8, -1, -1, -1, -1},
{9, 5, 4, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 11, 2, 0, 8, 11, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
{0, 5, 4, 0, 1, 5, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
{2, 1, 5, 2, 5, 8, 2, 8, 11, 4, 8, 5, -1, -1, -1, -1},
{10, 3, 11, 10, 1, 3, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1},
{4, 9, 5, 0, 8, 1, 8, 10, 1, 8, 11, 10, -1, -1, -1, -1},
{5, 4, 0, 5, 0, 11, 5, 11, 10, 11, 0, 3, -1, -1, -1, -1},
{5, 4, 8, 5, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1},
{9, 7, 8, 5, 7, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 3, 0, 9, 5, 3, 5, 7, 3, -1, -1, -1, -1, -1, -1, -1},
{0, 7, 8, 0, 1, 7, 1, 5, 7, -1, -1, -1, -1, -1, -1, -1},
{1, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 7, 8, 9, 5, 7, 10, 1, 2, -1, -1, -1, -1, -1, -1, -1},
{10, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3, -1, -1, -1, -1},
{8, 0, 2, 8, 2, 5, 8, 5, 7, 10, 5, 2, -1, -1, -1, -1},
{2, 10, 5, 2, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1},
{7, 9, 5, 7, 8, 9, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 11, -1, -1, -1, -1},
{2, 3, 11, 0, 1, 8, 1, 7, 8, 1, 5, 7, -1, -1, -1, -1},
{11, 2, 1, 11, 1, 7, 7, 1, 5, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 8, 8, 5, 7, 10, 1, 3, 10, 3, 11, -1, -1, -1, -1},
{5, 7, 0, 5, 0, 9, 7, 11, 0, 1, 0, 10, 11, 10, 0, -1},
{11, 10, 0, 11, 0, 3, 10, 5, 0, 8, 0, 7, 5, 7, 0, -1},
{11, 10, 5, 7, 11, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 0, 1, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 8, 3, 1, 9, 8, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
{1, 6, 5, 2, 6, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 6, 5, 1, 2, 6, 3, 0, 8, -1, -1, -1, -1, -1, -1, -1},
{9, 6, 5, 9, 0, 6, 0, 2, 6, -1, -1, -1, -1, -1, -1, -1},
{5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8, -1, -1, -1, -1},
{2, 3, 11, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{11, 0, 8, 11, 2, 0, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 9, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
{5, 10, 6, 1, 9, 2, 9, 11, 2, 9, 8, 11, -1, -1, -1, -1},
{6, 3, 11, 6, 5, 3, 5, 1, 3, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 11, 0, 11, 5, 0, 5, 1, 5, 11, 6, -1, -1, -1, -1},
{3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9, -1, -1, -1, -1},
{6, 5, 9, 6, 9, 11, 11, 9, 8, -1, -1, -1, -1, -1, -1, -1},
{5, 10, 6, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 3, 0, 4, 7, 3, 6, 5, 10, -1, -1, -1, -1, -1, -1, -1},
{1, 9, 0, 5, 10, 6, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
{10, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4, -1, -1, -1, -1},
{6, 1, 2, 6, 5, 1, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7, -1, -1, -1, -1},
{8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6, -1, -1, -1, -1},
{7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9, -1},
{3, 11, 2, 7, 8, 4, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
{5, 10, 6, 4, 7, 2, 4, 2, 0, 2, 7, 11, -1, -1, -1, -1},
{0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1},
{9, 2, 1, 9, 11, 2, 9, 4, 11, 7, 11, 4, 5, 10, 6, -1},
{8, 4, 7, 3, 11, 5, 3, 5, 1, 5, 11, 6, -1, -1, -1, -1},
{5, 1, 11, 5, 11, 6, 1, 0, 11, 7, 11, 4, 0, 4, 11, -1},
{0, 5, 9, 0, 6, 5, 0, 3, 6, 11, 6, 3, 8, 4, 7, -1},
{6, 5, 9, 6, 9, 11, 4, 7, 9, 7, 11, 9, -1, -1, -1, -1},
{10, 4, 9, 6, 4, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 10, 6, 4, 9, 10, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1},
{10, 0, 1, 10, 6, 0, 6, 4, 0, -1, -1, -1, -1, -1, -1, -1},
{8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 10, -1, -1, -1, -1},
{1, 4, 9, 1, 2, 4, 2, 6, 4, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4, -1, -1, -1, -1},
{0, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{8, 3, 2, 8, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1},
{10, 4, 9, 10, 6, 4, 11, 2, 3, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 2, 2, 8, 11, 4, 9, 10, 4, 10, 6, -1, -1, -1, -1},
{3, 11, 2, 0, 1, 6, 0, 6, 4, 6, 1, 10, -1, -1, -1, -1},
{6, 4, 1, 6, 1, 10, 4, 8, 1, 2, 1, 11, 8, 11, 1, -1},
{9, 6, 4, 9, 3, 6, 9, 1, 3, 11, 6, 3, -1, -1, -1, -1},
{8, 11, 1, 8, 1, 0, 11, 6, 1, 9, 1, 4, 6, 4, 1, -1},
{3, 11, 6, 3, 6, 0, 0, 6, 4, -1, -1, -1, -1, -1, -1, -1},
{6, 4, 8, 11, 6, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{7, 10, 6, 7, 8, 10, 8, 9, 10, -1, -1, -1, -1, -1, -1, -1},
{0, 7, 3, 0, 10, 7, 0, 9, 10, 6, 7, 10, -1, -1, -1, -1},
{10, 6, 7, 1, 10, 7, 1, 7, 8, 1, 8, 0, -1, -1, -1, -1},
{10, 6, 7, 10, 7, 1, 1, 7, 3, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7, -1, -1, -1, -1},
{2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9, -1},
{7, 8, 0, 7, 0, 6, 6, 0, 2, -1, -1, -1, -1, -1, -1, -1},
{7, 3, 2, 6, 7, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{2, 3, 11, 10, 6, 8, 10, 8, 9, 8, 6, 7, -1, -1, -1, -1},
{2, 0, 7, 2, 7, 11, 0, 9, 7, 6, 7, 10, 9, 10, 7, -1},
{1, 8, 0, 1, 7, 8, 1, 10, 7, 6, 7, 10, 2, 3, 11, -1},
{11, 2, 1, 11, 1, 7, 10, 6, 1, 6, 7, 1, -1, -1, -1, -1},
{8, 9, 6, 8, 6, 7, 9, 1, 6, 11, 6, 3, 1, 3, 6, -1},
{0, 9, 1, 11, 6, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{7, 8, 0, 7, 0, 6, 3, 11, 0, 11, 6, 0, -1, -1, -1, -1},
{7, 11, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 8, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 9, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{8, 1, 9, 8, 3, 1, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
{10, 1, 2, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 3, 0, 8, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
{2, 9, 0, 2, 10, 9, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
{6, 11, 7, 2, 10, 3, 10, 8, 3, 10, 9, 8, -1, -1, -1, -1},
{7, 2, 3, 6, 2, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{7, 0, 8, 7, 6, 0, 6, 2, 0, -1, -1, -1, -1, -1, -1, -1},
{2, 7, 6, 2, 3, 7, 0, 1, 9, -1, -1, -1, -1, -1, -1, -1},
{1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6, -1, -1, -1, -1},
{10, 7, 6, 10, 1, 7, 1, 3, 7, -1, -1, -1, -1, -1, -1, -1},
{10, 7, 6, 1, 7, 10, 1, 8, 7, 1, 0, 8, -1, -1, -1, -1},
{0, 3, 7, 0, 7, 10, 0, 10, 9, 6, 10, 7, -1, -1, -1, -1},
{7, 6, 10, 7, 10, 8, 8, 10, 9, -1, -1, -1, -1, -1, -1, -1},
{6, 8, 4, 11, 8, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 6, 11, 3, 0, 6, 0, 4, 6, -1, -1, -1, -1, -1, -1, -1},
{8, 6, 11, 8, 4, 6, 9, 0, 1, -1, -1, -1, -1, -1, -1, -1},  
{9, 4, 6, 9, 6, 3, 9, 3, 1, 11, 3, 6, -1, -1, -1, -1},
{6, 8, 4, 6, 11, 8, 2, 10, 1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 3, 0, 11, 0, 6, 11, 0, 4, 6, -1, -1, -1, -1},
{4, 11, 8, 4, 6, 11, 0, 2, 9, 2, 10, 9, -1, -1, -1, -1},
{10, 9, 3, 10, 3, 2, 9, 4, 3, 11, 3, 6, 4, 6, 3, -1},
{8, 2, 3, 8, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1},
{0, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8, -1, -1, -1, -1},
{1, 9, 4, 1, 4, 2, 2, 4, 6, -1, -1, -1, -1, -1, -1, -1},
{8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 10, 1, -1, -1, -1, -1},
{10, 1, 0, 10, 0, 6, 6, 0, 4, -1, -1, -1, -1, -1, -1, -1},
{4, 6, 3, 4, 3, 8, 6, 10, 3, 0, 3, 9, 10, 9, 3, -1},
{10, 9, 4, 6, 10, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 9, 5, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 4, 9, 5, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
{5, 0, 1, 5, 4, 0, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
{11, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5, -1, -1, -1, -1},
{9, 5, 4, 10, 1, 2, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
{6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5, -1, -1, -1, -1},
{7, 6, 11, 5, 4, 10, 4, 2, 10, 4, 0, 2, -1, -1, -1, -1},
{3, 4, 8, 3, 5, 4, 3, 2, 5, 10, 5, 2, 11, 7, 6, -1},
{7, 2, 3, 7, 6, 2, 5, 4, 9, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7, -1, -1, -1, -1},
{3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0, -1, -1, -1, -1},
{6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8, -1},
{9, 5, 4, 10, 1, 6, 1, 7, 6, 1, 3, 7, -1, -1, -1, -1},
{1, 6, 10, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4, -1},
{4, 0, 10, 4, 10, 5, 0, 3, 10, 6, 10, 7, 3, 7, 10, -1},
{7, 6, 10, 7, 10, 8, 5, 4, 10, 4, 8, 10, -1, -1, -1, -1},
{6, 9, 5, 6, 11, 9, 11, 8, 9, -1, -1, -1, -1, -1, -1, -1},
{3, 6, 11, 0, 6, 3, 0, 5, 6, 0, 9, 5, -1, -1, -1, -1},
{0, 11, 8, 0, 5, 11, 0, 1, 5, 5, 6, 11, -1, -1, -1, -1},
{6, 11, 3, 6, 3, 5, 5, 3, 1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 9, 5, 11, 9, 11, 8, 11, 5, 6, -1, -1, -1, -1},
{0, 11, 3, 0, 6, 11, 0, 9, 6, 5, 6, 9, 1, 2, 10, -1},
{11, 8, 5, 11, 5, 6, 8, 0, 5, 10, 5, 2, 0, 2, 5, -1},
{6, 11, 3, 6, 3, 5, 2, 10, 3, 10, 5, 3, -1, -1, -1, -1},
{5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2, -1, -1, -1, -1},
{9, 5, 6, 9, 6, 0, 0, 6, 2, -1, -1, -1, -1, -1, -1, -1},
{1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8, -1},
{1, 5, 6, 2, 1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 3, 6, 1, 6, 10, 3, 8, 6, 5, 6, 9, 8, 9, 6, -1},
{10, 1, 0, 10, 0, 6, 9, 5, 0, 5, 6, 0, -1, -1, -1, -1},
{0, 3, 8, 5, 6, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{10, 5, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{11, 5, 10, 7, 5, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{11, 5, 10, 11, 7, 5, 8, 3, 0, -1, -1, -1, -1, -1, -1, -1},
{5, 11, 7, 5, 10, 11, 1, 9, 0, -1, -1, -1, -1, -1, -1, -1},
{10, 7, 5, 10, 11, 7, 9, 8, 1, 8, 3, 1, -1, -1, -1, -1},
{11, 1, 2, 11, 7, 1, 7, 5, 1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 11, -1, -1, -1, -1},
{9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 11, 7, -1, -1, -1, -1},
{7, 5, 2, 7, 2, 11, 5, 9, 2, 3, 2, 8, 9, 8, 2, -1},
{2, 5, 10, 2, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1},
{8, 2, 0, 8, 5, 2, 8, 7, 5, 10, 2, 5, -1, -1, -1, -1},
{9, 0, 1, 5, 10, 3, 5, 3, 7, 3, 10, 2, -1, -1, -1, -1},
{9, 8, 2, 9, 2, 1, 8, 7, 2, 10, 2, 5, 7, 5, 2, -1},
{1, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 7, 0, 7, 1, 1, 7, 5, -1, -1, -1, -1, -1, -1, -1},
{9, 0, 3, 9, 3, 5, 5, 3, 7, -1, -1, -1, -1, -1, -1, -1},
{9, 8, 7, 5, 9, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{5, 8, 4, 5, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1},
{5, 0, 4, 5, 11, 0, 5, 10, 11, 11, 3, 0, -1, -1, -1, -1},
{0, 1, 9, 8, 4, 10, 8, 10, 11, 10, 4, 5, -1, -1, -1, -1},
{10, 11, 4, 10, 4, 5, 11, 3, 4, 9, 4, 1, 3, 1, 4, -1},
{2, 5, 1, 2, 8, 5, 2, 11, 8, 4, 5, 8, -1, -1, -1, -1},
{0, 4, 11, 0, 11, 3, 4, 5, 11, 2, 11, 1, 5, 1, 11, -1},
{0, 2, 5, 0, 5, 9, 2, 11, 5, 4, 5, 8, 11, 8, 5, -1},
{9, 4, 5, 2, 11, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{2, 5, 10, 3, 5, 2, 3, 4, 5, 3, 8, 4, -1, -1, -1, -1},
{5, 10, 2, 5, 2, 4, 4, 2, 0, -1, -1, -1, -1, -1, -1, -1},
{3, 10, 2, 3, 5, 10, 3, 8, 5, 4, 5, 8, 0, 1, 9, -1},
{5, 10, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2, -1, -1, -1, -1},
{8, 4, 5, 8, 5, 3, 3, 5, 1, -1, -1, -1, -1, -1, -1, -1},
{0, 4, 5, 1, 0, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5, -1, -1, -1, -1},
{9, 4, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 11, 7, 4, 9, 11, 9, 10, 11, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 4, 9, 7, 9, 11, 7, 9, 10, 11, -1, -1, -1, -1},
{1, 10, 11, 1, 11, 4, 1, 4, 0, 7, 4, 11, -1, -1, -1, -1},
{3, 1, 4, 3, 4, 8, 1, 10, 4, 7, 4, 11, 10, 11, 4, -1},
{4, 11, 7, 9, 11, 4, 9, 2, 11, 9, 1, 2, -1, -1, -1, -1},
{9, 7, 4, 9, 11, 7, 9, 1, 11, 2, 11, 1, 0, 8, 3, -1},
{11, 7, 4, 11, 4, 2, 2, 4, 0, -1, -1, -1, -1, -1, -1, -1},
{11, 7, 4, 11, 4, 2, 8, 3, 4, 3, 2, 4, -1, -1, -1, -1},
{2, 9, 10, 2, 7, 9, 2, 3, 7, 7, 4, 9, -1, -1, -1, -1},
{9, 10, 7, 9, 7, 4, 10, 2, 7, 8, 7, 0, 2, 0, 7, -1},
{3, 7, 10, 3, 10, 2, 7, 4, 10, 1, 10, 0, 4, 0, 10, -1},
{1, 10, 2, 8, 7, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 9, 1, 4, 1, 7, 7, 1, 3, -1, -1, -1, -1, -1, -1, -1},
{4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1, -1, -1, -1, -1},
{4, 0, 3, 7, 4, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 8, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 9, 3, 9, 11, 11, 9, 10, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 10, 0, 10, 8, 8, 10, 11, -1, -1, -1, -1, -1, -1, -1},
{3, 1, 10, 11, 3, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 11, 1, 11, 9, 9, 11, 8, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 9, 3, 9, 11, 1, 2, 9, 2, 11, 9, -1, -1, -1, -1},
{0, 2, 11, 8, 0, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 2, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{2, 3, 8, 2, 8, 10, 10, 8, 9, -1, -1, -1, -1, -1, -1, -1},
{9, 10, 2, 0, 9, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{2, 3, 8, 2, 8, 10, 0, 1, 8, 1, 10, 8, -1, -1, -1, -1},
{1, 10, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 3, 8, 9, 1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 3, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

        #endregion

        #region PathFinding

        #region A*
        /// <summary>
        /// Run the pathfinding algorithm on the grid
        /// </summary>
        /// <param name="tileArray2D">grid reference</param>
        /// <param name="start">starting coordinate</param>
        /// <param name="end">ending coordinate</param>
        /// <param name="euclideanDis">us ehte euclidian distance calculation = true, or manhattan = false</param>
        /// <param name="diagonalTiles">allow diagonal pathing</param>
        /// <param name="useWeights">use the weights</param>
        /// <param name="arrWeights">given weight ruleset</param>
        /// <returns></returns>
        public static Tuple<List<DFTile>, List<DFTile>> A_StarPathfinding2D(DFTile[,] tileArray2D, Vector2Int start, Vector2Int end, bool euclideanDis = true, bool diagonalTiles = false, bool useWeights = false, float[] arrWeights = null)
        {
            bool checkForUse = useWeights == true && arrWeights != null ? true : false;

            List<AStar_Node> openList = new List<AStar_Node>();
            List<AStar_Node> closedList = new List<AStar_Node>();

            AStar_Node start_node = new AStar_Node(tileArray2D[start.x, start.y]);
            start_node.parent = null;

            AStar_Node end_node = new AStar_Node(tileArray2D[end.x, end.y]);

            int[,] childPosArry = new int[0, 0];

            if (diagonalTiles)
                childPosArry = DFGeneralUtil.childPosArry8Side;
            else
                childPosArry = DFGeneralUtil.childPosArry4Side;

            openList.Add(start_node);

            while (openList.Count > 0)
            {

                AStar_Node currNode = openList[0];
                int currIndex = 0;
                for (int i = 0; i < openList.Count; i++)
                {
                    if (openList[i].f < currNode.f)
                    {
                        currNode = openList[i];
                        currIndex = i;
                    }
                }

                openList.RemoveAt(currIndex);

                closedList.Add(currNode);

                if (currNode.refToBasicTile.position.x == end_node.refToBasicTile.position.x && currNode.refToBasicTile.position.y == end_node.refToBasicTile.position.y)
                {
                    List<AStar_Node> path = new List<AStar_Node>();

                    AStar_Node current = currNode;

                    while (current.parent != null)
                    {
                        path.Add(current);
                        current = current.parent;
                    }

                    var pathOfBasicTiles = new List<DFTile>();

                    foreach (var tile in path)
                    {
                        pathOfBasicTiles.Add(tile.refToBasicTile);
                    }

                    var allVisiteBasicTiles = new List<DFTile>();
                    foreach (var tile in openList)
                    {
                        allVisiteBasicTiles.Add(tile.refToBasicTile);
                    }

                    return new Tuple<List<DFTile>, List<DFTile>>(pathOfBasicTiles, allVisiteBasicTiles);
                }
                else
                {
                    List<AStar_Node> children = new List<AStar_Node>();

                    for (int i = 0; i < childPosArry.Length / 2; i++)
                    {
                        int x_buff = childPosArry[i, 0];
                        int y_buff = childPosArry[i, 1];

                        int[] node_position = { currNode.refToBasicTile.position.x + x_buff, currNode.refToBasicTile.position.y + y_buff };

                        if (node_position[0] < 0 || node_position[1] < 0 || node_position[0] >= tileArray2D.GetLength(0) || node_position[1] >= tileArray2D.GetLength(1))
                        {
                            continue;
                        }
                        else
                        {
                            //here an if statment also saying that walkable 
                            AStar_Node new_node = new AStar_Node(tileArray2D[node_position[0], node_position[1]]);
                            children.Add(new_node);
                        }
                    }

                    foreach (var child in children)
                    {

                        bool alreadyThere = false;

                        for (int i = 0; i < closedList.Count; i++)
                        {
                            if (child.refToBasicTile.position.x == closedList[i].refToBasicTile.position.x && child.refToBasicTile.position.y == closedList[i].refToBasicTile.position.y)
                            {
                                alreadyThere = true;
                                break;
                            }
                        }

                        if (alreadyThere == false)
                        {
                            child.g = currNode.g + 0.5f;

                            if (euclideanDis)
                                child.h = DFGeneralUtil.EuclideanDistance2D(new Vector2(end_node.refToBasicTile.position.x, end_node.refToBasicTile.position.y), new Vector2(child.refToBasicTile.position.x, child.refToBasicTile.position.y));
                            else
                                child.h = DFGeneralUtil.ManhattanDistance2D(new Vector2(end_node.refToBasicTile.position.x, end_node.refToBasicTile.position.y), new Vector2(child.refToBasicTile.position.x, child.refToBasicTile.position.y));

                            if (checkForUse)
                            {
                                child.f = child.g + child.h + arrWeights[(int)child.refToBasicTile.tileType];   //added value here
                                child.parent = currNode;
                            }
                            else
                            {
                                child.f = child.g + child.h;   //added value here
                                child.parent = currNode;
                            }

                            bool alreadyThereAgain = false;

                            foreach (var openListItem in openList)
                            {
                                if (child.refToBasicTile.position.x == openListItem.refToBasicTile.position.x && child.refToBasicTile.position.y == openListItem.refToBasicTile.position.y && child.g > openListItem.g)// 
                                {
                                    alreadyThereAgain = true;
                                    break;
                                }
                            }

                            if (alreadyThereAgain == false)
                                openList.Add(child);
                        }
                    }
                }
            }

            return null;
        }

        #endregion

        #region beizier
        public static Vector3 CubicBeizier(Vector2Int pos1, Vector2Int pos2, Vector2Int pos3, Vector2Int pos4, float t)
        {
            var correctedPos1 = new Vector3(pos1.x, 0, pos1.y);
            var correctedPos2 = new Vector3(pos2.x, 0, pos2.y);
            var correctedPos3 = new Vector3(pos3.x, 0, pos3.y);
            var correctedPos4 = new Vector3(pos4.x, 0, pos4.y);

            return (Mathf.Pow((1 - t), 3) * correctedPos1) + (3 * (Mathf.Pow((1 - t), 2)) * t * correctedPos2) + (3 * (1 - t) * t * t * correctedPos3) + t * t * t * correctedPos4;
        }

        public static Tuple<Vector2, Vector2> ExtrapolatePos(Vector2 startPos, Vector2 EndPos, int margin)
        {
            float lerpPoint2 = Random.Range(0.15f, 0.40f);
            float lerpPoint3 = Random.Range(0.60f, 0.80f);

            margin = Mathf.Abs(margin);

            Vector2 dir = startPos - EndPos;

            var normalised = Vector2.Perpendicular(dir).normalized;
            var point2 = Vector2.Lerp(startPos, EndPos, lerpPoint2);
            point2 = point2 + normalised * Random.Range(margin * -1, margin);


            normalised = Vector2.Perpendicular(dir).normalized;
            var point3 = Vector2.Lerp(startPos, EndPos, lerpPoint3);
            point3 = point3 + normalised * Random.Range(margin * -1, margin);


            return Tuple.Create(point2, point3);
        }

        public static void BezierCurvePathing(Vector2Int tileA, Vector2Int tileB, int margin, DFTile[,] gridArr)
        {
            var startPos = new Vector2Int(tileA.x, tileA.y);
            var endPos = new Vector2Int(tileB.x, tileB.y);

            var prevCoord = new Vector2Int(0, 0);

            var positions = ExtrapolatePos(startPos, endPos, margin);

            var mid1Pos = new Vector2Int((int)MathF.Round(positions.Item1.x), (int)MathF.Round(positions.Item1.y));
            var mid2Pos = new Vector2Int((int)MathF.Round(positions.Item2.x), (int)MathF.Round(positions.Item2.y));

            var firstBezierPoint = CubicBeizier(startPos, mid1Pos, mid2Pos, endPos, 0);

            var pathB = A_StarPathfinding2D(gridArr, startPos, new Vector2Int((int)MathF.Round(firstBezierPoint.x), (int)MathF.Round(firstBezierPoint.z)));

            SetUpCorridorWithPath(pathB.Item1);

            for (float t = 0; t < 1; t += 0.05f)
            {
                float currT = t;
                float prevT = t - 0.05f;

                var currCord = CubicBeizier(startPos, mid1Pos, mid2Pos, endPos, currT);

                if (prevT < 0)
                {
                    prevCoord = new Vector2Int((int)MathF.Round(currCord.x), (int)MathF.Round(currCord.z));
                    continue;
                }

                else if (currCord.x < 0 || currCord.z < 0 || currCord.x >= gridArr.GetLength(0) || currCord.z >= gridArr.GetLength(1))
                { continue; }

                if ((int)MathF.Round(currCord.x) < 0)
                {
                    currCord.x = 0;
                }
                if ((int)MathF.Round(currCord.x) >= gridArr.GetLength(0))
                {
                    currCord.x = gridArr.GetLength(0) - 1;
                }

                if ((int)MathF.Round(currCord.z) < 0)
                {
                    currCord.z = 0;
                }
                if ((int)MathF.Round(currCord.z) >= gridArr.GetLength(1))
                {
                    currCord.z = gridArr.GetLength(1) - 1;
                }

                pathB = A_StarPathfinding2D(gridArr, prevCoord, new Vector2Int((int)MathF.Round(currCord.x), (int)MathF.Round(currCord.z)));

                prevCoord = new Vector2Int((int)MathF.Round(currCord.x), (int)MathF.Round(currCord.z));

                SetUpCorridorWithPath(pathB.Item1);
            }

            pathB = A_StarPathfinding2D(gridArr, prevCoord, endPos);

            SetUpCorridorWithPath(pathB.Item1);
        }

        #endregion

        #region Dijstra

        public static List<DFTile> DijstraPathfinding(DFTile[,] gridArr, Vector2Int startPoint, Vector2Int endPoint, bool avoidWalls = false)
        {
            int[,] childPosArry = new int[0, 0];

            childPosArry = DFGeneralUtil.childPosArry4Side;

            List<DjNode> openListDjNodes = new List<DjNode>();
            DjNode[,] DjNodesArr = new DjNode[gridArr.GetLength(0), gridArr.GetLength(1)];

            for (int y = 0; y < gridArr.GetLength(1); y++)
            {
                for (int x = 0; x < gridArr.GetLength(0); x++)
                {
                    if (avoidWalls)
                    {
                        if (gridArr[x, y].tileType != DFTile.TileType.WALLCORRIDOR)
                        {
                            var newRef = new DjNode() { coord = new Vector2Int(x, y), distance = startPoint == new Vector2Int(x, y) ? 0 : 9999999, gridRefTile = gridArr[x, y], parentDJnode = null };

                            DjNodesArr[x, y] = newRef;
                            openListDjNodes.Add(newRef);
                        }
                    }
                    else
                    {
                        var newRef = new DjNode() { coord = new Vector2Int(x, y), distance = startPoint == new Vector2Int(x, y) ? 0 : 9999999, gridRefTile = gridArr[x, y], parentDJnode = null };

                        DjNodesArr[x, y] = newRef;
                        openListDjNodes.Add(newRef);
                    }
                }
            }

            DjNode lastNode = null;

            while (openListDjNodes.Count > 1)
            {
                float smallestDist = 99999999;
                DjNode currNode = null;

                foreach (var djNode in openListDjNodes)
                {
                    if (djNode.distance < smallestDist)
                    {
                        smallestDist = djNode.distance;
                        currNode = djNode;
                    }
                }

                openListDjNodes.Remove(currNode);

                for (int i = 0; i < childPosArry.Length / 2; i++)
                {
                    int x_buff = childPosArry[i, 0];
                    int y_buff = childPosArry[i, 1];

                    int[] node_position = { currNode.coord.x + x_buff, currNode.coord.y + y_buff };

                    if (node_position[0] < 0 || node_position[1] < 0 || node_position[0] >= gridArr.GetLength(0) || node_position[1] >= gridArr.GetLength(1))
                    {
                        continue;
                    }
                    else
                    {
                        if (avoidWalls)
                        {
                            if (gridArr[node_position[0], node_position[1]].tileType != DFTile.TileType.WALLCORRIDOR)
                            {
                                float newDist = currNode.distance + 1;

                                if (newDist < DjNodesArr[node_position[0], node_position[1]].distance)
                                {
                                    DjNodesArr[node_position[0], node_position[1]].distance = newDist;
                                    DjNodesArr[node_position[0], node_position[1]].parentDJnode = currNode;

                                }
                            }
                        }
                        else
                        {
                            float newDist = currNode.distance + 1;

                            if (newDist < DjNodesArr[node_position[0], node_position[1]].distance)
                            {
                                DjNodesArr[node_position[0], node_position[1]].distance = newDist;
                                DjNodesArr[node_position[0], node_position[1]].parentDJnode = currNode;
                            }
                        }

                    }
                }

                if (currNode.coord == endPoint)
                {
                    lastNode = currNode;

                    break;
                }

            }

            var solutioPath = new List<DFTile>();

            while (lastNode.parentDJnode != null)
            {
                solutioPath.Add(lastNode.gridRefTile);

                lastNode = lastNode.parentDJnode;
            }

            return solutioPath;
        }

        #endregion

        #endregion

        #region Random Walk


        /// <summary>
        /// Random walker
        /// </summary>
        /// <param name="gridArr"></param>
        /// <param name="iterations">The number of steps the walker takes</param>
        /// <param name="alreadyPassed">If it were to retrace it self should that reatraced step count?</param>
        /// <param name="maxIterMultiplier">it is possible for the walker to get stuck so to safely exit the while loop a maximum of (iterations * maxIterMultiplier) are allowed</param>
        /// <param name="randomStart">True for a random starting point, false for starting in the middle</param>
        public static void RandomWalk(DFTile[,] gridArr, int iterations, bool alreadyPassed, float maxIterMultiplier = 1.4f, bool randomStart = true)
        {
            int iterationsLeft = iterations;

            Vector2Int currentHead = new Vector2Int(gridArr.GetLength(0) / 2, gridArr.GetLength(1) / 2);

            if (randomStart)
                currentHead = new Vector2Int(Random.Range(0, gridArr.GetLength(0)), Random.Range(0, gridArr.GetLength(1)));

            while (iterationsLeft > 0)
            {
                int ranDir = Random.Range(0, 4);

                switch (ranDir)
                {
                    case 0:    //for

                        if (currentHead.y + 1 >= gridArr.GetLength(1))
                        { }
                        else
                        {
                            currentHead.y++;
                        }

                        break;

                    case 1:    //back
                        if (currentHead.y - 1 < 0)
                        { }
                        else
                        {
                            currentHead.y--;
                        }
                        break;

                    case 2:    //left
                        if (currentHead.x - 1 < 0)
                        { }
                        else
                        {
                            currentHead.x--;
                        }
                        break;

                    case 3:   //rigth
                        if (currentHead.x + 1 >= gridArr.GetLength(0))
                        { }
                        else
                        {
                            currentHead.x++;
                        }
                        break;

                    default:
                        break;
                }


                if (alreadyPassed)
                {
                    if (gridArr[(int)currentHead.x, (int)currentHead.y].tileWeight != 1)
                    {
                        gridArr[(int)currentHead.x, (int)currentHead.y].tileWeight = 1;
                        iterationsLeft--;
                    }
                }
                else
                {
                    gridArr[(int)currentHead.x, (int)currentHead.y].tileWeight = 1;
                    iterationsLeft--;
                }
            }
        }

        public static DFTile[,] CompartimentalisedRandomWalk(BoundsInt boundsRoom)
        {
            int maxY = boundsRoom.zMax - boundsRoom.zMin;
            int maxX = boundsRoom.xMax - boundsRoom.xMin;

            int iterations = (maxX) * (maxY);

            int iterationsLeft = iterations;

            DFTile[,] gridArr = new DFTile[maxX, maxY];

            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    gridArr[x, y] = new DFTile();
                    gridArr[x, y].position = new Vector2Int(x, y);
                }
            }

            Vector2Int currentHead = new Vector2Int(maxX / 2, maxY / 2);

            while (iterationsLeft > 0)
            {
                int ranDir = Random.Range(0, 4);

                switch (ranDir)
                {
                    case 0:    //for

                        if (currentHead.y + 1 >= gridArr.GetLength(1))
                        { }
                        else
                        {
                            currentHead.y++;
                        }

                        break;

                    case 1:    //back
                        if (currentHead.y - 1 < 0)
                        { }
                        else
                        {
                            currentHead.y--;
                        }
                        break;

                    case 2:    //left
                        if (currentHead.x - 1 < 0)
                        { }
                        else
                        {
                            currentHead.x--;
                        }
                        break;

                    case 3:   //rigth
                        if (currentHead.x + 1 >= gridArr.GetLength(0))
                        { }
                        else
                        {
                            currentHead.x++;
                        }
                        break;

                    default:
                        break;
                }


                gridArr[(int)currentHead.x, (int)currentHead.y].tileWeight = 1;
                iterationsLeft--;
            }

            RunCaIteration2D(gridArr, 4);

            return gridArr;
        }

        #endregion

        #region PerlinNoise

        /// <summary>
        /// Generate Perlin noise map, from 0 to 1
        /// </summary>
        /// <param name="gridArr"></param>
        /// <param name="scale"></param>
        /// <param name="octaves"></param>
        /// <param name="persistance"></param>
        /// <param name="lacu"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="threashold">If this value is bigger than 0 than anything belowe will have a set weight of 0 and anything above will have a set weight of 1</param>
        public static void PerlinNoise(DFTile[,] gridArr, float scale, int octaves, float persistance, float lacu, int offsetX, int offsetY, float threashold = 0)
        {
            if (scale <= 0)
            {
                scale = 0.0001f;
            }

            float maxN = float.MinValue;
            float minN = float.MaxValue;

            Parallel.For(0, gridArr.GetLength(1), y =>
            {
                for (int x = 0; x < gridArr.GetLength(0); x++)
                {

                    float amplitude = 1;
                    float freq = 1;
                    float noiseHeight = 0;

                    for (int i = 0; i < octaves; i++)
                    {

                        float sampleX = x / scale * freq + offsetX;
                        float sampleY = y / scale * freq + offsetY;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistance;

                        freq *= lacu;
                    }

                    if (noiseHeight > maxN) { maxN = noiseHeight; }
                    else if (noiseHeight < minN) { minN = noiseHeight; }

                    gridArr[x, y].tileWeight = Mathf.InverseLerp(minN, maxN, noiseHeight);
                    if (threashold != 0)
                    {
                        if (threashold < gridArr[x, y].tileWeight)
                            gridArr[x, y].tileWeight = 1;
                        else
                            gridArr[x, y].tileWeight = 0;
                    }
                }
            });
        }

        #region Manipulation of perlin noise

        /// <summary>
        /// apply smooth steps functions to the weightings of the perlin noise
        /// </summary>
        /// <param name="gridArr"></param>
        /// <param name="scale"></param>
        /// <param name="octaves"></param>
        /// <param name="persistance"></param>
        /// <param name="lacu"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        public static void PerlinNoiseSmoothStep(DFTile[,] gridArr, float scale, int octaves, float persistance, float lacu, int offsetX, int offsetY) 
        {
            PerlinNoise(gridArr, scale, octaves, persistance, lacu, offsetX, offsetY);

            Parallel.For(0, gridArr.GetLength(0), x =>
            {
                for (int y = 0; y < gridArr.GetLength(1); y++)
                {
                    gridArr[x, y].tileWeight = Mathf.SmoothStep(0, 1, gridArr[x, y].tileWeight);
                }
            });
        }


        /// <summary>
        /// apply easing out functions to the weightings of the perlin noise, lots of highs and a quick fall
        /// </summary>
        /// <param name="gridArr"></param>
        /// <param name="scale"></param>
        /// <param name="octaves"></param>
        /// <param name="persistance"></param>
        /// <param name="lacu"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="formulaType">Number from 1 to 4, 1 = quadratic easing  ---  2 = cubic easing  ---  3 = quartic easing  ---  4 = exponential easing </param>
        public static void PerlinNoiseEasingOut(DFTile[,] gridArr, float scale, int octaves, float persistance, float lacu, int offsetX, int offsetY, int formulaType)
        {
            PerlinNoise(gridArr, scale, octaves, persistance, lacu, offsetX, offsetY);

            Parallel.For(0, gridArr.GetLength(0), x =>
            {
                for (int y = 0; y < gridArr.GetLength(1); y++)
                {
                    switch (formulaType)
                    {
                        case 1:
                            gridArr[x, y].tileWeight = QuadraticEasingOut(gridArr[x, y].tileWeight);
                            break;
                        case 2:
                            gridArr[x, y].tileWeight = CubicEasingOut(gridArr[x, y].tileWeight);
                            break;
                        case 3:
                            gridArr[x, y].tileWeight = QuarticEasingOut(gridArr[x, y].tileWeight);
                            break;
                        case 4:
                            gridArr[x, y].tileWeight = ExponentialEasingOut(gridArr[x, y].tileWeight);
                            break;

                        default:
                            break;
                    }
                }
            });
        }

        public static float QuadraticEasingOut(float t) => 1f - Mathf.Pow(1f - t, 2f);

        public static float CubicEasingOut(float t) => 1f - Mathf.Pow(1f - t, 3f);

        public static float QuarticEasingOut(float t) => 1f - Mathf.Pow(1f - t, 4f);

        public static float ExponentialEasingOut(float t) => 1f - Mathf.Pow(2f, -10f * t);


        /// <summary>
        /// Noise weight Squared,  Flat-ish lands but with occasionally high peaks
        /// </summary>
        /// <param name="gridArr"></param>
        /// <param name="scale"></param>
        /// <param name="octaves"></param>
        /// <param name="persistance"></param>
        /// <param name="lacu"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        public static void PerlinNoiseSmoothStart(DFTile[,] gridArr, float scale, int octaves, float persistance, float lacu, int offsetX, int offsetY)
        {
            PerlinNoise(gridArr, scale, octaves, persistance, lacu, offsetX, offsetY);

            Parallel.For(0, gridArr.GetLength(0), x =>
            {
                for (int y = 0; y < gridArr.GetLength(1); y++)
                {
                    gridArr[x, y].tileWeight = MathF.Pow(gridArr[x, y].tileWeight, 2);
                }
            });
        }

        /// <summary>
        /// Good For creating Channels
        /// </summary>
        /// <param name="gridArr"></param>
        /// <param name="scale"></param>
        /// <param name="octaves"></param>
        /// <param name="persistance"></param>
        /// <param name="lacu"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="absoluteValue"></param>
        public static void PerlinNoiseAbsoluteValue(DFTile[,] gridArr, float scale, int octaves, float persistance, float lacu, int offsetX, int offsetY, float absoluteValue)
        {
            PerlinNoise(gridArr, scale, octaves, persistance, lacu, offsetX, offsetY);

            Parallel.For(0, gridArr.GetLength(0), x =>
            {
                for (int y = 0; y < gridArr.GetLength(1); y++)
                {
                    if (gridArr[x, y].tileWeight < absoluteValue)
                        gridArr[x, y].tileWeight = 1 - gridArr[x, y].tileWeight;
                }
            });
        }

        #endregion

        private static Vector2 MoveVector(Vector2 vector, float direction, float turnMulti)
        {
            float dx = Mathf.Cos(direction * 2 * Mathf.PI) * turnMulti;
            float dy = Mathf.Sin(direction * 2 * Mathf.PI) * turnMulti;

            Vector2 result = new Vector2(vector.x + dx, vector.y + dy);
            return result;
        }

        /// <summary>
        /// Perlin worms create a smooth and flowing pattern using the weight generated from a normal perlin noise map
        /// </summary>
        /// <param name="gridArr"></param>
        /// <param name="scale"></param>
        /// <param name="octaves"></param>
        /// <param name="persistance"></param>
        /// <param name="lacu"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="maxWormLength">The maximum length a worm can be</param>
        /// <param name="turnMultiplier">a higher turning value will make the turns more sharp</param>
        /// <returns></returns>
        public static HashSet<DFTile> PerlinWorms(DFTile[,] gridArr, float scale, int octaves, float persistance, float lacu, int offsetX, int offsetY, int maxWormLength, float turnMultiplier)
        {
            var wormTiles = new HashSet<DFTile>();

            PerlinNoise(gridArr, scale, octaves, persistance, lacu, offsetX, offsetY);

            var currPos = new Vector2(Random.Range(0, gridArr.GetLength(0) - 1), Random.Range(0, gridArr.GetLength(1) - 1));

            DFTile lastTileAdded = DFGeneralUtil.WorldPosToTile(currPos, gridArr);

            for (int i = 0; i < maxWormLength; i++)
            {
                currPos = MoveVector(currPos, gridArr[lastTileAdded.position.x, lastTileAdded.position.y].tileWeight, turnMultiplier);

                var newTile = DFGeneralUtil.WorldPosToTile(currPos, gridArr);

                if (newTile != null)
                {
                    if (!wormTiles.Contains(newTile))
                    {
                        wormTiles.Add(newTile);
                        lastTileAdded = newTile;
                    }
                }
                else
                {
                    break;
                }
            }

            return wormTiles;
        }


        #endregion

        #region Triangulation


        /// <summary>
        /// Call this function if the triangle data is not avaialable, this will run the delunay triangulation function for you
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static List<Edge> PrimAlgoNoDelu(List<Vector2> points)
        {
            var triangulation = DelaunayTriangulation(points);

            return PrimAlgo(points, triangulation.Item1);
        }

        /// <summary>
        /// Run the prims algorithm to find the minimum spanning tree
        /// </summary>
        /// <param name="points"></param>
        /// <param name="triangulation"></param>
        /// <returns></returns>
        public static List<Edge> PrimAlgo(List<Vector2> points, List<Triangle> triangulation)
        {
            List<Edge> primsAlgo = new List<Edge>();

            HashSet<Vector2> visitedVertices = new HashSet<Vector2>();

            var ran = Random.Range(0, points.Count);
            var vertex = points[ran];

            visitedVertices.Add(vertex);

            while (visitedVertices.Count != points.Count)
            {
                List<Edge> edgesWithPoint = new List<Edge>();

                foreach (var trig in triangulation)
                {
                    foreach (var edge in trig.edges)
                    {
                        foreach (var point in visitedVertices)
                        {
                            if (visitedVertices.Contains(edge.edge[0]) && visitedVertices.Contains(edge.edge[0]))
                            {
                                // do nothing
                            }
                            else if (visitedVertices.Contains(edge.edge[0]))
                            {
                                edgesWithPoint.Add(edge);
                            }
                            else if (visitedVertices.Contains(edge.edge[1]))
                            {
                                edgesWithPoint.Add(edge);
                            }
                        }
                    }
                }

                var edgesWithPointSort = edgesWithPoint.OrderBy(c => c.length).ToArray();   // we sort all the edges by the smallest to biggest

                visitedVertices.Add(edgesWithPointSort[0].edge[0]);
                visitedVertices.Add(edgesWithPointSort[0].edge[1]);
                primsAlgo.Add(edgesWithPointSort[0]);
            }

            return primsAlgo;
        }


        /// <summary>
        /// Deluna
        /// </summary>
        /// <param name="points">points in 2D space to triangulate</param>
        /// <returns>Returns a touple containing   item1 = Triangles list   ---  item2 = All edges list</returns>
        public static Tuple<List<Triangle>, List<Edge>> DelaunayTriangulation(List<Vector2> points)
        {
            if (points.Count < 4) 
            {
                Debug.Log($"The given points in the list for triangulation are not enough, need to have at least 3 points and i was given {points.Count}");
                return null;
            }

            var triangulation = new List<Triangle>();

            Vector2 superTriangleA = new Vector2(10000, 10000);
            Vector2 superTriangleB = new Vector2(10000, 0);
            Vector2 superTriangleC = new Vector2(0, 10000);

            triangulation.Add(new Triangle(superTriangleA, superTriangleB, superTriangleC));

            foreach (Vector2 point in points)
            {
                List<Triangle> badTriangles = new List<Triangle>();

                foreach (Triangle triangle in triangulation)
                {
                    if (IsPointInCircumcircle(triangle.a, triangle.b, triangle.c, point))
                    {
                        badTriangles.Add(triangle);
                    }
                }

                List<Edge> polygon = new List<Edge>();

                foreach (Triangle triangle in badTriangles)
                {
                    foreach (Edge triangleEdge in triangle.edges)
                    {
                        bool isShared = false;

                        foreach (Triangle otherTri in badTriangles)
                        {
                            if (otherTri == triangle) { continue; }

                            foreach (Edge otherEdge in otherTri.edges)
                            {
                                if (LineIsEqual(triangleEdge, otherEdge))
                                {
                                    isShared = true;
                                }
                            }
                        }

                        if (isShared == false)
                        {
                            polygon.Add(triangleEdge);
                        }

                    }
                }

                foreach (Triangle badTriangle in badTriangles)
                {
                    triangulation.Remove(badTriangle);   // i think this is the issue here
                }

                foreach (Edge edge in polygon)
                {
                    Triangle newTriangle = new Triangle(edge.edge[0], edge.edge[1], point);
                    triangulation.Add(newTriangle);
                }
            }

            for (int i = triangulation.Count - 1; i >= 0; i--)
            {
                if (triangulation[i].HasVertex(superTriangleA) || triangulation[i].HasVertex(superTriangleB) || triangulation[i].HasVertex(superTriangleC))
                {
                    triangulation.Remove(triangulation[i]);
                }
            }

            var edges = new List<Edge>();

            foreach (var tri in triangulation)
            {
                foreach (var edge in tri.edges)
                {
                    bool same = false;

                    foreach (var alreadySavedEdeg in edges)
                    {
                        if (LineIsEqual(alreadySavedEdeg, edge))
                        {
                            same = true;
                            break;
                        }
                    }

                    if (same == false)
                        edges.Add(edge);
                }
            }

            return new Tuple<List<Triangle>, List<Edge>>(triangulation, edges);
        }

        /// <summary>
        /// checks if two edges are the same
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static bool LineIsEqual(Edge A, Edge B)
        {
            if ((A.edge[0] == B.edge[0] && A.edge[1] == B.edge[1]) || (A.edge[0] == B.edge[1] && A.edge[1] == B.edge[0])) { return true; }
            else { return false; }
        }

        private static bool IsPointInCircumcircle(Vector3 A, Vector3 B, Vector3 C, Vector3 D)
        {
            float ax_ = A[0] - D[0];
            float ay_ = A[1] - D[1];
            float bx_ = B[0] - D[0];
            float by_ = B[1] - D[1];
            float cx_ = C[0] - D[0];
            float cy_ = C[1] - D[1];

            if ((
                (ax_ * ax_ + ay_ * ay_) * (bx_ * cy_ - cx_ * by_) -
                (bx_ * bx_ + by_ * by_) * (ax_ * cy_ - cx_ * ay_) +
                (cx_ * cx_ + cy_ * cy_) * (ax_ * by_ - bx_ * ay_)
            ) < 0)
            {
                return true;
            }

            else { return false; }
        }

        #endregion

        #region Binary Partition System

        public static List<BoundsInt> BSPAlgo(BoundsInt toSplit, int minHeight, int minWidth)
        {

            List<BoundsInt> roomList = new List<BoundsInt>();
            Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();

            roomsQueue.Enqueue(toSplit);   // enque add to que
            while (roomsQueue.Count > 0)
            {
                var room = roomsQueue.Dequeue();   // take out and split this

                // this room can either contain a room or split  room
                if (room.size.y >= minHeight && room.size.x >= minWidth)   // all rooms should at least be big enough
                {
                    if (Random.value < 0.5f)
                    {
                        if (room.size.y >= minHeight * 2 + 1)
                        {
                            SplitHori(minHeight, room, roomsQueue);
                        }
                        else if (room.size.x >= minWidth * 2 + 1)
                        {
                            SplitVert(minWidth, room, roomsQueue);
                        }
                        else
                        {
                            roomList.Add(room);
                        }
                    }
                    else
                    {
                        if (room.size.x >= minWidth * 2 + 1)
                        {
                            SplitVert(minWidth, room, roomsQueue);
                        }
                        else if (room.size.y >= minHeight * 2 + 1)
                        {
                            SplitHori(minHeight, room, roomsQueue);
                        }
                        else
                        {
                            roomList.Add(room);
                        }
                    }
                }
            }

            return roomList;
        }

        private static void SplitVert(int minWidth, BoundsInt room, Queue<BoundsInt> roomQue)
        {

            int minX = room.min.x;
            int maxX = room.max.x;

            int adjustedMinX = minX + minWidth;
            int adjustedMaxX = maxX - minWidth;

            var ranPosition = Random.Range(adjustedMinX, adjustedMaxX);

            BoundsInt roomLeft = new BoundsInt();

            roomLeft.min = new Vector3Int(room.min.x, room.min.y, 0);
            roomLeft.max = new Vector3Int(ranPosition, room.max.y, 0);

            BoundsInt roomRight = new BoundsInt();

            roomRight.min = new Vector3Int(ranPosition, room.min.y, 0);
            roomRight.max = new Vector3Int(room.max.x, room.max.y, 0);

            roomQue.Enqueue(roomRight);
            roomQue.Enqueue(roomLeft);
        }

        private static void SplitHori(int minHeight, BoundsInt room, Queue<BoundsInt> roomQue)
        {
            int minY = room.min.y;
            int maxY = room.max.y;

            int adjustedMinY = minY + minHeight;
            int adjustedMaxY = maxY - minHeight;

            var ranPosition = Random.Range(adjustedMinY, adjustedMaxY);

            BoundsInt roomTop = new BoundsInt();

            roomTop.min = new Vector3Int(room.min.x, ranPosition, 0);
            roomTop.max = new Vector3Int(room.max.x, room.max.y, 0);

            BoundsInt roomBot = new BoundsInt();

            roomBot.min = new Vector3Int(room.min.x, room.min.y, 0);
            roomBot.max = new Vector3Int(room.max.x, ranPosition, 0);

            roomQue.Enqueue(roomBot);
            roomQue.Enqueue(roomTop);
        }

        #endregion

        #region Flood Fill

        private static void ResetVisited(DFTile[,] gridArr)
        {
            Parallel.For(0, gridArr.GetLength(1), y =>
            {
                for (int x = 0; x < gridArr.GetLength(0); x++)
                {
                    gridArr[x, y].visited = false;
                }
            });
        }


        public static List<Vector2Int> FloodFill(DFTile[,] gridArr, Vector2Int start, bool checkForRoomOnly, DFTile.TileType typeToCheck, float minCheckWeight)
        {
            int height = gridArr.GetLength(1);
            int width = gridArr.GetLength(0);

            List<Vector2Int> room = new List<Vector2Int>();
            room.Add(start);

            for (int i = 0; i < room.Count; i++)
            {
                int x = room[i].x;
                int y = room[i].y;

                if (x - 1 >= 0)
                {
                    if (checkForRoomOnly)
                    {
                        if (gridArr[x - 1, y].tileType == typeToCheck && gridArr[x - 1, y].visited == false)
                        {
                            gridArr[x - 1, y].visited = true;
                            room.Add(new Vector2Int(x - 1, y));
                        }
                    }
                    else
                    {
                        if (gridArr[x - 1, y].tileWeight > minCheckWeight && gridArr[x - 1, y].visited == false)
                        {
                            gridArr[x - 1, y].visited = true;
                            room.Add(new Vector2Int(x - 1, y));
                        }
                    }
                }

                if (y - 1 >= 0)
                {
                    if (checkForRoomOnly)
                    {
                        if (gridArr[x, y - 1].tileType == typeToCheck && gridArr[x, y - 1].visited == false)
                        {
                            gridArr[x, y - 1].visited = true;
                            room.Add(new Vector2Int(x, y - 1));
                        }
                    }
                    else
                    {
                        if (gridArr[x, y - 1].tileWeight > minCheckWeight && gridArr[x, y - 1].visited == false)
                        {
                            gridArr[x, y - 1].visited = true;
                            room.Add(new Vector2Int(x, y - 1));
                        }
                    }
                }

                if (x + 1 < width)
                {

                    if (checkForRoomOnly)
                    {
                        if (gridArr[x + 1, y].tileType == typeToCheck && gridArr[x + 1, y].visited == false)
                        {
                            gridArr[x + 1, y].visited = true;
                            room.Add(new Vector2Int(x + 1, y));
                        }
                    }
                    else
                    {
                        if (gridArr[x + 1, y].tileWeight > minCheckWeight && gridArr[x + 1, y].visited == false)
                        {
                            gridArr[x + 1, y].visited = true;
                            room.Add(new Vector2Int(x + 1, y));
                        }
                    }
                }

                if (y + 1 < height)
                {
                    if (checkForRoomOnly)
                    {
                        if (gridArr[x, y + 1].tileType == typeToCheck && gridArr[x, y + 1].visited == false)
                        {
                            gridArr[x, y + 1].visited = true;
                            room.Add(new Vector2Int(x, y + 1));
                        }
                    }
                    else
                    {
                        if (gridArr[x, y + 1].tileWeight > minCheckWeight && gridArr[x, y + 1].visited == false)
                        {
                            gridArr[x, y + 1].visited = true;
                            room.Add(new Vector2Int(x, y + 1));
                        }
                    }
                }
            }

            return room;
        }


        /// <summary>
        /// Used to get all the rooms that are in the given grid
        /// </summary>
        /// <param name="gridArr"></param>
        /// <param name="checkTypeOnly"> If true this function will use the type given to check for the rooms, if false it will use the weight of the tile</param>
        /// <param name="typeToCheck">the types of tiles to group</param>
        /// <param name="minCheckWeight">The minimum weight to be recognised as part of a room</param>
        /// <returns>Returns a list of a lists of Tiles that equal to a room</returns>
        public static List<List<DFTile>> GetAllRooms(DFTile[,] gridArr, bool checkTypeOnly = false, DFTile.TileType typeToCheck = DFTile.TileType.FLOORROOM, float minCheckWeight = 0.5f)
        {
            var rooms = new List<List<DFTile>>();

            ResetVisited(gridArr);  //sest everything back to false ready for flood

            List<Vector2Int> openCoords = new List<Vector2Int>();  // this has all the tile coords of the tiles that are considered roomable

            //Parallel.For(0, gridArr.GetLength(1), y =>
            for (int y = 0; y < gridArr.GetLength(1); y++)
            {
                for (int x = 0; x < gridArr.GetLength(0); x++)
                {
                    if (checkTypeOnly)
                    {
                        if (gridArr[x, y].tileType == typeToCheck)
                            openCoords.Add(new Vector2Int(x, y));
                    }
                    else
                    {
                        if (gridArr[x, y].tileWeight > minCheckWeight)
                            openCoords.Add(new Vector2Int(x, y));
                    }
                }
            }//);


            while (openCoords.Count > 2)   // until there is stuff in the open coords  then
            {
                var ranCoord = openCoords[Random.Range(0, openCoords.Count - 1)];   //get a random from the list of possible positionss

                var room = FloodFill(gridArr, ranCoord, checkTypeOnly, typeToCheck,minCheckWeight);

                for (int i = openCoords.Count(); i-- > 0;) //for every open coord 
                {
                    foreach (var coor in room)    //does the open cord contain then remove to satisfy infinte while loop
                    {
                        if (openCoords[i] == coor)
                        {
                            openCoords.RemoveAt(i);
                            break;
                        }
                    }
                }

                List<DFTile> roomBasicTile = new List<DFTile>();

                for (int y = 0; y < gridArr.GetLength(1); y++)
                {
                    for (int x = 0; x < gridArr.GetLength(0); x++)
                    {
                        foreach (var coord in room)
                        {
                            if (new Vector2Int(x, y) == coord)
                            {
                                gridArr[x, y].tileType = DFTile.TileType.FLOORROOM;
                                roomBasicTile.Add(gridArr[x, y]);
                            }
                        }
                    }
                }

                rooms.Add(roomBasicTile);
            }

            return rooms;
        }

        #endregion

        #region Cellular Automata

        /// <summary>
        /// Fills the grid randomly with points
        /// </summary>
        /// <param name="gridArr"></param>
        /// <param name="percentageOfSpawn">Each points percenatge of being populated</param>
        /// <param name="weight">The weight set of the populated tile</param>
        /// <returns></returns>
        public static void SpawnRandomPointsOnTheGrid(DFTile[,] gridArr, float percentageOfSpawn = 0.5f, float weight = 1)
        {
            Parallel.For(0, gridArr.GetLength(1), y =>
            {
                for (int x = 0; x < gridArr.GetLength(0); x++)
                {
                    if (Random.value > percentageOfSpawn)
                    {
                        gridArr[x, y].tileWeight = 0;
                    }
                    else
                    {
                        gridArr[x, y].tileWeight = weight;
                    }
                }
            });
        }

        /// <summary>
        /// Cellular Automata iteration
        /// </summary>
        /// <param name="gridArr"></param>
        /// <param name="neighboursNeeded">The Number of alive neighbour a cell needs to be alive</param>
        public static void RunCaIteration2D(DFTile[,] gridArr, int neighboursNeeded)
        {
            float[,] copyArrayStorage = new float[gridArr.GetLength(0), gridArr.GetLength(1)];

            for (int y = 0; y < gridArr.GetLength(1); y++)
            {
                for (int x = 0; x < gridArr.GetLength(0); x++)
                {
                    copyArrayStorage[x, y] = gridArr[x, y].tileWeight;
                }
            }

            for (int y = 0; y < gridArr.GetLength(1); y++)
            {
                for (int x = 0; x < gridArr.GetLength(0); x++)
                {
                    int neighbours = 0;

                    for (int col_offset = -1; col_offset < 2; col_offset++)
                    {
                        for (int row_offset = -1; row_offset < 2; row_offset++)
                        {
                            if (!(y + col_offset < 0 || x + row_offset < 0 || y + col_offset >= gridArr.GetLength(1) - 1 || x + row_offset >= gridArr.GetLength(0) - 1) && !(col_offset == 0 && row_offset == 0))
                            {
                                if (copyArrayStorage[x + row_offset, y + col_offset] == 1)
                                {
                                    neighbours++;
                                }
                            }
                        }
                    }

                    if (neighbours >= neighboursNeeded)
                    {   //empty is = false therefore weight is there
                        gridArr[x, y].tileWeight = 1;
                    }
                    else
                    {   //true
                        gridArr[x, y].tileWeight = 0;
                    }
                }
            }

        }

        /// <summary>
        /// Cellular Automata iteration without the new spawning of points, used for smooting out other algorithms generations
        /// </summary>
        /// <param name="gridArr"></param>
        /// <param name="neighboursNeeded">The Number of alive neighbour a cell needs to be alive</param>
        public static void CleanUp2dCA(DFTile[,] gridArr, int neighboursNeeded)
        {
            float[,] copyArrayStorage = new float[gridArr.GetLength(0), gridArr.GetLength(1)];

            for (int y = 0; y < gridArr.GetLength(1); y++)
            {
                for (int x = 0; x < gridArr.GetLength(0); x++)
                {
                    copyArrayStorage[x, y] = gridArr[x, y].tileWeight;
                }
            }

            Parallel.For(0, gridArr.GetLength(1), y =>
            {
                for (int x = 0; x < gridArr.GetLength(0); x++)
                {
                    int neighbours = 0;
                    if (copyArrayStorage[x, y] == 1)
                    {
                        for (int col_offset = -1; col_offset < 2; col_offset++)
                        {
                            for (int row_offset = -1; row_offset < 2; row_offset++)
                            {
                                if (!(y + col_offset < 0 || x + row_offset < 0 || y + col_offset >= gridArr.GetLength(1) - 1 || x + row_offset >= gridArr.GetLength(0) - 1) && !(col_offset == 0 && row_offset == 0))
                                {
                                    if (copyArrayStorage[x + row_offset, y + col_offset] == 1)
                                    {
                                        neighbours++;
                                    }
                                }
                            }
                        }

                        if (neighbours >= neighboursNeeded)
                        {   //empty is = false therefore weight is there
                            gridArr[x, y].tileWeight = 1;
                        }
                        else
                        {   //true
                            gridArr[x, y].tileWeight = 0;
                        }
                    }
                }
            });
        }

        #endregion

        #region Room Gen 

        /// <summary>
        /// Return null if the shape is not a valid or it hit something else, If the shape is valid it will return a list of tiles in that shape
        /// </summary>
        /// <param name="width">Widht of the shape</param>
        /// <param name="height">Height of the shape</param>
        /// <param name="centerPoint">The center point</param>
        /// <param name="gridArr">The grid to draw the circle on</param>
        /// <param name="actuallyDraw">False to make no changes to the grid, true to apply changes</param>
        /// <param name="tileType">The type of tile to make the tiles in the radius</param>
        /// <param name="setWeight">The weight to set the tiles in the circle</param>
        /// <returns></returns>
        public static List<DFTile> CreateSquareRoom(int width, int height, Vector2Int centerPoint, DFTile[,] gridArr,  bool actuallyDraw = false, DFTile.TileType tileType = DFTile.TileType.FLOORROOM, float setWeight = 0.75f, bool checkForFitting = false)
        {
            var room = new List<DFTile>();

            int halfWidth = width / 2;
            int halfHeight = height / 2;

            if (centerPoint.x - halfWidth < 0 || centerPoint.x + halfWidth >= gridArr.GetLength(0)  && checkForFitting)
                return null;

            if (centerPoint.y - halfHeight < 0 || centerPoint.y + halfHeight >= gridArr.GetLength(1)  && checkForFitting)
                return null;

            for (int y = centerPoint.y - halfHeight; y < centerPoint.y + halfHeight; y++)
            {
                for (int x = centerPoint.x - halfWidth; x < centerPoint.x + halfWidth; x++)
                {
                    if (x < 0 || y < 0 || y >= gridArr.GetLength(1) || x >= gridArr.GetLength(0))
                    { continue; }

                    if (gridArr[x, y].tileType != DFTile.TileType.VOID  && checkForFitting)
                    {
                        return null;
                    }

                    if (actuallyDraw)
                    {
                        gridArr[x, y].tileType = tileType;
                        gridArr[x, y].tileWeight = setWeight;
                        room.Add(gridArr[x, y]);
                    }
                }
            }

            return room;
        }

        /// <summary>
        /// Return null if the circle is not a valid or it hit something else, If the circle is valid it will return a list of tiles in that circle
        /// </summary>
        /// <param name="gridArr">The grid to draw the circle on</param>
        /// <param name="center">Center of the circle</param>
        /// <param name="radius">The radius</param>
        /// <param name="tileType">The type of tile to make the tiles in the radius</param>
        /// <param name="setWeight">The weight to set the tiles in the circle</param>
        /// <param name="actuallyDraw">False to make no changes to the grid, true to apply changes</param>
        /// <returns></returns>
        public static List<DFTile> CreateCircleRoom(DFTile[,] gridArr, Vector2Int center, int radius, DFTile.TileType tileType = DFTile.TileType.FLOORROOM, bool actuallyDraw = false, float setWeight = 0.75f, bool checkForFitting = false)
        {
            var room = new List<DFTile>();

            for (int y = center.y - radius; y <= center.y + radius; y++)
            {
                for (int x = center.x - radius; x <= center.x + radius; x++)
                {
                    bool isInCircle = (x - center.x) * (x - center.x) + (y - center.y) * (y - center.y) < radius * radius;
                    if (isInCircle)
                    {
                        if (x < 0 || y <0 || y>= gridArr.GetLength(1) || x >= gridArr.GetLength(0))
                        { continue; }

                        if (gridArr[x, y].tileType != DFTile.TileType.VOID && checkForFitting)
                        {
                            return null;
                        }

                        if (actuallyDraw)
                        {
                            gridArr[x, y].tileType = tileType;
                            gridArr[x, y].tileWeight = setWeight;
                            room.Add(gridArr[x, y]);
                        }
                    }
                }
            }

            return room;
        }

        /// <summary>
        /// to change
        /// </summary>
        /// <param name="boundsRoom"></param>
        /// <returns></returns>
        public static DFTile[,] CompartimentalisedCA(BoundsInt boundsRoom)
        {
            int maxY = boundsRoom.zMax - boundsRoom.zMin;
            int maxX = boundsRoom.xMax - boundsRoom.xMin;

            DFTile[,] _gridarray2D = new DFTile[maxX, maxY];

            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    _gridarray2D[x, y] = new DFTile();
                    _gridarray2D[x, y].position = new Vector2Int(x, y);
                }
            }

            SpawnRandomPointsOnTheGrid(_gridarray2D, 0.55f);
            RunCaIteration2D(_gridarray2D, 4);
            RunCaIteration2D(_gridarray2D, 4);
            CleanUp2dCA(_gridarray2D, 4);

            return _gridarray2D;
        }

        #endregion

        #region Diffusion-Limited Aggregation

        /// <summary>
        /// Diffusion-Limited Aggregation function
        /// </summary>
        /// <param name="gridArr"></param>
        /// <param name="moversAmount">Amoutn of particles</param>
        /// <param name="stickingProbability">The chance of those particles sticking to eachother</param>
        /// <returns></returns>
        public static bool DiffLimAggregation(DFTile[,] gridArr, int moversAmount, float stickingProbability)
        {
            if (stickingProbability <= 0 || stickingProbability > 1) 
            {
                Debug.Log($"The given probabiliy of sticking for Diffusion-Limited Aggregation is not accepted and should be between 0 and 1 or 1");
                return false;
            }

            int height = gridArr.GetLength(1)-1;
            int length = gridArr.GetLength(0)-1;

            for (int i = 0; i < moversAmount; i++)
            {
                Vector2Int position = new Vector2Int(Random.Range(0, length), Random.Range(0, height));

                while (position.x > 0 && position.x < length - 1 && position.y > 0 && position.y < height - 1)
                {
                    // Move the particle in a random direction

                    if (CheckAround(gridArr, position))
                    {
                        if (Random.Range(0f, 1f) < stickingProbability)
                        {
                            gridArr[position.x, position.y].tileWeight = 0.75f;
                            gridArr[position.x, position.y].tileType = DFTile.TileType.FLOORCORRIDOR;
                        }
                        break;
                    }

                    Vector2 direction;
                    switch (Random.Range(0, 4))
                    {
                        case 0:
                            direction = Vector2.left;
                            break;
                        case 1:
                            direction = Vector2.right;
                            break;
                        case 2:
                            direction = Vector2.up;
                            break;
                        case 3:
                            direction = Vector2.down;
                            break;
                        default:
                            direction = Vector2.zero;
                            break;
                    }

                    position += Vector2Int.RoundToInt(direction);
                }
            }

            return true;
        }

        /// <summary>
        /// Used for Diffusion Limited Aggregation Algorithm, check for nearby cells to stop and settles
        /// </summary>
        /// <param name="gridArr"></param>
        /// <param name="currPos"></param>
        /// <returns></returns>
        private static bool CheckAround(DFTile[,] gridArr, Vector2Int currPos)
        {
            if (currPos.x + 1 < gridArr.GetLength(0))
            {
                if (gridArr[currPos.x + 1, currPos.y].tileType == DFTile.TileType.FLOORROOM || gridArr[currPos.x + 1, currPos.y].tileType == DFTile.TileType.FLOORCORRIDOR)
                    return true;
            }

            if (currPos.y + 1 < gridArr.GetLength(1))
            {
                if (gridArr[currPos.x, currPos.y + 1].tileType == DFTile.TileType.FLOORROOM || gridArr[currPos.x, currPos.y + 1].tileType == DFTile.TileType.FLOORCORRIDOR)
                    return true;
            }

            if (currPos.y - 1 >= 0)
            {
                if (gridArr[currPos.x, currPos.y - 1].tileType == DFTile.TileType.FLOORROOM || gridArr[currPos.x, currPos.y - 1].tileType == DFTile.TileType.FLOORCORRIDOR)
                    return true;
            }

            if (currPos.x - 1 >= 0)
            {
                if (gridArr[currPos.x - 1, currPos.y].tileType == DFTile.TileType.FLOORROOM || gridArr[currPos.x - 1, currPos.y].tileType == DFTile.TileType.FLOORCORRIDOR)
                    return true;
            }

            return false;
        }

        #endregion

        #region Poissant


        /// <summary>
        /// runs the poissant algo and gives back position that are allowed
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="radius"></param>
        /// <param name="numSamplesBeforeRejection"></param>
        /// <returns></returns>
        public static List<Vector2> GeneratePossiantPoints(float width, float height, float radius, int numSamplesBeforeRejection = 30)
        {
            float cellSize = radius / (float)Math.Sqrt(2);
            int[,] grid = new int[Mathf.CeilToInt(height / cellSize), Mathf.CeilToInt(width / cellSize)];
            List<Vector2> points = new List<Vector2>();
            List<Vector2> spawnPoints = new List<Vector2>();
            spawnPoints.Add(new Vector2(Random.value * width, Random.value * height));

            while (spawnPoints.Count > 0)
            {
                int spawnIndex = Random.Range(0, spawnPoints.Count);
                Vector2 spawnCenter = spawnPoints[spawnIndex];
                bool candidateAccepted = false;

                for (int i = 0; i < numSamplesBeforeRejection; i++)
                {
                    float angle = Random.value * Mathf.PI * 2;
                    Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                    float r = Random.Range(radius, 2 * radius);
                    Vector2 candidate = spawnCenter + dir * r;

                    if (IsValid(candidate, width, height, radius, cellSize, grid, points))
                    {
                        points.Add(candidate);
                        spawnPoints.Add(candidate);
                        grid[(int)(candidate.y / cellSize), (int)(candidate.x / cellSize)] = points.Count;
                        candidateAccepted = true;
                        break;
                    }
                }

                if (!candidateAccepted)
                {
                    spawnPoints.RemoveAt(spawnIndex);
                }
            }

            return points;
        }

        private static bool IsValid(Vector2 candidate, float width, float height, float radius, float cellSize, int[,] grid, List<Vector2> points)
        {
            if (candidate.x < 0 || candidate.x > width || candidate.y < 0 || candidate.y > height)
            {
                return false;
            }

            int cellX = (int)(candidate.x / cellSize);
            int cellY = (int)(candidate.y / cellSize);
            int searchStartX = Mathf.Max(0, cellX - 2);
            int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(1) - 1);
            int searchStartY = Mathf.Max(0, cellY - 2);
            int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(0) - 1);

            for (int x = searchStartX; x <= searchEndX; x++)
            {
                for (int y = searchStartY; y <= searchEndY; y++)
                {
                    int pointIndex = grid[y, x] - 1;

                    if (pointIndex != -1)
                    {
                        Vector2 point = points[pointIndex];

                        if (Vector2.Distance(candidate, point) < radius)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// checks against the map
        /// </summary>
        /// <param name="poissantPositions"></param>
        /// <param name="gridArr"></param>
        /// <param name="perc"></param>
        /// <returns></returns>
        public static List<Vector2> RunPoissantCheckOnCurrentTileMap(List<Vector2> poissantPositions, DFTile[,] gridArr, float perc)
        {
            var acceptedPoissantList = new List<Vector2>();

            for (int i = 0; i < poissantPositions.Count; i++)
            {
                float pointX = poissantPositions[i].x;
                float pointY = poissantPositions[i].y;

                float tileWidth = 1;
                float tileHeight = 1;
                int tileX = Mathf.FloorToInt(pointX / tileWidth);
                int tileY = Mathf.FloorToInt(pointY / tileHeight);

                if (tileX > gridArr.GetLength(0) - 1)
                    continue;
                if (tileY > gridArr.GetLength(1) - 1)
                    continue;

                if (gridArr[tileX, tileY].tileType == DFTile.TileType.FLOORROOM)
                {
                    if (Random.value > perc)
                        acceptedPoissantList.Add(poissantPositions[i]);
                }
            }

            return acceptedPoissantList;
        }

        #endregion

        #region DiamondSquare algo

        public static bool DiamondSquare(int maxHeight, int minHeight, float roughness, DFTile[,] gridArr)
        {
            if (gridArr.GetLength(0) != gridArr.GetLength(1))
            {
                Debug.Log($"The diamond square algorithm only accepts grid that are square shaped, therefore both height and width need to be equal");
                return false;
            }

            if (gridArr.GetLength(1) % 2 == 0) 
            {
                Debug.Log($"The diamond square algorithm only accepts width and height which satisfies the 2n+1 formula");
                return false;
            }

            // get the size
            var mapSize = gridArr.GetLength(1);

            // start the grid
            float[,] grid2D = new float[mapSize, mapSize];

            //set the 4 random corners
            grid2D[0, 0] = Random.Range(minHeight, maxHeight);   // top left
            grid2D[mapSize - 1, mapSize - 1] = Random.Range(minHeight, maxHeight);    // bot right
            grid2D[0, mapSize - 1] = Random.Range(minHeight, maxHeight); // top right
            grid2D[mapSize - 1, 0] = Random.Range(minHeight, maxHeight); // bot left

            var chunkSize = mapSize - 1; 

            while (chunkSize > 1)
            {
                int halfChunk = chunkSize / 2;

                for (int y = 0; y < mapSize - 1; y = y + chunkSize)
                {
                    for (int x = 0; x < mapSize - 1; x = x + chunkSize)
                    {
                        grid2D[y + halfChunk, x + halfChunk] = (grid2D[y, x] + grid2D[y, x + chunkSize] + grid2D[y + chunkSize, x] + grid2D[y + chunkSize, x + chunkSize]) / 4 + Random.Range(-roughness, roughness);
                    }
                }

                for (int y = 0; y < mapSize; y = y + halfChunk)
                {
                    for (int x = (y + halfChunk) % chunkSize; x < mapSize; x = x + chunkSize)
                    {
                        grid2D[y, x] =
                            (grid2D[(y - halfChunk + mapSize) % mapSize, x] +
                                  grid2D[(y + halfChunk) % mapSize, x] +
                                  grid2D[y, (x + halfChunk) % mapSize] +
                                  grid2D[y, (x - halfChunk + mapSize) % mapSize]) / 4 + Random.Range(-roughness, roughness);
                    }
                }
                chunkSize = chunkSize / 2;
                roughness = roughness / 2;
            }

            for (int y = 0; y < gridArr.GetLength(1); y++)
            {
                for (int x = 0; x < gridArr.GetLength(0); x++)
                {
                    gridArr[x, y].tileWeight = grid2D[y, x];
                }
            }

            return true;
        }

        #endregion

        #region Voronoi

        /// <summary>
        /// Given a grid performs the voronoi fracture algorithm. Sets the index variable in the DFTile class to the specific fracture index
        /// </summary>
        /// <param name="gridArr"></param>
        /// <param name="numOfPoints">number of voronoi fractures</param>
        /// <param name="DistanceCalculationType">True for Euclidean Distance, False for Manhattan Distance</param>
        public static void Voronoi2D(DFTile[,] gridArr, int numOfPoints, bool DistanceCalculationType)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            List<Color> colors = new List<Color>();
            for (int i = 0; i < numOfPoints; i++)
            {
                Color newColor = new Color(Random.value, Random.value, Random.value);
                colors.Add(newColor);
            }

            var pointsArr = new List<Vector2>();

            int totalSize = gridArr.GetLength(1) * gridArr.GetLength(0);

            for (int i = 0; i < numOfPoints; i++)
            {
                int ran = Random.Range(0, totalSize);

                var wantedCoor = new Vector2(ran % gridArr.GetLength(0), ran / gridArr.GetLength(0));

                wantedCoor = new Vector2Int(gridArr[(int)wantedCoor.x, (int)wantedCoor.y].position.x, gridArr[(int)wantedCoor.x, (int)wantedCoor.y].position.y);

                if (pointsArr.Contains(wantedCoor))
                {
                    i--;
                }
                else
                {
                    pointsArr.Add(wantedCoor);
                }
            }


            Parallel.For(0, gridArr.GetLength(1), y =>
            {
                for (int x = 0; x < gridArr.GetLength(0); x++)
                {
                    int closestIndex = 0;
                    float closestDistance = -1;

                    for (int i = 0; i < pointsArr.Count; i++)
                    {
                        if (closestDistance < 0)
                        {
                            if (DistanceCalculationType)
                                closestDistance = DFGeneralUtil.EuclideanDistance2D(pointsArr[i], new Vector2(gridArr[x, y].position.x, gridArr[x, y].position.y));
                            else
                                closestDistance = DFGeneralUtil.ManhattanDistance2D(pointsArr[i], new Vector2(gridArr[x, y].position.x, gridArr[x, y].position.y));
                        }
                        else
                        {
                            float newDist;

                            if (DistanceCalculationType)
                                newDist = DFGeneralUtil.EuclideanDistance2D(pointsArr[i], new Vector2(gridArr[x, y].position.x, gridArr[x, y].position.y));
                            else
                                newDist = DFGeneralUtil.ManhattanDistance2D(pointsArr[i], new Vector2(gridArr[x, y].position.x, gridArr[x, y].position.y));

                            if (closestDistance > newDist)
                            {
                                closestDistance = newDist;
                                closestIndex = i;
                            }
                        }
                    }

                    gridArr[x, y].idx = closestIndex;
                    gridArr[x, y].color = colors[closestIndex];
                }
            });


            GetBoundariesVoronoi(gridArr);


            stopwatch.Stop();
            long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            Console.WriteLine($"Elapsed time for Voronoi Calculation: {elapsedMilliseconds} ms, using normal loop");

        }

        /// <summary>
        /// Looks for the boundaries between the voronoi cells and resets the neighbouring tiles to creates boundries and walls. This is based on the index variable in the DFTile class
        /// </summary>
        /// <param name="gridArr"></param>
        private static void GetBoundariesVoronoi(DFTile[,] gridArr)
        {
            var childPosArry = DFGeneralUtil.childPosArry4Side;

            Parallel.For(0, gridArr.GetLength(1), y =>
            {
                for (int x = 0; x < gridArr.GetLength(0); x++)
                {
                    int wantedIdx = gridArr[x, y].idx;

                    bool sameIdx = true;

                    for (int i = 0; i < childPosArry.Length / 2; i++)
                    {
                        int x_buff = childPosArry[i, 0];
                        int y_buff = childPosArry[i, 1];

                        int[] node_position = { x + x_buff, y + y_buff };

                        if (node_position[0] < 0 || node_position[1] < 0 || node_position[0] >= gridArr.GetLength(0) || node_position[1] >= gridArr.GetLength(1))
                        {
                            continue;
                        }
                        else
                        {
                            if (gridArr[node_position[0], node_position[1]].idx == wantedIdx)
                            {

                            }
                            else
                            {
                                sameIdx = false;
                                break;
                            }
                        }
                    }

                    if (sameIdx)
                    {
                        gridArr[x, y].tileWeight = 1;
                    }
                    else
                    {
                        DFGeneralUtil.ResetTile(gridArr[x, y]);
                    }
                }
            });
        }

        #endregion

        #region Marching Cubes Generation

        public static MarchingCubeClass[,,] ExtrapolateMarchingCubes(DFTile[,] gridArr, int roomHeight = 7)
        {
            var marchingCubesArr = new MarchingCubeClass[gridArr.GetLength(0), gridArr.GetLength(1), roomHeight];

            for (int z = 0; z < marchingCubesArr.GetLength(2); z++)  // this is the heihgt of the room
            {
                for (int y = 0; y < marchingCubesArr.GetLength(1); y++)
                {
                    for (int x = 0; x < marchingCubesArr.GetLength(0); x++)
                    {
                        if (z == 0 || z == marchingCubesArr.GetLength(2) - 1) //we draw everything as this is the ceiling and the floor
                        {
                            if (gridArr[x, y].tileType == DFTile.TileType.WALL || gridArr[x, y].tileType == DFTile.TileType.WALLCORRIDOR)
                            {
                                marchingCubesArr[x, y, z] = new MarchingCubeClass(new Vector3Int(gridArr[x, y].position.x, z, gridArr[x, y].position.y), gridArr[x, y].tileWeight != 0 ? 1 : 0, 0.95f);
                            }
                            else
                            {
                                marchingCubesArr[x, y, z] = new MarchingCubeClass(new Vector3Int(gridArr[x, y].position.x, z, gridArr[x, y].position.y), gridArr[x, y].tileWeight != 0 ? 1 : 0, 0.05f);
                            }
                        }
                        else // this is justt he wall
                        {
                            if (gridArr[x, y].tileType == DFTile.TileType.WALL || gridArr[x, y].tileType == DFTile.TileType.WALLCORRIDOR) // draw everything but the floor
                            {
                                marchingCubesArr[x, y, z] = new MarchingCubeClass(new Vector3Int(gridArr[x, y].position.x, z, gridArr[x, y].position.y), 1, 1);
                            }
                            else // set the floor to 0 
                            {
                                marchingCubesArr[x, y, z] = new MarchingCubeClass(new Vector3Int(gridArr[x, y].position.x, z, gridArr[x, y].position.y), 0, 1);
                            }
                        }
                    }
                }
            }

            return marchingCubesArr;
        }

        public static Mesh MarchingCubesAlgo(MarchingCubeClass[,,] positionVertex, bool inverse = false)
        {
            Mesh mesh = new Mesh();

            mesh.Clear();

            List<int> triangles = new List<int>();
            List<Vector3> vertecies = new List<Vector3>();

            for (int z = 0; z < positionVertex.GetLength(2); z++)
            {
                for (int y = 0; y < positionVertex.GetLength(1); y++)
                {
                    for (int x = 0; x < positionVertex.GetLength(0); x++)
                    {

                        if (x + 1 >= positionVertex.GetLength(0) || y + 1 >= positionVertex.GetLength(1) || z + 1 >= positionVertex.GetLength(2))
                        {
                            continue;
                        }

                        var midPosArr = new Vector3[12]
                        {
                            Vector3.Lerp(  positionVertex[x,y,z].position,              positionVertex[x + 1,y,z].position,      positionVertex[x,y,z].weight / (positionVertex[x,y,z].weight + positionVertex[x + 1,y,z].weight)),    //0   1
                            Vector3.Lerp(  positionVertex[x + 1,y,z].position,          positionVertex[x + 1,y + 1,z].position,  positionVertex[x + 1,y,z].weight / (positionVertex[x + 1,y,z].weight + positionVertex[x + 1,y + 1,z].weight)),   //1   2
                            Vector3.Lerp(  positionVertex[x + 1,y + 1,z].position,      positionVertex[x,y + 1,z].position,      positionVertex[x + 1,y + 1,z].weight / (positionVertex[x + 1,y + 1,z].weight + positionVertex[x,y + 1,z].weight)),       //2   3
                            Vector3.Lerp(  positionVertex[x,y + 1,z].position,          positionVertex[x,y,z].position,          positionVertex[x,y + 1,z].weight / (positionVertex[x,y + 1,z].weight + positionVertex[x,y,z].weight)),           //3   0
                           
                            Vector3.Lerp(  positionVertex[x,y,z + 1].position,          positionVertex[x+1,y,z+1].position,      positionVertex[x,y,z + 1].weight / (positionVertex[x,y,z + 1].weight + positionVertex[x+1,y,z+1].weight)),       //4   5
                            Vector3.Lerp(  positionVertex[x+1,y,z+1].position ,         positionVertex[x + 1,y+1,z+1].position,  positionVertex[x+1,y,z+1].weight / (positionVertex[x+1,y,z+1].weight + positionVertex[x + 1,y+1,z+1].weight)),   //5   6
                            Vector3.Lerp(  positionVertex[x + 1,y+1,z+1].position,        positionVertex[x,y+1,z+1].position,    positionVertex[x + 1,y+1,z+1].weight / (positionVertex[x + 1,y+1,z+1].weight + positionVertex[x,y+1,z+1].weight)),       //6   7
                            Vector3.Lerp(  positionVertex[x,y + 1,z +1].position,          positionVertex[x,y,z+1].position,     positionVertex[x,y + 1,z +1].weight / (positionVertex[x,y + 1,z +1].weight + positionVertex[x,y,z+1].weight)),          //7   4
                            
                            Vector3.Lerp(  positionVertex[x,y,z+1].position,            positionVertex[x,y,z].position,          positionVertex[x,y,z+1].weight / (positionVertex[x,y,z+1].weight + positionVertex[x,y,z].weight)),           //4   0
                            Vector3.Lerp(  positionVertex[x+1,y,z+1].position,          positionVertex[x + 1,y,z].position,      positionVertex[x+1,y,z+1].weight / (positionVertex[x+1,y,z+1].weight + positionVertex[x + 1,y,z].weight)),       //5   1
                            Vector3.Lerp(  positionVertex[x + 1,y+1,z+1].position,        positionVertex[x+1,y+1,z].position,    positionVertex[x + 1,y+1,z+1].weight / (positionVertex[x + 1,y+1,z+1].weight + positionVertex[x+1,y+1,z].weight)),       //6   2
                            Vector3.Lerp(  positionVertex[x,y+1,z+1].position,          positionVertex[x,y + 1,z].position,      positionVertex[x,y+1,z+1].weight / (positionVertex[x,y+1,z+1].weight + positionVertex[x,y + 1,z].weight))          //7   3

                        };

                        int index = positionVertex[x, y, z].state * 1 +
                                        positionVertex[x + 1, y, z].state * 2 +
                                        positionVertex[x + 1, y + 1, z].state * 4 +
                                        positionVertex[x, y + 1, z].state * 8 +
                                        positionVertex[x, y, z + 1].state * 16 +
                                        positionVertex[x + 1, y, z + 1].state * 32 +
                                        positionVertex[x + 1, y + 1, z + 1].state * 64 +
                                        positionVertex[x, y + 1, z + 1].state * 128;


                        for (int i = 0; i < triTable.GetLength(1); i++)
                        {

                            if (triTable[index, i] == -1)
                                break;

                            triangles.Add(vertecies.Count());

                            vertecies.Add(midPosArr[triTable[index, i]]);

                        }
                    }
                }
            }

            if (inverse)
            {
                triangles.Reverse();
            }

            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = vertecies.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            return mesh;

        }

        #endregion

        #region Type and Utility section

        /// <summary>
        /// Given a list it shuffles the elements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        public static void ShuffleList<T>(this IList<T> ts)
        {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = Random.Range(i, count);
                var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }
        }

        public static void SetUpTileCorridorTypesUI(DFTile[,] gridArr, int width)
        {
            SetUpTileTypesCorridor(gridArr);

            for (int i = 0; i < width - 1; i++)
            {
                Parallel.For(0, gridArr.GetLength(1), y =>
                {
                    for (int x = 0; x < gridArr.GetLength(0); x++)
                    {
                        if (gridArr[x, y].tileType == DFTile.TileType.WALLCORRIDOR)
                        {
                            gridArr[x, y].tileType = DFTile.TileType.FLOORCORRIDOR;
                        }
                        if (gridArr[x, y].tileType == DFTile.TileType.FLOORCORRIDOR)
                        {
                        }
                    }
                });

                SetUpTileTypesCorridor(gridArr);
            }

            SetUpTileTypesFloorWall(gridArr);
        }

       
        /// <summary>
        /// call this to set up the tile type, for wall and floor, algo that recognises walls. this recognise
        /// </summary>
        /// <param name="gridArr"></param>
        /// <param name="diagonalTiles">diag tile depends if it uses the 8 or 4 child arr</param>
        public static void SetUpTileTypesFloorWall(DFTile[,] gridArr)
        {
            int[,] childPosArry = new int[0, 0];

            childPosArry = DFGeneralUtil.childPosArry4Side;

            Parallel.For(0, gridArr.GetLength(1), y =>
            {
                for (int x = 0; x < gridArr.GetLength(0); x++)
                {
                    if (gridArr[x, y].tileWeight != 0)
                    {
                        bool wall = false;

                        for (int i = 0; i < childPosArry.Length / 2; i++)
                        {
                            int x_buff = childPosArry[i, 0];
                            int y_buff = childPosArry[i, 1];

                            int[] node_position = { x + x_buff, y + y_buff };

                            if (node_position[0] < 0 || node_position[1] < 0 || node_position[0] >= gridArr.GetLength(0) || node_position[1] >= gridArr.GetLength(1))
                            {
                                wall = true;
                                break;
                            }
                            else
                            {
                                if (gridArr[node_position[0], node_position[1]].tileWeight == 0)
                                {
                                    wall = true;
                                    break;
                                }
                            }
                        }

                        if (wall)
                        {
                            gridArr[x, y].tileType = DFTile.TileType.WALL;
                            gridArr[x, y].tileWeight = 1;
                        }
                        else
                        {
                            gridArr[x, y].tileWeight = 0.5f;
                        }
                    }
                }
            });

            var copyArr = new DFTile[gridArr.GetLength(0), gridArr.GetLength(1)];

            Parallel.For(0, copyArr.GetLength(1), y =>
            {
                for (int x = 0; x < copyArr.GetLength(0); x++)
                {
                    copyArr[x, y] = new DFTile();
                    copyArr[x, y].tileType = gridArr[x, y].tileType;
                    copyArr[x, y].tileWeight = gridArr[x, y].tileWeight;
                }
            });

            Parallel.For(0, copyArr.GetLength(1), y =>
            {
                for (int x = 0; x < copyArr.GetLength(0); x++)
                {
                    if (copyArr[x, y].tileWeight == 0)
                    {
                        int neigh = 0;

                        for (int i = 0; i < childPosArry.Length / 2; i++)
                        {
                            int x_buff = childPosArry[i, 0];
                            int y_buff = childPosArry[i, 1];

                            int[] node_position = { x + x_buff, y + y_buff };

                            if (node_position[0] < 0 || node_position[1] < 0 || node_position[0] >= copyArr.GetLength(0) || node_position[1] >= copyArr.GetLength(1))
                            {
                                continue;
                            }
                            else if (copyArr[node_position[0], node_position[1]].tileType != DFTile.TileType.VOID)
                            {
                                neigh++;
                            }
                        }

                        if (neigh >= 2)
                        {
                            gridArr[x, y].tileWeight = 1;
                            gridArr[x, y].tileType = DFTile.TileType.WALL;
                        }
                    }
                }
            });
        }

        public static void SetUpTileTypesCorridor(DFTile[,] gridArr)
        {
            int[,] childPosArry = new int[0, 0];

            childPosArry = DFGeneralUtil.childPosArry4Side;

            var copyArr = new DFTile[gridArr.GetLength(0), gridArr.GetLength(1)];

            Parallel.For(0, copyArr.GetLength(1), y =>
            {
                for (int x = 0; x < copyArr.GetLength(0); x++)
                {
                    copyArr[x, y] = new DFTile();
                    copyArr[x, y].tileType = gridArr[x, y].tileType;
                    copyArr[x, y].tileWeight = gridArr[x, y].tileWeight;
                }
            });

            Parallel.For(0, copyArr.GetLength(1), y =>
            {
                for (int x = 0; x < copyArr.GetLength(0); x++)
                {
                    copyArr[x, y] = new DFTile();
                    copyArr[x, y].tileType = gridArr[x, y].tileType;
                    copyArr[x, y].tileWeight = gridArr[x, y].tileWeight;

                    if (copyArr[x, y].tileType == DFTile.TileType.FLOORCORRIDOR)
                    {

                        for (int i = 0; i < childPosArry.Length / 2; i++)
                        {
                            int x_buff = childPosArry[i, 0];
                            int y_buff = childPosArry[i, 1];

                            int[] node_position = { x + x_buff, y + y_buff };

                            if (node_position[0] < 0 || node_position[1] < 0 || node_position[0] >= copyArr.GetLength(0) || node_position[1] >= copyArr.GetLength(1))
                            {
                                continue;
                            }
                            else if (copyArr[node_position[0], node_position[1]].tileType == DFTile.TileType.VOID)
                            {
                                gridArr[node_position[0], node_position[1]].tileType = DFTile.TileType.WALLCORRIDOR;
                                gridArr[node_position[0], node_position[1]].tileWeight = 1;
                            }
                        }
                    }
                }
            });
        }

        public static void SetUpCorridorWithPath(List<DFTile> path, float customWeight = 0.75f)
        {
            foreach (var tile in path)
            {
                if (tile.tileType != DFTile.TileType.FLOORROOM)
                    tile.tileType = DFTile.TileType.FLOORCORRIDOR;

                tile.tileWeight = customWeight;
            }
        }

        public static void SaveTileArrayData(DFTile[,] grid, string saveFileName)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();

            // Create a new array to store the data
            SerializableTile[,] serializableMap = new SerializableTile[grid.GetLength(0), grid.GetLength(1)];
            for (int i = 0; i < grid.GetLength(1); i++)
            {
                for (int j = 0; j < grid.GetLength(0); j++)
                {
                    serializableMap[j, i] = new SerializableTile(grid[j, i].position, grid[j, i].tileWeight, grid[j, i].cost, (int)grid[j, i].tileType);
                }
            }

            formatter.Serialize(stream, serializableMap);

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
                AssetDatabase.Refresh();
            }

            if (!AssetDatabase.IsValidFolder("Assets/Resources/Resources_Algorithms"))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Resources_Algorithms");
                AssetDatabase.Refresh();
            }

            if (!AssetDatabase.IsValidFolder("Assets/Resources/Resources_Algorithms/Saved_Gen_Data"))
            {
                AssetDatabase.CreateFolder("Assets/Resources/Resources_Algorithms", "Saved_Gen_Data");
                AssetDatabase.Refresh();
            }

            File.WriteAllBytes(Application.dataPath + "/Resources/Resources_Algorithms/Saved_Gen_Data/" + saveFileName, stream.ToArray());
        }

        public static DFTile[,] LoadTileArrayData(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                EditorUtility.DisplayDialog("Error", $"The file name {fileName} given is not valid", "OK");
                return null;
            }

            string filePath = Application.dataPath + "/Resources/Resources_Algorithms/Saved_Gen_Data/" + fileName;

            if (File.Exists(filePath))
            {
                byte[] data = File.ReadAllBytes(filePath);
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream(data);
                SerializableTile[,] serializableMap = (SerializableTile[,])formatter.Deserialize(stream);

                DFTile[,] map = new DFTile[serializableMap.GetLength(0), serializableMap.GetLength(1)];

                Parallel.For(0, serializableMap.GetLength(1), i =>
                {
                    for (int j = 0; j < serializableMap.GetLength(0); j++)
                    {
                        map[j, i] = new DFTile(serializableMap[j, i].position, serializableMap[j, i].tileWeight, serializableMap[j, i].cost, serializableMap[j, i].tileType);

                        if (map[j, i].tileType == DFTile.TileType.WALLCORRIDOR)
                            map[j, i].tileType = DFTile.TileType.WALL;

                        if (map[j, i].tileType == DFTile.TileType.FLOORCORRIDOR)
                            map[j, i].tileType = DFTile.TileType.FLOORROOM;
                    }
                });

                return map;
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "The file name given is not valie", "OK");
            }
            return null;
        }

        #endregion

    }

    #region classes

    /// <summary>
    /// This is the basic tile to inherit from 
    /// </summary>
    public class DFTile
    {
        public Color32 color;
        public Vector2Int position = Vector2Int.zero;
        public float tileWeight;
        public float cost = 0;
        public int idx = 0;
        public bool visited = false;

        public enum TileType
        {
            VOID,
            FLOORROOM,
            WALL,
            WALLCORRIDOR,
            ROOF,
            FLOORCORRIDOR,
            AVOID
        }

        public TileType tileType = TileType.VOID;

        public DFTile() { }
        public DFTile(DFTile toCopy)
        {
            color = toCopy.color;
            tileType = toCopy.tileType;
            position = toCopy.position;
            cost = toCopy.cost;
            idx = toCopy.idx;
            visited = toCopy.visited;
            tileWeight = toCopy.tileWeight;
        }

        public DFTile(SerialiableVector2Int position, float tileWeight, float cost, int tileType)
        {
            this.tileType = (TileType)tileType;
            this.position = new Vector2Int(position.x, position.y);
            this.cost = cost;
            this.tileWeight = tileWeight;
        }

    }

    #region triangulation classes

    public class Triangle
    {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;

        public Edge[] edges = new Edge[3];

        public Triangle(Vector3 a, Vector3 b, Vector3 c)
        {
            this.a = a;
            this.b = b;
            this.c = c;


            this.edges[0] = new Edge(a, b);
            this.edges[1] = new Edge(b, c);
            this.edges[2] = new Edge(c, a);
        }


        public bool HasVertex(Vector3 point)
        {
            if (a == point || b == point || c == point) { return true; }
            else { return false; }
        }

    }

    public class Edge
    {
        public Vector3[] edge = new Vector3[2];
        public float length;
        public Edge(Vector3 a, Vector3 b)
        {
            edge[0] = a;
            edge[1] = b;

            length = Mathf.Abs(Vector3.Distance(a, b));
        }
    }

    #endregion

    public class AStar_Node
    {

        public DFTile refToBasicTile;
        public AStar_Node parent;

        public float g = 0;
        public float f = 0;
        public float h = 0;

        public AStar_Node(DFTile basicTile)
        {
            refToBasicTile = basicTile;
        }

    }

    public class MarchingCubeClass
    {
        public Vector3Int position;
        public int state;
        public float weight;

        public MarchingCubeClass(Vector3Int position, int state, float weight)
        {
            this.position = position;
            this.state = state;
            this.weight = weight;
        }
    }

    public class DjNode
    {
        public float distance = 99999;
        public DjNode parentDJnode = null;
        public DFTile gridRefTile = null;
        public Vector2Int coord = Vector2Int.zero;
    }

    #endregion
}