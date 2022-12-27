using UnityEngine;
using UnityEngine.EventSystems;

public class InnerInput : BaseInput
{
    public override Vector2 mousePosition => Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

    void Start()
    {
        GetComponent<StandaloneInputModule>().inputOverride = this;
    }
}
