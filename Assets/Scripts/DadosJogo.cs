using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//classe para armazenar os dados do jogo
//Padrões utilizados nessa classe: Singleton e Observer


public class DadosJogo : Subject {
    private int vida;//vida atual do jogador
    private int bolas;//numero de bolas disparadas
    private int pontos;//pontos do jogador

    private GameState gameState;//instancia da classe GameState que implementa o padrão observer
                                //optou-se por coloca-la pois alguns elementos da interface observam apenas se é ou não game over

    private static DadosJogo instance;//instancia da classe dados jogo necessaria para implementar o padrao singleton

    public static DadosJogo getInstance()//metodo statico getInstance() necessario para implementar o padrao singleton
    {
        if (instance == null)
        {
            instance = new DadosJogo();
        }
        return instance;
    }

    private DadosJogo() : base()//construtor privado necessario para implementar o padrao singleton
    {
        gameState = new GameState();
        bolas = 0;
        pontos = 0;
    }

    //todos os metodos a segir possuem o estilo do padrao observer ou seja quando o mesmo for alterado notificar os observadores
    public int Vida
    {
        get { return vida; }

        set
        {
            vida = value;
            if (vida <= 0)
            {
                gameState.GameOver = true;
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
        get { return gameState.GameOver; }
        set
        {
            gameState.GameOver = value;
            notifyObservers();
        }
    }

    public GameState getGame()
    {
        return gameState;
    }
    
    public void clearObservers()
    {
        removeAllObservers();
        gameState.removeAllObservers();
    }
}
