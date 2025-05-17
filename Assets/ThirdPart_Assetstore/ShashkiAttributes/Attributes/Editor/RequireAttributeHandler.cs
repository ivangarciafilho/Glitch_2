using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Shashki.Attributes.Editor
{
    [InitializeOnLoad]
    public static class RequireAttributeHandler
    {
        static RequireAttributeHandler()
        {
            EditorApplication.delayCall += OnScriptsReloaded;
        }

        private static void OnScriptsReloaded()
        {
            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(assembly => assembly.GetTypes())
                                .Where(type => type.IsClass && type.IsSubclassOf(typeof(UnityEngine.MonoBehaviour)));

            foreach (var type in allTypes)
            {
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    var attributes = field.GetCustomAttributes(false);

                    foreach (var attribute in attributes)
                    {
                        var attributeType = attribute.GetType();
                        var requireAttribute = attributeType.GetCustomAttribute<RequireAttributeAttribute>();

                        if (requireAttribute != null)
                        {
                            foreach (var requiredType in requireAttribute.RequiredAttributes)
                            {
                                if (!attributes.Any(attr => attr.GetType() == requiredType))
                                {
                                    if (requiredType == typeof(SerializeField) || field.IsPublic) continue;

                                    if (requiredType == typeof(SerializeField))
                                    {
                                        throw new MissingMemberException($"Field '{field.Name}' in class '{type.Name}' requires the attribute '{requiredType.Name}' when using '{attributeType.Name}'. Add this attribute(s) or make the {field.Name} public.");
                                    }

                                    throw new MissingMemberException($"Field '{field.Name}' in class '{type.Name}' requires the attribute '{requiredType.Name}' when using '{attributeType.Name}'.");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}