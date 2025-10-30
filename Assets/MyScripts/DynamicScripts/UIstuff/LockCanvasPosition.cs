using UnityEngine;

public class LockCanvasPosition : MonoBehaviour
{
    public RectTransform uiElementToCenter;

    [SerializeField] private string TagForWhereToCenter;
    private void Awake(){
        uiElementToCenter = GameObject.FindGameObjectWithTag(TagForWhereToCenter).GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        if (uiElementToCenter != null)
        {
            uiElementToCenter.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
            transform.position = uiElementToCenter.position;
        }
    }
}
