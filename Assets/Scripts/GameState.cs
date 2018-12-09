using UnityEngine;
using System.Collections;

public class GameState: Subject// Classe que implementa o padrão observer
{
    private bool gameOver;
    private bool win;
    public GameState()
    {
        gameOver = false;
        win = false;
    }
    public bool GameOver
    {
        get { return gameOver; }
        set
        {
            this.gameOver = value;
            notifyObservers();
        }
    }
    public bool Win
    {
        get { return win; }
        set
        {
            this.win = value;
            notifyObservers();
        }
    }
}
