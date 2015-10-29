using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using System.Security.Permissions;

namespace MemorySharp.Helpers
{
    /// <summary>
    ///     The reflection helper.
    /// </summary>
    [SecurityCritical]
    public static class ReflectionHelper
    {
        #region Methods
        /// <summary>
        ///     Binds the method to delegate.
        /// </summary>
        /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
        /// <param name="methodInfo">The method info.</param>
        /// <returns>The delegate</returns>
        [ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
        public static TDelegate BindMethodToDelegate<TDelegate>(MethodInfo methodInfo) where TDelegate : class
        {
            Type[] typeArray;
            Type type;

            ExtractDelegateSignature(typeof (TDelegate), out typeArray, out type);

            var name = "BindMethodToDelegate_" + methodInfo.Name;
            var returnType = type;
            var parameterTypes = typeArray;

            var method = new DynamicMethod(name, returnType, parameterTypes, true /* RestrictedSkipVisibility */);

            var ilgenerator = method.GetILGenerator();
            for (var i = 0; i < typeArray.Length; i++)
            {
                ilgenerator.Emit(OpCodes.Ldarg, (short) i);
            }

            ilgenerator.Emit(OpCodes.Callvirt, methodInfo);
            ilgenerator.Emit(OpCodes.Ret);

            return method.CreateDelegate(typeof (TDelegate)) as TDelegate;
        }

        /// <summary>
        ///     Finds the constructor.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="isStatic">if set to <c>true</c> [is static].</param>
        /// <param name="argumentTypes">The argument types.</param>
        /// <returns>The constructor info</returns>
        public static ConstructorInfo FindConstructor(Type type, bool isStatic, Type[] argumentTypes)
        {
            var info = type.GetConstructor(GetBindingFlags(isStatic), null, argumentTypes, null);

            return info;
        }

        /// <summary>
        ///     Finds the field.
        /// </summary>
        /// <param name="containingType">Type of the containing.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="isStatic">if set to <c>true</c> [is static].</param>
        /// <returns>The field info</returns>
        public static FieldInfo FindField(Type containingType, string fieldName, bool isStatic)
        {
            var field = containingType.GetField(fieldName, GetBindingFlags(isStatic));

            return field;
        }

        /// <summary>
        ///     Finds the method.
        /// </summary>
        /// <param name="containingType">Type of the containing.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="isStatic">if set to <c>true</c> [is static].</param>
        /// <param name="argumentTypes">The argument types.</param>
        /// <returns>The method info</returns>
        public static MethodInfo FindMethod(Type containingType, string methodName, bool isStatic, Type[] argumentTypes)
        {
            var info = containingType.GetMethod(methodName, GetBindingFlags(isStatic), null, argumentTypes, null);

            return info;
        }

        /// <summary>
        ///     Makes the delegate.
        /// </summary>
        /// <typeparam name="T">The entity</typeparam>
        /// <param name="method">The method.</param>
        /// <returns>The delegate to the method</returns>
        public static T MakeDelegate<T>(MethodInfo method) where T : class
        {
            return MakeDelegate<T>(null, method);
        }

        /// <summary>
        ///     Makes the delegate.
        /// </summary>
        /// <typeparam name="T">The entity</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="method">The method.</param>
        /// <returns>
        ///     The created delegate
        /// </returns>
        public static T MakeDelegate<T>(object target, MethodInfo method) where T : class
        {
            return MakeDelegate(typeof (T), target, method) as T;
        }

        /// <summary>
        ///     Makes the delegate.
        /// </summary>
        /// <param name="delegateType">Type of the delegate.</param>
        /// <param name="target">The target.</param>
        /// <param name="method">The method.</param>
        /// <returns>The created delegate</returns>
        [ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
        public static Delegate MakeDelegate(Type delegateType, object target, MethodInfo method)
        {
            return Delegate.CreateDelegate(delegateType, target, method);
        }

        /// <summary>
        ///     Makes the fast create delegate.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance.</typeparam>
        /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
        /// <param name="methodInfo">The method info.</param>
        /// <returns>The method delegate</returns>
        [ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
        public static Func<TInstance, TDelegate> MakeFastCreateDelegate<TInstance, TDelegate>(MethodInfo methodInfo)
            where TInstance : class
            where TDelegate : class
        {
            var name = "FastCreateDelegate_" + methodInfo.Name;
            var returnType = typeof (TDelegate);
            var parameterTypes = new[] {typeof (TInstance)};

            var method = new DynamicMethod(name, returnType, parameterTypes, true /* skipVisibility */);

            var constructor = typeof (TDelegate).GetConstructor(new[] {typeof (object), typeof (IntPtr)});
            Debug.Assert(constructor != null, "constructor != null");

            var ilgenerator = method.GetILGenerator();
            ilgenerator.Emit(OpCodes.Ldarg_0);
            ilgenerator.Emit(OpCodes.Dup);
            ilgenerator.Emit(OpCodes.Ldvirtftn, methodInfo);

            ilgenerator.Emit(OpCodes.Newobj, constructor);
            ilgenerator.Emit(OpCodes.Ret);

            return (Func<TInstance, TDelegate>) method.CreateDelegate(typeof (Func<TInstance, TDelegate>));
        }

        /// <summary>
        ///     Makes the fast new object.
        /// </summary>
        /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>The entity instance delegate</returns>
        [ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
        public static TDelegate MakeFastNewObject<TDelegate>(Type type) where TDelegate : class
        {
            Type[] typeArray;
            Type type2;
            ExtractDelegateSignature(typeof (TDelegate), out typeArray, out type2);

            var type3 = type;

            var argumentTypes = typeArray;
            var con = FindConstructor(type3, false /* isStatic */, argumentTypes);

            var name = "MakeFastNewObject_" + type.Name;
            var returnType = type2;
            var parameterTypes = typeArray;

            var method = new DynamicMethod(name, returnType, parameterTypes, true /* RestrictedSkipVisibility */);
            var ilgenerator = method.GetILGenerator();
            for (var i = 0; i < typeArray.Length; i++)
            {
                ilgenerator.Emit(OpCodes.Ldarg, (short) i);
            }

            ilgenerator.Emit(OpCodes.Newobj, con);
            ilgenerator.Emit(OpCodes.Ret);

            return method.CreateDelegate(typeof (TDelegate)) as TDelegate;
        }

        /// <summary>
        ///     Reads the field.
        /// </summary>
        /// <param name="fieldInfo">The field info.</param>
        /// <param name="target">The target.</param>
        /// <returns>The field value</returns>
        [ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
        public static object ReadField(FieldInfo fieldInfo, object target)
        {
            return fieldInfo.GetValue(target);
        }

        /// <summary>
        ///     Writes the field.
        /// </summary>
        /// <param name="fieldInfo">The field info.</param>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        [ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
        public static void WriteField(FieldInfo fieldInfo, object target, object value)
        {
            fieldInfo.SetValue(target, value);
        }

        /// <summary>
        ///     Gets the binding flags.
        /// </summary>
        /// <param name="isStatic">if set to <c>true</c> [is static].</param>
        /// <returns>The Binding flags</returns>
        private static BindingFlags GetBindingFlags(bool isStatic)
        {
            return ((isStatic ? BindingFlags.Static : BindingFlags.Instance) | BindingFlags.NonPublic) |
                   BindingFlags.Public;
        }

        /// <summary>
        ///     Extracts the delegate signature.
        /// </summary>
        /// <param name="delegateType">Type of the delegate.</param>
        /// <param name="argumentTypes">The argument types.</param>
        /// <param name="returnType">Type of the return.</param>
        private static void ExtractDelegateSignature(IReflect delegateType, out Type[] argumentTypes,
            out Type returnType)
        {
            var method = delegateType.GetMethod("Invoke",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            argumentTypes = Array.ConvertAll(method.GetParameters(), pInfo => pInfo.ParameterType);

            returnType = method.ReturnType;
        }

        /// <summary>
        ///     Creates the instance.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <returns>
        ///     The new instance of specified type
        /// </returns>
        public static object CreateInstance(this Type type, params object[] args)
        {
            if (type == null) return null;
            if (args != null && args.Length > 0)
            {
                return Activator.CreateInstance(
                    type,
                    BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    args,
                    null,
                    null);
            }

            return Activator.CreateInstance(type);
        }

        /// <summary>
        ///     The get custom attribute property value.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="func">
        ///     The func.
        /// </param>
        /// <param name="defaultValue">
        ///     The defaultValue if attribute doesn't exist
        /// </param>
        /// <typeparam name="TAttribute">
        ///     The Attribute type
        /// </typeparam>
        /// <typeparam name="TResult">
        ///     The Result type
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TResult" />.
        /// </returns>
        public static TResult GetCustomAttributePropertyValue<TAttribute, TResult>(
            this Type type,
            Func<TAttribute, TResult> func,
            TResult defaultValue = default(TResult)) where TAttribute : Attribute
        {
            IList<TAttribute> attrList;
            if (!CustomAttributes<TAttribute>.NotInherited.TryGetValue(type.GUID, out attrList))
            {
                CustomAttributes<TAttribute>.RegisterType(type);
            }

            if (!CustomAttributes<TAttribute>.NotInherited.TryGetValue(type.GUID, out attrList)) return defaultValue;
            var attr = attrList.FirstOrDefault();

            return attr != null ? func(attr) : defaultValue;
        }

        /// <summary>
        ///     Creates the instance.
        /// </summary>
        /// <typeparam name="T">
        ///     The type
        /// </typeparam>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <returns>
        ///     The new instance of specified type
        /// </returns>
        public static T CreateInstance<T>(this Type type, params object[] args) where T : class
        {
            return type.CreateInstance(args) as T;
        }
        #endregion

        #region Nested
        /// <summary>
        ///     The custom attributes.
        /// </summary>
        /// <typeparam name="TAttribute">
        ///     The attribute type
        /// </typeparam>
        public static class CustomAttributes<TAttribute>
            where TAttribute : Attribute
        {
            #region  Fields
            /// <summary>
            ///     The inherited.
            /// </summary>
            public static readonly ConcurrentDictionary<Guid, IList<TAttribute>> Inherited;

            /// <summary>
            ///     The not inherited.
            /// </summary>
            public static readonly ConcurrentDictionary<Guid, IList<TAttribute>> NotInherited;
            #endregion

            #region Constructors
            /// <summary>
            ///     Initializes static members of the <see cref="CustomAttributes{TAttribute}" /> class.
            /// </summary>
            static CustomAttributes()
            {
                NotInherited = new ConcurrentDictionary<Guid, IList<TAttribute>>();
                Inherited = new ConcurrentDictionary<Guid, IList<TAttribute>>();
            }
            #endregion

            #region Methods
            /// <summary>
            ///     The register type.
            /// </summary>
            /// <param name="type">
            ///     The type.
            /// </param>
            public static void RegisterType(Type type)
            {
                var guid = type.GUID;

                if (!NotInherited.ContainsKey(guid))
                {
                    NotInherited[guid] = type.GetCustomAttributes<TAttribute>(false).ToArray();
                }

                if (!Inherited.ContainsKey(guid))
                {
                    Inherited[guid] = type.GetCustomAttributes<TAttribute>(true).ToArray();
                }
            }

            /// <summary>
            ///     The register type.
            /// </summary>
            /// <typeparam name="T">
            /// </typeparam>
            public static void RegisterType<T>() where T : class
            {
                RegisterType(typeof (T));
            }

            /// <summary>
            ///     The register types.
            /// </summary>
            /// <param name="types">
            ///     The types.
            /// </param>
            public static void RegisterTypes(params Type[] types)
            {
                if (types != null)
                {
                    foreach (var type in types)
                    {
                        RegisterType(type);
                    }
                }
            }

            public static void RegisterByAssembly(Assembly assembly = null, Func<Type, bool> predicate = null)
            {
                assembly = assembly ?? Assembly.GetExecutingAssembly();

                if (predicate == null)
                {
                    predicate = t => t.GetCustomAttribute<TAttribute>() != null;
                }

                RegisterTypes(assembly.GetTypes().Where(predicate).ToArray());
            }
            #endregion
        }
        #endregion
    }
}