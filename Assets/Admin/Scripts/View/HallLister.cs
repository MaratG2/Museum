using System;
using System.Collections.Generic;
using Admin.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Admin.View
{
    /// <summary>
    /// Отвечает за создание и удаление интерфейсных элементов – списка доступных залов.
    /// </summary>
    public class HallLister : MonoBehaviour
    {
        [SerializeField] private Button _hallListingPrefab;
        [SerializeField] private RectTransform _hallListingsParent;

        public void ClearAllHallListings()
        {
            for (int i = 0; i < _hallListingsParent.childCount; i++)
                Destroy(_hallListingsParent.GetChild(i).gameObject);
        }

        public void CreateAllHallListings(List<Hall> halls, Action<int> SelectHall)
        {
            foreach (var hall in halls)
            {
                var newInstance = Instantiate(_hallListingPrefab, Vector3.zero, Quaternion.identity,
                    _hallListingsParent);
                newInstance.gameObject.name = hall.hnum + " - " + hall.name;
                newInstance.GetComponentInChildren<TextMeshProUGUI>().text = hall.name;
                newInstance.onClick.AddListener(() =>
                    SelectHallFromButton(hall.hnum, newInstance.gameObject, SelectHall));
            }
        }

        private void SelectHallFromButton(int onum, GameObject linkGO, Action<int> SelectHall)
        {
            for (int i = 0; i < _hallListingsParent.transform.childCount; i++)
            {
                if (_hallListingsParent.GetChild(i).gameObject == linkGO)
                {
                    _hallListingsParent.GetChild(i).GetComponent<Image>().color = Color.green;
                    _hallListingsParent.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                }
                else
                {
                    _hallListingsParent.GetChild(i).GetComponent<Image>().color = Color.gray;
                    _hallListingsParent.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                }
            }

            SelectHall?.Invoke(onum);
        }
    }
}