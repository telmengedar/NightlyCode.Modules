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
        readonly IModuleContext context;

        readonly Dictionary<Type, Type> implementations = new Dictionary<Type, Type>();
        readonly Dictionary<Type, object> instances = new Dictionary<Type, object>();
        readonly object instancelock = new object();

        /// <summary>
        /// creates a new <see cref="ModuleProvider"/>
        /// </summary>
        /// <param name="context">access to module context</param>
        public ModuleProvider(IModuleContext context) {
            this.context = context;
        }

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
        /// determines whether module provider has created an instance for the specified type
        /// </summary>
        /// <typeparam name="T">type to check</typeparam>
        /// <returns>true if provider contains an instance for the specified type, false otherwise</returns>
        public bool HasInstance<T>() {
            return HasInstance(typeof(T));
        }

        /// <summary>
        /// determines whether module provider has created an instance for the specified type
        /// </summary>
        /// <param name="type">type to check</param>
        /// <returns>true if provider contains an instance for the specified type, false otherwise</returns>
        public bool HasInstance(Type type) {
            lock (instancelock)
                return instances.ContainsKey(type);
        }

        void AddType(Type requesttype, Type instancetype, object instance = null) {
            implementations[requesttype] = instancetype;
            if (instance != null)
                lock(instancelock)
                    instances[requesttype] = instance;
        }

        void TraverseTypes(Type type, object instance = null) {
            foreach (Type interfacetype in type.GetInterfaces())
                AddType(interfacetype, type, instance);

            Type basetype = type;
            while (basetype != null && basetype != typeof(object))
            {
                AddType(basetype, type, instance);
                basetype = basetype.BaseType;
            }
        }

        /// <summary>
        /// adds an instance to the module provider
        /// </summary>
        /// <param name="instance">instance to add</param>
        public void AddInstance(object instance) {
            TraverseTypes(instance.GetType(), instance);
        }

        /// <summary>
        /// adds a type to available implementations
        /// </summary>
        /// <param name="type">type to add</param>
        public void Add(Type type) {
            TraverseTypes(type);
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

            ModuleInformation information = context.GetModuleInformation(type);
            if (information.Provider != null)
                return information.Provider(this);

            ConstructorInfo constructor;
            try {
                constructor = type.GetConstructors().First();
            }
            catch (Exception e) {
                throw new ModuleCreateException($"Unable find any constructor for '{type.Name}'", type, e);
            }
                
            object[] parameters = CreateParameters(constructor.GetParameters(), history).ToArray();
            try {
                return constructor.Invoke(parameters);
            }
            catch (TargetInvocationException e) {
                if (e.InnerException != null)
                    throw new ModuleCreateException($"Unable to invoke constructor for '{type.Name}", type, e.InnerException);
                throw new ModuleCreateException($"Unable to invoke constructor for '{type.Name}", type, e);
            }
        }
    }
}