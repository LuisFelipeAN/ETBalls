using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DadosJogo : Subject {
    private int vida;
    private int bolas;
    private int pontos;
    private Game game;

    private float dv;

    private static DadosJogo instance;

    
    public static DadosJogo getInstance()
    {
        if (instance == null)
        {
            instance = new DadosJogo();
        }
        return instance;
    }

    private DadosJogo() : base()
    {
        game = new Game();
        bolas = 0;
        pontos = 0;
    }


    public int Vida
    {
        get { return vida; }

        set
        {
            vida = value;
            if (vida <= 0)
            {
                game.GameOver = true;
            }
            notifyObservers();
        }
    }

    public int Pontos
    {
        get { return pontos; }
        set
        {
            pontos = value;
            notifyObservers();
        }
    }

    public int Bolas
    {
        get { return bolas; }
        set
        {
            bolas = value;
            notifyObservers();
        }
    }

    public bool GameOver
    {
        get { return game.GameOver; }
        set
        {
            game.GameOver = value;
            notifyObservers();
        }
    }

    public Game getGame()
    {
        return game;
    }
    
}
