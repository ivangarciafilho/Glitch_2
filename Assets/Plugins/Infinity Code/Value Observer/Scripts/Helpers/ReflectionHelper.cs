/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System.Reflection;

namespace InfinityCode.Observers
{
    public static class ReflectionHelper
    {
        public const BindingFlags BindingFlags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
    }
}