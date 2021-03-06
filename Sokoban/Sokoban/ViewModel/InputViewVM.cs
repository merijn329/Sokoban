﻿using Sokoban.Model;
using Sokoban.View;
using System;

namespace Sokoban.ViewModel
{
    public class InputViewVM
    {
        private OutputViewVM _output;
        private MainView _view;
        private GameLogic logic;

        public InputViewVM(OutputViewVM output, MainView view)
        {
            _view = view;
            _output = output;
        }

        public void LoadLevel(ConsoleKey input)
        {
            if (input == ConsoleKey.S)
            {
                Environment.Exit(0);
            }
            try
            {
                string x = input.ToString().Substring(1, 1);
                int i = int.Parse(x);
                logic = new GameLogic();
                _output.LoadLevel(i - 1);
                logic.SetPlayer(_output._currentLevel);
                _output.ShowLevel();
            }
            catch (Exception e)
            {
                _view.StartListening();
            }
                

        }



        public void PlayLevel(ConsoleKey key)
        {
            if (key == ConsoleKey.S)
            {
                Environment.Exit(0);
            }
            else if (key == ConsoleKey.R)
            {
                _output.ReloadLevel(_output._currentLevelNumber);
                logic.GameWon = false;
                logic._isOnSpecialSquare = false;
                logic.SetPlayer(_output._currentLevel);
                _output.ShowLevel();
            }
            else if (key == ConsoleKey.M)
            {
                _output.ReshowMenu();
            }
            else if (key == ConsoleKey.L && logic.GameWon)
            {
                if (_output.GetAmountOfLevels() <= _output._currentLevelNumber + 1)
                    _output.ReshowMenu();
                _output.LoadLevel(_output._currentLevelNumber + 1);
                logic.GameWon = false;
                logic._isOnSpecialSquare = false;
                logic.SetPlayer(_output._currentLevel);
                _output.ShowLevel();
            }
            else if (key == ConsoleKey.UpArrow && !logic.GameWon || key == ConsoleKey.W && !logic.GameWon)
            {
                _output._currentLevel = logic.MoveUp(_output._currentLevel);
                CheckGameOver();
                _output.ShowLevel();
            }
            else if (key == ConsoleKey.LeftArrow && !logic.GameWon || key == ConsoleKey.A && !logic.GameWon)
            {
                _output._currentLevel = logic.MoveLeft(_output._currentLevel);
                CheckGameOver();
                _output.ShowLevel();
            }
            else if (key == ConsoleKey.DownArrow && !logic.GameWon || key == ConsoleKey.S && !logic.GameWon)
            {
                _output._currentLevel = logic.MoveDown(_output._currentLevel);
                CheckGameOver();
                _output.ShowLevel();
            }
            else if (key == ConsoleKey.RightArrow && !logic.GameWon || key == ConsoleKey.D && !logic.GameWon)
            {
                _output._currentLevel = logic.MoveRight(_output._currentLevel);
                CheckGameOver();
                _output.ShowLevel();
            }
            else
            {
                _view.StartPlaying();
            }

        }

        private void CheckGameOver()
        {
            if (logic.GameFinished(_output._currentLevel))
            {
                _output.ShowLevel(true);
                _view.WriteLine("Congratulations, you have completed the level");
                _view.WriteLine("Press M to go to Menu");
                _view.WriteLine("Press R to play again");
                _view.WriteLine("Press L to play next level");
                _view.StartPlaying();
            }
        }
    }
}