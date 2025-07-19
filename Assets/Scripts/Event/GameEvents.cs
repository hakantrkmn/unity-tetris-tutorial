using System;

public static class GameEvents
{
    // Bir sıra temizlendiğinde tetiklenir. Parametre: temizlenen sıra sayısı.
    public static event Action<int> OnLineCleared;

    public static void TriggerLineCleared(int lines)
    {
        OnLineCleared?.Invoke(lines);
    }
    
    // Oyuncunun kaynaklarını (altın vb.) tutacak bir sisteme de ihtiyacın olacak.
    // public static event Action<int> OnGoldGained;
}