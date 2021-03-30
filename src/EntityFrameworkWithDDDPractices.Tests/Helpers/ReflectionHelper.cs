using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EntityFrameworkWithDDDPractices.Tests.Helpers
{
    public static class ReflectionHelper
    {
        private static Lazy<Type[]> _allTypes = new Lazy<Type[]>(GetAllTypes);

        public static MethodInfo GetPrivateStaticMethod<T>(string methodName)
        {
            return typeof(T).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
        }

        public static bool IsConcrete(this Type type)
        {
            return type.IsClass && !type.IsAbstract && !type.ContainsGenericParameters;
        }

        internal static IReadOnlyCollection<(Type concreteType, Type genericType)> GetAllConcreteTypesImplementingGenericInterface(Type genericType)
        {
            bool IsGenericInterface(Type type)
            {
                return type.IsGenericType && type.GetGenericTypeDefinition() == genericType;
            }
            return _allTypes.Value
                .SelectMany(type =>
                    type.GetInterfaces()
                        .Where(IsGenericInterface)
                        .Select(@interface => (type, @interface.GenericTypeArguments[0])))
                .ToArray();
        }

        private static Type[] GetAllTypes()
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(IsConcrete)
                .ToArray();
        }
    }
}