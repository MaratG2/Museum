using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InfoController : MonoBehaviour
{
    [SerializeField] private InfoPart _infoPartPrefab;
    [SerializeField] private RectTransform _parentForInfoPart;
    private string _allJsonData;

    public string AllJsonData
    {
        get => _allJsonData;
        set => _allJsonData = value;
    }

    public void Setup(string newAllJson)
    {
        for (int i = 0; i < _parentForInfoPart.childCount - 1; i++)
            Destroy(_parentForInfoPart.GetChild(i).gameObject);

        AllJsonData = newAllJson;
        InfoPart.InfoPartData[] partsDatas = JsonHelper.FromJson<InfoPart.InfoPartData>(AllJsonData);
        foreach (var pd in partsDatas)
            CreateNewInfoPart(pd);
    }
    
    public void InfoPartsChanged(InfoPart partDeleted = null)
    {
        InfoPart.InfoPartData[] partsDatas = {};
        List<InfoPart> childParts = _parentForInfoPart.GetComponentsInChildren<InfoPart>().ToList();
        List<InfoPart.InfoPartData> partsDatasList = new List<InfoPart.InfoPartData>();
        foreach (var cp in childParts)
            if(partDeleted == null || cp != partDeleted)
                partsDatasList.Add(cp.InfoData);
        partsDatas = partsDatasList.ToArray();
        AllJsonData = JsonHelper.ToJson(partsDatas);
    }

    public void CreateNewInfoPart()
    {
        var newPart = Instantiate(_infoPartPrefab, _parentForInfoPart);
        newPart.transform.SetSiblingIndex(_parentForInfoPart.childCount - 2);
    }
    public void CreateNewInfoPart(InfoPart.InfoPartData newData)
    {
        var newPart = Instantiate(_infoPartPrefab, _parentForInfoPart);
        newPart.transform.SetSiblingIndex(_parentForInfoPart.childCount - 2);
        newPart.InfoData = newData;
        newPart.ApplyNewData();
    }
}
