using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable 0649
public class ActiveObjectView : MonoBehaviour
{
    #region Singleton

    private static ActiveObjectView _instance;
    public static ActiveObjectView Instance
    {
        get
        {
            if (_instance == null)
            {
                print("Instance");
            }
            return _instance;
        }
    }
    #endregion

    [SerializeField]
    private Transform CustomObj;
    [SerializeField]
    private GameObject Camera;
    [SerializeField]
    private GameObject ParentObj;
    GameObject objectInst;
    private void Awake()
    {
        _instance = gameObject.GetComponent<ActiveObjectView>();
    }
    public void CreateObj(GameObject obj)
    {
        Camera.SetActive(true);
        ParentObj.SetActive(true);
        var yPos = CustomObj.position.y - obj.transform.localScale.y*obj.GetComponent<BoxCollider>().size.y/2;
        var Pos = new Vector3(CustomObj.position.x, yPos, CustomObj.position.z);
        objectInst = Instantiate(obj, Pos, Quaternion.identity, CustomObj);

    }
    public void DestroyObj()
    {
        Camera.SetActive(false);
        ParentObj.SetActive(false);
        if(objectInst!=null)
            Destroy(objectInst);
    }
}
#pragma warning restore 0649