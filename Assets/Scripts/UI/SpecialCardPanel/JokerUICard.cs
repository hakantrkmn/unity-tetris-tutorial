using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

// IBeginDragHandler arayüzünü ekliyoruz.
public class JokerUICard : MonoBehaviour ,IPointerClickHandler,IBeginDragHandler,IEndDragHandler,IDragHandler,IDropHandler
{
    public Image artwork;
    public PowerBase power;

    public JokerPanelManager jokerPanelManager;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        // Sürükleme sırasında raycast'i devre dışı bırakabilmek için CanvasGroup bileşenine ihtiyacımız var.
        // Eğer bu nesnede yoksa, bir tane ekleyelim.
        if (!TryGetComponent<CanvasGroup>(out canvasGroup))
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Sürükleme başladığında, bu kartın mouse olaylarını "bloke etmesini" engelliyoruz.
        // Bu sayede mouse imlecinin altındaki diğer kartları (drop hedeflerini) algılayabiliriz.
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Sürüklenen nesnenin de bir kart olduğundan emin olalım
        JokerUICard draggedCard = eventData.pointerDrag.GetComponent<JokerUICard>();
        if (draggedCard == null || draggedCard == this)
        {
            // Sürüklenen şey bir kart değilse veya kendimiz üzerine bıraktıysak, işlemi iptal et
            return;
        }

        // Üzerine bırakılan kart biziz (drop target)
        JokerUICard droppedOnCard = this;

        // Kartların mevcut yuvalarını (parent transform'larını) takas işleminden önce saklayalım
        Transform draggedCardOriginalSlot = draggedCard.transform.parent;
        Transform droppedOnCardOriginalSlot = droppedOnCard.transform.parent;

        // Kartların yuvalarını değiştirerek takas işlemini gerçekleştir
        draggedCard.transform.SetParent(droppedOnCardOriginalSlot);
        droppedOnCard.transform.SetParent(draggedCardOriginalSlot);

        // Kartların yeni yuvalarındaki pozisyonlarını sıfırla
        draggedCard.transform.localPosition = Vector3.zero;
        droppedOnCard.transform.localPosition = Vector3.zero;

        // Yönetici (JokerPanelManager) üzerindeki veri yapısını da güncelleyelim
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Sürükleme bittiğinde, kartın tekrar normal şekilde mouse olaylarını alabilmesi için ayarı geri açıyoruz.
        canvasGroup.blocksRaycasts = true;

        // Bu satır, kart başarılı bir yuvaya bırakılsa da bırakılmasa da onu ait olduğu yuvanın merkezine hizalar.
        // OnDrop'ta pozisyon zaten ayarlanıyor ama başarısız drop durumları için bu bir güvencedir.
        transform.localPosition = Vector3.zero;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("JokerUICard clicked");
        GameEvents.JokerCardBought?.Invoke(this);
    }

}
