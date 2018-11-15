using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NightlyCode.Modules.Dependencies;

namespace NightlyCode.Modules {

    /// <summary>
    /// provides instances of modules when they are requested
    /// </summary>
    public class ModuleProvider {
        readonly Dictionary<Type, Type> implementations = new Dictionary<Type, Type>();
        readonly Dictionary<Type, object> instances = new Dictionary<Type, object>();
        readonly object instancelock = new object();

        /// <summary>
        /// enumeration of all created instances
        /// </summary>
        public IEnumerable<object> Instances
        {
            get
            {
                lock(instancelock)
                    foreach(object instance in instances.Values)
                        yield return instance;
            }
        }

        /// <summary>
        /// adds a type to available implementations
        /// </summary>
        /// <param name="type">type to add</param>
        public void Add(Type type) {
            
            foreach (Type interfacetype in type.GetInterfaces())
                implementations[interfacetype] = type;

            Type basetype = type.BaseType;
            while (basetype != null && basetype != typeof(object)) {
                implementations[basetype] = type;
                basetype = type.BaseType;
            }
        }

        /// <summary>
        /// adds a type to available implementations
        /// </summary>
        /// <typeparam name="T">type to add</typeparam>
        public void Add<T>() {
            Add(typeof(T));
        }

        /// <summary>
        /// get an implementation of a type
        /// </summary>
        /// <typeparam name="T">type of which to get implementation</typeparam>
        /// <returns>instance of specified type</returns>
        public object Get<T>() {
            return Get(typeof(T));
        }

        /// <summary>
        /// gets an implementation of the specified type
        /// </summary>
        /// <param name="type">type to create</param>
        /// <returns></returns>
        public object Get(Type type) {
            lock (instancelock) {
                if (!instances.TryGetValue(type, out object instance)) {
                    HashSet<Type> history = new HashSet<Type>();
                    instances[type] = instance = GetOrCreate(type, history);
                }

                return instance;
            }
        }

        object GetOrCreate(Type type, HashSet<Type> history) {
            if (!implementations.TryGetValue(type, out Type tocreate))
                tocreate = type;

            if (!instances.TryGetValue(tocreate, out object instance)) {
                if (history.Contains(tocreate))
                    throw new DependencyException($"Circular dependency detected when trying to create {tocreate.Name}");
                instances[tocreate] = instance = CreateInstance(tocreate, history);
            }

            return instance;
        }

        IEnumerable<object> CreateParameters(ParameterInfo[] parameters, HashSet<Type> history) {
            foreach (ParameterInfo parameter in parameters)
                yield return GetOrCreate(parameter.ParameterType, history);
        }

        object CreateInstance(Type type, HashSet<Type> history) {
            history.Add(type);
            ConstructorInfo constructor = type.GetConstructors().First();
            object[] parameters = CreateParameters(constructor.GetParameters(), history).ToArray();
            return constructor.Invoke(parameters);
        }
    }
}