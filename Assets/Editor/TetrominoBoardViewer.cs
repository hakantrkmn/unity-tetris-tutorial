using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;

public class TetrominoBoardViewer : OdinEditorWindow
{
    // Pencereyi açmak için menü öğesi
    [MenuItem("Window/Tetromino Board Viewer")]
    private static void OpenWindow()
    {
        GetWindow<TetrominoBoardViewer>().Show();
    }

    [ShowInInspector, ReadOnly, Title("Tetromino Board State")]
    [TableMatrix(DrawElementMethod = "DrawCell", SquareCells = true, IsReadOnly = true)]
    private TetrominoData[,] _boardMatrix;

    private TetrominoBoardController _targetController;

    protected override void OnEnable()
    {
        base.OnEnable();
        this.titleContent = new GUIContent("Board Viewer");
        // Pencere ilk açıldığında seçili nesneyi kontrol et
        OnSelectionChange();
    }

    // Unity'de seçili nesne değiştiğinde otomatik olarak çağrılır
    private void OnSelectionChange()
    {
        var activeGo = Selection.activeGameObject;
        if (activeGo != null)
        {
            _targetController = activeGo.GetComponent<TetrominoBoardController>();
        }
        else
        {
            _targetController = null;
        }
        Repaint(); // Pencereyi yeniden çiz
    }

    // Odin'in bu penceredeki nesneleri çizmesini sağlar
    protected override void OnGUI()
    {
        if (_targetController != null)
        {
            // Matris verilerini hedef controller'dan al
            _boardMatrix = _targetController.tetronimoBoard;
        }
        else
        {
            _boardMatrix = null;
        }

        if (_boardMatrix == null)
        {
            EditorGUILayout.HelpBox("Lütfen TetrominoBoardController bileşenine sahip bir GameObject seçin.", MessageType.Info);
        }

        // base.OnGUI() çağrısı, Odin'in _boardMatrix'i çizmesini sağlar
        base.OnGUI();
    }

    // TableMatrix tarafından her bir hücreyi çizmek için çağrılır
    private static TetrominoData DrawCell(Rect rect, TetrominoData value)
    {
        if (value != null)
        {
            string label = value.tetromino.ToString();

            GUIStyle style = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(rect, label, style);
        }
        else
        {
            // Boş hücreler için bir kutu çiz
            GUI.Box(rect.Padding(1), GUIContent.none);
        }

        // Bu sadece bir görüntüleyici olduğu için değeri değiştirmeden geri döndür
        return value;
    }

    // Oyun çalışırken pencerenin güncel kalmasını sağlar
    private void OnInspectorUpdate()
    {
        Repaint();
    }
}
