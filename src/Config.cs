using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Frogger
{
    class Config
    {

        /// <summary>
        /// jaka czesc zabki musi byc podparta zeby nie zatonela
        /// </summary>
        public const float FrogFloorArea=0.65f;

        /// <summary>
        /// czy wyśietlać informacje pomocne w debugowaniu
        /// </summary>
        public static bool IsDebug=true;

        /// <summary>
        /// 
        /// </summary>
        /// <returns>katalog z danymi gry</returns>
        public static string GetDataDir()
        {
            return "data" + Path.DirectorySeparatorChar;
        }
    }
}
