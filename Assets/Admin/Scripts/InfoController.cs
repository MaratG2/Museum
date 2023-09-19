using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Admin.Edit;
using Admin.Utility;
using UnityEngine;

/// <summary>
/// Отвечает за управление информационным стендом: добавление и удаление его частей, сериализация всей информации в JSON для последующего сохранения в БД.
/// </summary>
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

        if (string.IsNullOrWhiteSpace(newAllJson))
            return;
        string descriptionWithNewLines = FormatNewLines(newAllJson);
        for (int k = 1; k < descriptionWithNewLines.Length - 1; k++)
        {
            if (descriptionWithNewLines[k] == '"' || descriptionWithNewLines[k] == '\'')
            {
                if (descriptionWithNewLines[k - 1] == '{' ||
                    descriptionWithNewLines[k + 1] == '}' ||
                    descriptionWithNewLines[k - 1] == ',' ||
                    descriptionWithNewLines[k + 1] == ',' ||
                    descriptionWithNewLines[k - 1] == ':' ||
                    descriptionWithNewLines[k + 1] == ':')
                    continue;
                descriptionWithNewLines = ReplaceAt(descriptionWithNewLines, k, ' ');
            }
        }
        Debug.Log(descriptionWithNewLines);
        InfoPart.InfoPartData[] partsDatas = JsonHelper.FromJson<InfoPart.InfoPartData>(descriptionWithNewLines);
        foreach (var pd in partsDatas)
            CreateNewInfoPart(pd);
    }
    
    public string ReplaceAt(string input, int index, char newChar)
    {
        if (input == null)
        {
            throw new ArgumentNullException("input");
        }
        StringBuilder builder = new StringBuilder(input);
        builder[index] = newChar;
        return builder.ToString();
    }
    
    private string FormatNewLines(string description)
    {
        return description.Replace("\n", "\\n")
            .Replace("\r", "\\r").Replace("\t", "\\t");
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
        
        for(int i = 0; i < partsDatas.Length; i++)
            partsDatas[i].desc = partsDatas[i].desc.Replace("\"", "'");
        
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
