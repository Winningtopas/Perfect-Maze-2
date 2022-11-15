using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIcon : MonoBehaviour
{
    public float frontCameraScale, topDownCameraScale;

    [SerializeField]
    private Transform firstPersonCamera;
    [SerializeField]
    private bool frontView, topDownView;

    [HideInInspector]
    public Vector3 mazeDimensions;

    private RectTransform rectTransform;
    private float maxUISize = 320;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(mazeDimensions.z > 0 && frontView) // if there is a maze
        {
            //float x = (firstPersonCamera.localPosition.z / mazeDimensions.z) * maxUISize - (maxUISize / 2f);
            //float y = (firstPersonCamera.localPosition.y / mazeDimensions.y) * maxUISize - (maxUISize / 2f);            
            float x = (firstPersonCamera.localPosition.z / mazeDimensions.z) * maxUISize - (maxUISize / 2f);
            float y = (firstPersonCamera.localPosition.y / mazeDimensions.y) * frontCameraScale - (frontCameraScale / 2f);
            Vector3 newPosition = new Vector3(x, y, rectTransform.localPosition.z);

            rectTransform.localPosition = newPosition;
        }
        else if(mazeDimensions.y > 0 && topDownView)
        {
            float x = (firstPersonCamera.localPosition.z / mazeDimensions.z) * maxUISize - (maxUISize / 2f);
            float y =  (1f -(firstPersonCamera.localPosition.x / mazeDimensions.x)) * maxUISize - (maxUISize / 2f);
            Vector3 newPosition = new Vector3(x, y, rectTransform.localPosition.z);

            rectTransform.localPosition = newPosition;
        }
    }
}
