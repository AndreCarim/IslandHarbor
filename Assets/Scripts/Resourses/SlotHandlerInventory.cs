using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SlotHandlerInventory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject toolTip;
    private RectTransform backgroundRectTransform;
    private TextMeshProUGUI textMeshPro;
    private RectTransform rectTransform;

    private RectTransform canvasRectTransform;

    private ResourceGenericHandler resource;
    private GameObject player;

    private IEnumerator SetTextDelayed(string toolTipText)
    {
        textMeshPro.SetText(toolTipText);
        textMeshPro.ForceMeshUpdate();

        // Wait for the end of the frame
        yield return null;

        Vector2 textSize = textMeshPro.GetRenderedValues(false);
        Vector2 paddingSize = new Vector2(8, 8);

        backgroundRectTransform.sizeDelta = textSize + paddingSize;
    }

    private void SetText(string toolTipText)
    {
        StartCoroutine(SetTextDelayed(toolTipText));
    }

    private void Update()
    {
        if (toolTip != null && toolTip.activeSelf && rectTransform != null)
        {
            // Update tooltip position only when active
            rectTransform.anchoredPosition = Input.mousePosition / canvasRectTransform.localScale.x;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {        
        if (toolTip != null)
        {
            // Find the components
            backgroundRectTransform = toolTip.transform.Find("BackGround")?.GetComponent<RectTransform>();
            textMeshPro = toolTip.transform.Find("ResourceName")?.GetComponent<TextMeshProUGUI>();
            rectTransform = toolTip.transform.GetComponent<RectTransform>();

            // Add null checks and handle cases where components are not found
            if (backgroundRectTransform == null || textMeshPro == null || rectTransform == null)
            {
                Debug.LogError("Required components not found.");
            }

            SetText(resource.getResourceName());
            toolTip.SetActive(true);

            // Update tooltip position immediately on enter
            rectTransform.anchoredPosition = Input.mousePosition / canvasRectTransform.localScale.x;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {        
        if (toolTip != null)
        {
            toolTip.SetActive(false);
        }
    }

    // Called by the inventory script attached to the player
    public void setOnCreate(GameObject tip, RectTransform canvas, ResourceGenericHandler resource, GameObject player)
    {        
        toolTip = tip;
        canvasRectTransform = canvas;
        this.resource = resource;
        this.player = player;
    }

    public void onClick(){
        player.GetComponent<ResourceInventory>().slotSelected(resource, gameObject);
    }
}
