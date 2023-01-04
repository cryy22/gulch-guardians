using System.Linq;
using Crysc.Common;
using UnityEngine;

namespace GulchGuardians.Constants
{
    public static class Names
    {
        private static readonly InstanceCacher<TextAsset> _cacher = new(AssetAddresses.UnitNamesList);
        private static TextAsset NamesFile => _cacher.I;

        public static string RandomName()
        {
            int numLines = NamesFile.text.Count(c => c == '\n');
            int randomLine = Random.Range(minInclusive: 0, maxExclusive: numLines);
            var currentLine = 0;
            var name = "";

            foreach (char c in NamesFile.text)
            {
                if (c == '\n')
                {
                    currentLine++;
                    if (currentLine > randomLine) break;
                    continue;
                }

                if (currentLine == randomLine) name += c;
            }

            return name;
        }
    }
}
