using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace xpath.org.eclipse.wst.xml.xpath2.processor.@internal.function
{
    public class GenericIComparer
    {
        private readonly static Dictionary<Type, Dictionary<Tuple<string, Type[]>, RuntimeMethodHandle>> comparers =
            new Dictionary<Type, Dictionary<Tuple<string, Type[]>, RuntimeMethodHandle>>();

        private MethodBase _handle;

        private GenericIComparer(MethodBase handle)
        {
            _handle = handle;
        }

        public static MethodBase GetComparer<T>(string propertyName, Type[] args)
        {
            var type_of_t = typeof(T);
            if (!comparers.ContainsKey(type_of_t))
                comparers.Add(type_of_t, new Dictionary<Tuple<string, Type[]>, RuntimeMethodHandle>());
            var x = comparers.TryGetValue(type_of_t, out Dictionary<Tuple<string, Type[]>, RuntimeMethodHandle> foo);
            if (foo == null)
                throw new Exception();
            if (!foo.ContainsKey(new Tuple<string, Type[]>(propertyName, args)))
                foo.Add(new Tuple<string, Type[]>(propertyName, args),
                    typeof(T).GetMethod(propertyName, args).MethodHandle);
            return 
                MethodInfo.GetMethodFromHandle(comparers[typeof(T)][new Tuple<string, Type[]>(propertyName, args)]);
        }
    }
}

