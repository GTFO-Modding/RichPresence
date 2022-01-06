using UnityEngine;

namespace RichPresence.Utils
{
    static class Extension
    {
        public static T Instantiate<T>(this GameObject gameObject, string name) where T : Component
        {
            var newGameObject = GameObject.Instantiate(gameObject, gameObject.transform.parent, false);
            newGameObject.name = name;
            T component = newGameObject.GetComponent<T>();
            return component;
        }
    }
}