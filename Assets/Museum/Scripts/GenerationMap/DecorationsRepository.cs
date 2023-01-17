using System;
using System.Collections.Generic;
using UnityEngine;

namespace Museum.Scripts.GenerationMap
{
    public class DecorationsRepository : MonoBehaviour
    {
        private GameObject _repository;
        private Dictionary<string, GameObject> _decorationDictionary;
        
        public void Start()
        {
            _repository = gameObject;
            _decorationDictionary = GetDecorationsDictionary();
        }

        public GameObject GetDecorationGOByName(string nameDecoration)
        {
            if (string.IsNullOrEmpty(nameDecoration))
            {
                Debug.LogWarning("Attempt to search for a decoration with an empty name");
                return null;
            }

            if (!_decorationDictionary.TryGetValue(nameDecoration, out var go))
            {
                Debug.LogWarning($"A decoration with the name was not found:{nameDecoration}");
                return null;
            }

            return go;
        }

        private Dictionary<string, GameObject> GetDecorationsDictionary()
        {
            var decorationDict = new Dictionary<string, GameObject>(); 
            foreach (Transform child in _repository.transform)
            {
                if(!child.TryGetComponent(out Decoration decoration))
                    continue;
                if(string.IsNullOrEmpty(decoration.name))
                    continue;
                decorationDict.Add(decoration.name, child.gameObject);
            }

            return decorationDict;
        }
    }
}
