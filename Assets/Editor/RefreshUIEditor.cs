using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.SceneManagement; // Sahneyi "değiştirildi" olarak işaretlemek için gerekli

public class RefreshUIEditor
{
    // Unity'nin üst menüsüne "Tools/Refresh UI" adında bir buton ekler.
    [MenuItem("Tools/Refresh UI")]
    private static void ForceRefreshUI()
    {
        Debug.Log("UI yenileme işlemi başlatıldı...");

        // 1. Editor Pencerelerini Yeniden Çiz (Her zaman faydalı)
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        Debug.Log("Tüm editor pencereleri yeniden çizildi.");

        // 2. Sahnedeki tüm Canvas'ları bul ve layout'larını yeniden oluşturmaya zorla.
        // Bu işlem artık hem Edit Mode'da hem de Play Mode'da çalışır.
        Canvas[] allCanvases = Object.FindObjectsOfType<Canvas>();

        if (allCanvases.Length == 0) {
            Debug.Log("Sahnede güncellenecek Canvas bulunamadı.");
            return;
        }

        int refreshedCanvasCount = 0;
        foreach (Canvas canvas in allCanvases)
        {
            // Canvas'a bağlı tüm UI elemanlarının layout'unu yeniden hesaplamaya zorlar.
            if (canvas.gameObject.activeInHierarchy)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(canvas.GetComponent<RectTransform>());
                refreshedCanvasCount++;
            }
        }

        if (refreshedCanvasCount > 0)
        {
            Debug.Log($"{refreshedCanvasCount} adet aktif Canvas'ın layout'u yeniden oluşturuldu.");

            // Edit Mode'da bu işlem sahneyi "değiştirilmiş" (dirty) olarak işaretler.
            // Bu, kullanıcının değişiklikleri kaydetmesini hatırlatır.
            if (!Application.isPlaying)
            {
                Debug.Log("Edit Mode'da UI güncellendi. Sahnenin kaydedilmesi gerekebilir.");
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
    }
}
