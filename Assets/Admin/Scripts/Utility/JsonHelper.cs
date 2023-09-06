using System;
using UnityEngine;

namespace Admin.Utility
{
    /// <summary>
    /// Отвечает за сериализацию и десериализацию массивов в JSON и обратно.
    /// </summary>
    /// <remarks>
    /// Доступный по умолчанию в Unity класс JsonUtility не способен корректно это делать.
    /// </remarks>
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            if (wrapper == null)
                return new T[] {};
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}