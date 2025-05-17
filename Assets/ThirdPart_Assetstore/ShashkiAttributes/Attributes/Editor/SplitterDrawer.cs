using UnityEditor;
using UnityEngine;

namespace Shashki.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(SpliteAttribute))]
    public sealed class SplitterDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            var splite = attribute as SpliteAttribute;

            Rect spliteRect = new Rect(position.xMin, position.yMin + splite.splitterSpacing, position.width, splite.splitterSize);

            EditorGUI.DrawRect(spliteRect, splite.splitterColor);
        }

        public override float GetHeight()
        {
            var splite = attribute as SpliteAttribute;

            float totalSpacing = splite.splitterSpacing + splite.splitterSize + splite.splitterSpacing;

            return totalSpacing;
        }
    }
}
