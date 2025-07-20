using UnityEngine.UI;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections; // Coroutine için gerekli

public class GridLayoutSetter : MonoBehaviour
{
    public GridLayoutGroup gridLayout;
    public int maxRowCount;
    public int maxColumnCount;

    public float spacing;
    public int padding;

    [Button]
    public void SetGridLayoutSize()
    {
        var rectTransform = transform.GetComponent<RectTransform>();
        var width = rectTransform.rect.width;
        var height = rectTransform.rect.height;

        if (width == 0 || height == 0)
        {
            Debug.LogWarning("GridLayoutSetter: RectTransform boyutu sıfır. Hücre boyutu hesaplanamıyor. Layout'un hesaplanmasını bekleyin.");
            return;
        }

        gridLayout.padding = new RectOffset(padding, padding, padding, padding);
        gridLayout.spacing = new Vector2(spacing, spacing);

        // Toplam yatay ve dikey boşlukları hesapla
        float totalHorizontalSpace = (padding * 2) + (spacing * (maxColumnCount - 1));
        float totalVerticalSpace = (padding * 2) + (spacing * (maxRowCount - 1));

        // Boşlukları çıkardıktan sonra kalan alana göre hücre boyutunu hesapla
        float cellWidth = (width - totalHorizontalSpace) / maxColumnCount;
        float cellHeight = (height - totalVerticalSpace) / maxRowCount;

        gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
    }

    private void Start()
    {
        // Metodu doğrudan çağırmak yerine, bir Coroutine başlatarak karenin sonuna kadar bekliyoruz.
        // Bu, UI layout sistemine hesaplama yapması için zaman tanır.
        StartCoroutine(SetLayoutAfterFrame());
    }

    private IEnumerator SetLayoutAfterFrame()
    {
        // Bu karenin sonuna kadar bekle.
        yield return new WaitForEndOfFrame();

        // Artık RectTransform boyutları doğru olmalı.
        SetGridLayoutSize();
    }
}
