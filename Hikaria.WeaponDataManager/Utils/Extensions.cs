using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hikaria.WeaponDataLoader.Utils
{
    internal static class Extensions
    {
        public static Il2CppSystem.Collections.Generic.List<T> ToIl2CppList<T>(this System.Collections.Generic.List<T> list)
        {
            Il2CppSystem.Collections.Generic.List<T> result = new();
            foreach (T item in list)
            {
                result.Add(item);
            }
            return result;
        }
    }
}
