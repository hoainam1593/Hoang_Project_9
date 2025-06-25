using UnityEngine;

interface IGame
{
    public void StartGame();
    public void PauseGame();
    public void ResumeGame();
    public void ExitGame();
}

public class Game : IGame
{
    #region IGameManager
    private bool isStarted = false;
    private bool isPaused = false;
    
    public void StartGame()
    {
        isStarted = true;
        isPaused = false;
    }

    public void PauseGame()
    {
        isPaused = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
    }

    public void ExitGame()
    {
        isStarted = false;
        isPaused = false;
    }
    #endregion IGameManager
}
