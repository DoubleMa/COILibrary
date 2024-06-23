using HarmonyLib;
using System;
using System.Reflection;

namespace COILib.Extensions;

public static class HarmonyExtension {

	public static HarmonyMethod ToHarmonyMethod(this MethodInfo m) => new(m);

	public static HarmonyMethod GetHarmonyMethod(this Type t, string method) => new(t, method);

	public static HarmonyMethod GetHarmonyMethod<T>(this T t, string method) => t.GetType().GetHarmonyMethod(method);

	public static MethodInfo GetGenericMethod<T>(this Type t, string method) => t.AtGetMethod(method).MakeGenericMethod(typeof(T));

	public static MethodInfo GetGenericMethod<T>(this object instance, string method) => instance.GetType().GetGenericMethod<T>(method);

	public static ConstructorInfo AtGetConstructor(this Type t, Type[] paramTypes) => AccessTools.Constructor(t, paramTypes) ?? throw new MissingMethodException($"Constructor with params '{paramTypes}' not found in type '{t.FullName}'.");

	public static FieldInfo AtGetField(this Type type, string fieldName) => AccessTools.Field(type, fieldName) ?? throw new MissingFieldException($"Field '{fieldName}' not found in type '{type.FullName}'.");

	public static MethodInfo AtGetMethod(this Type type, string methodName) => AccessTools.Method(type, methodName) ?? throw new MissingMethodException($"Method '{methodName}' not found in type '{type.FullName}'.");

	public static MethodInfo AtGetPropertyGetter(this Type type, string propertyName) => AccessTools.PropertyGetter(type, propertyName) ?? throw new MissingFieldException($"Property getter for '{propertyName}' not found in type '{type.FullName}'.");

	public static MethodInfo AtGetPropertySetter(this Type type, string propertyName) => AccessTools.PropertySetter(type, propertyName) ?? throw new MissingFieldException($"Property setter for '{propertyName}' not found in type '{type.FullName}'.");

	public static T GetField<T>(this Type type, object instance, string fieldName) => (T)type.AtGetField(fieldName).GetValue(instance);

	public static T GetField<T>(this object instance, string fieldName) => instance.GetType().GetField<T>(instance, fieldName);

	public static void SetField<T>(this Type type, object instance, string fieldName, T value) => type.AtGetField(fieldName).SetValue(instance, value);

	public static void SetField<T>(this object instance, string fieldName, T value) => instance.GetType().SetField(instance, fieldName, value);

	public static TU GetFieldRef<T, TU>(this T instance, string fieldName) => AccessTools.FieldRefAccess<T, TU>(instance, fieldName);

	public static void InvokeMethod(this object instance, string methodName, params object[] parameters) => instance.GetType().AtGetMethod(methodName).Invoke(instance, parameters);

	public static TU InvokeMethod<TU>(this object instance, string methodName, params object[] parameters) => (TU)instance.GetType().AtGetMethod(methodName).Invoke(instance, parameters);

	public static void InvokeStaticMethod(this Type type, string methodName, params object[] parameters) => type.AtGetMethod(methodName).Invoke(null, parameters);

	public static TU InvokeStaticMethod<TU>(this Type type, string methodName, params object[] parameters) => (TU)type.AtGetMethod(methodName).Invoke(null, parameters);

	public static T InvokeGetter<T>(this object instance, string propertyName) => (T)instance.GetType().AtGetPropertyGetter(propertyName).Invoke(instance, null);

	public static void InvokeSetter<T>(this object instance, string propertyName, T value) => instance.GetType().AtGetPropertySetter(propertyName).Invoke(instance, [value]);
}