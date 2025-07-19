using UnityEngine;

// CreateAssetMenu, Unity editöründen yeni güçler oluşturmanı sağlar.
[CreateAssetMenu(fileName = "New Special Power", menuName = "Tetris/Special Power")]
public abstract class SpecialPower : ScriptableObject
{
    [Tooltip("Gücün adı")]
    public string powerName;

    [Tooltip("Gücün açıklaması")]
    [TextArea]
    public string description;

    public Sprite artwork;

    // Her güç, bu metodu kendine göre dolduracak.
    // piece: gücü tetikleyen parça
    // board: oyun tahtası
    public abstract void Activate(Piece piece, Board board);
}