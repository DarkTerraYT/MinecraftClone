using System;
using System.Collections.Generic;
using System.Linq;

namespace MinecraftClone.Core.Reflection;

public static class ReflectionHelper
{
    public static IEnumerable<Type> FindAll<T>() => FindAll(typeof(T));
    public static IEnumerable<Type> FindAll(Type baseType)
    {
        return AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes().Where(type => 
            type.IsClass &&
            type.IsAssignableFrom(baseType) &&
            !type.IsAbstract));
    }
}