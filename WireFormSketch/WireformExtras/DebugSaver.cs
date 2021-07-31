using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wireform;

namespace Wireform.Sketch.WireformExtras
{
    internal class DebugSaver : ISaver
    {

        public string GetJson()
        {
            return "";
        }

        public string WriteJson(string json, string locationIdentifier)
        {
            Debug.WriteLine(json);
            return "test";
        }
    }
}