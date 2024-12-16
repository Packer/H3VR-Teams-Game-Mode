using FistVR;
using UnityEngine;

public class TGM_FVRPointableButton : FVRPointableButton
{

    public void OnValidate()
    {
        BoxCollider box = GetComponent<BoxCollider>();

        if (box != null)
        {
            if (box.enabled == false || box.size.x > 1)
                return;

            RectTransform rect = GetComponent<RectTransform>();
            box.size = new Vector3(rect.sizeDelta.x, rect.sizeDelta.y, 1);
        }
    }
}
