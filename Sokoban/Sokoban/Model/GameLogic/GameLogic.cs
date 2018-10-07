﻿using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;

namespace Sokoban.Model
{
    public class GameLogic
    {
        private LinkedGameObject _playerObject;
        private char _tempChar = (char)Characters.Tile;
        public bool IsOnSpecialSquare { get; set; }
        public bool GameWon { get; set; }
        
        // set PlayerObject
        public void SetPlayer(LinkedList currentLevel)
        {
            var rows = currentLevel.First;
            var columns = currentLevel.First;

            while (rows != null)
            {
                while (columns != null)
                {
                    if (columns.GameObject.GetChar() == (char)Characters.Player)
                    {
                        _playerObject = columns;
                        return;
                    }
                    columns = columns.ObjectNext;
                }
                rows = rows.ObjectBelow;
                columns = rows;
            }
        }

        // normal move
        private void SwapTwo(LinkedGameObject first, LinkedGameObject second)
        {
            var temp = first.GameObject.GetChar();

            first.GameObject.SetChar(second.GameObject.GetChar());

            second.GameObject.SetChar(temp);

            _playerObject = second;
        }

        // move with chest
        private void SwapTwo(LinkedGameObject first, LinkedGameObject second, bool withChest)
        {
            var temp = first.GameObject.GetChar();

            first.GameObject.SetChar(second.GameObject.GetChar());

            second.GameObject.SetChar(temp);
        }

        private bool NormalMove(LinkedGameObject move)
        {
            // fallback to same level if player goes to empty object
            try
            {
                if (move.GameObject == null) ;
            }
            catch (Exception e) 
            {
                return true;
            }

            if (move.GameObject.GetChar() == (char)Characters.Tile)
            {
                if (IsOnSpecialSquare)
                {
                    move.GameObject.SetChar((char)Characters.Player);
                    _playerObject.GameObject.SetChar(_tempChar);
                    _playerObject = move;
                    IsOnSpecialSquare = false; // reset object
                    return true;
                }
                SwapTwo(_playerObject, move);
                return true;
            }

            return false;
        }

        private bool MoveOntoDestinationOrTrap(LinkedGameObject move)
        {
            if (move.GameObject.GetChar() == (char)Characters.Destination
                || move.GameObject.GetChar() == (char)Characters.Trap
                || move.GameObject.GetChar() == (char)Characters.OpenTrap)
            {
                if (move.GameObject.GetChar() != (char)Characters.Destination)
                {
                    move.GameObject.IsOnTrap();
                }

                if (IsOnSpecialSquare)
                    _playerObject.GameObject.SetChar(_tempChar);
                else
                    _playerObject.GameObject.SetChar((char)Characters.Tile);

                IsOnSpecialSquare = true;
                SetChar(move.GameObject.GetChar());
                move.GameObject.SetChar((char)Characters.Player);
                _playerObject = move;
                return true;
            }
            return false;
        }

        private void SetChar(char c)
        {
            if (c == (char)Characters.Destination)
                _tempChar = (char)Characters.Destination;
            if (c == (char)Characters.Trap)
                _tempChar = (char)Characters.Trap;
            if (c == (char)Characters.OpenTrap)
                _tempChar = (char)Characters.OpenTrap;
        }

        private bool CheckCrateMove(LinkedGameObject move, LinkedGameObject moveAfter)
        {
            if (CheckCrateOnTrap(move, moveAfter))
                return true;

            if (NormalCrateMove(move, moveAfter))
                return true;

            if (MoveCrateToTrap(move, moveAfter))
                return true;

            if (move.GameObject.HasChest)
            {
                if (moveAfter.GameObject.GetChar() == (char)Characters.CrateOnDestination)
                    return false;
                if (moveAfter.GameObject.GetChar() == (char)Characters.Crate)
                    return false;

                if (moveAfter.GameObject.GetChar() == (char)Characters.Destination)
                {
                    SwapTwo(move, moveAfter, true);
                    IsOnSpecialSquare = true;
                    _tempChar = (char)Characters.Destination;
                    move.GameObject.SetChar((char)Characters.Tile);
                    SwapTwo(_playerObject, move);
                    return true;
                }

                if (IsOnSpecialSquare)
                    move.GameObject.SetChar(_tempChar);
                else
                {
                    IsOnSpecialSquare = true;
                    _tempChar = (char)Characters.Destination;
                    move.GameObject.SetChar((char)Characters.Tile);
                }
                _tempChar = (char)Characters.Destination;
                move.GameObject.HasChest = false;
                SwapTwo(_playerObject, move);
                moveAfter.GameObject.SetChar((char)Characters.Crate);
            }

            if (move.GameObject.GetChar() == (char)Characters.Crate &&
                 moveAfter.GameObject.GetChar() == (char)Characters.Destination)
            {
                moveAfter.GameObject.HasChest = true;
                moveAfter.GameObject.SetChar((char)Characters.CrateOnDestination);
                if (IsOnSpecialSquare)
                {
                    move.GameObject.SetChar(_tempChar);
                    IsOnSpecialSquare = false;
                }
                else
                    move.GameObject.SetChar((char)Characters.Tile);
                SwapTwo(_playerObject, move);
                return true;
            }
            return false;
        }

        private bool CheckCrateOnTrap(LinkedGameObject move, LinkedGameObject moveAfter)
        {
            if (move.GameObject.ChestOnTrap)
            {                
                if (moveAfter.GameObject.GetChar() == (char)Characters.Trap)
                {
                    moveAfter.GameObject.IsOnTrap();
                    moveAfter.GameObject.ChestOnTrap = true;
                    moveAfter.GameObject.SetChar((char)Characters.Crate);
                    move.GameObject.SetChar((char)Characters.Player);
                    if (IsOnSpecialSquare)
                        _playerObject.GameObject.SetChar(_tempChar);
                    else
                        _playerObject.GameObject.SetChar((char)Characters.Tile);
                    _playerObject = move;
                    _tempChar = (char)Characters.Trap;
                    IsOnSpecialSquare = true;
                    return true;
                }

                if (moveAfter.GameObject.GetChar() == (char)Characters.Destination)
                {
                    move.GameObject.ChestOnTrap = false;
                    moveAfter.GameObject.SetChar((char)Characters.CrateOnDestination);
                    move.GameObject.SetChar((char)Characters.Player);
                    if (IsOnSpecialSquare)
                        _playerObject.GameObject.SetChar(_tempChar);
                    else
                        _playerObject.GameObject.SetChar((char)Characters.Tile);
                    _playerObject = move;
                    _tempChar = (char)Characters.Trap;
                    IsOnSpecialSquare = true;
                    return true;
                }

                if (moveAfter.GameObject.GetChar() == (char)Characters.Tile)
                {
                    move.GameObject.IsOnTrap();
                    moveAfter.GameObject.SetChar((char)Characters.Crate);
                    move.GameObject.SetChar((char)Characters.Player);
                    if (IsOnSpecialSquare)
                        _playerObject.GameObject.SetChar(_tempChar);
                    else
                        _playerObject.GameObject.SetChar((char)Characters.Tile);
                    _playerObject = move;
                    _tempChar = (char)Characters.Trap;
                    IsOnSpecialSquare = true;
                    move.GameObject.ChestOnTrap = false;
                    return true;
                }
            }
            return false;
        }

        private bool NormalCrateMove(LinkedGameObject move, LinkedGameObject moveAfter)
        {
            if (move.GameObject.GetChar() == (char)Characters.Crate &&
                moveAfter.GameObject.GetChar() == (char)Characters.Tile)
            {
                SwapTwo(move, moveAfter, true);
                if (IsOnSpecialSquare)
                {
                    move.GameObject.SetChar(_tempChar);
                    IsOnSpecialSquare = false;
                }
                if (moveAfter.GameObject.ChestOnTrap)
                {
                    moveAfter.GameObject.ChestOnTrap = false;
                    IsOnSpecialSquare = true;
                    _tempChar = (char)Characters.Trap;
                }
                SwapTwo(_playerObject, move);
                return true;
            }

            if (move.GameObject.GetChar() == (char)Characters.CrateOnDestination &&
                moveAfter.GameObject.GetChar() == (char)Characters.Destination)
            {
                moveAfter.GameObject.SetChar((char)Characters.CrateOnDestination);
                if (IsOnSpecialSquare)
                    move.GameObject.SetChar(_tempChar);
                else
                    move.GameObject.SetChar((char)Characters.Tile);
                SwapTwo(_playerObject, move);
                IsOnSpecialSquare = true;
                _tempChar = (char)Characters.Destination;
                return true;
            }
                return false;
        }

        private bool MoveCrateToTrap(LinkedGameObject move, LinkedGameObject moveAfter)
        {
            if (move.GameObject.GetChar() == (char)Characters.Crate &&
                moveAfter.GameObject.GetChar() == (char)Characters.Trap)
            { 
                moveAfter.GameObject.ChestOnTrap = true;

                moveAfter.GameObject.IsOnTrap();

                moveAfter.GameObject.SetChar(move.GameObject.GetChar()); // ~ -> o
                
                if (IsOnSpecialSquare)
                    move.GameObject.SetChar(_tempChar);
                else
                    move.GameObject.SetChar((char)Characters.Tile);
                SwapTwo(_playerObject, move);

                return true;
            }
            if (move.GameObject.GetChar() == (char)Characters.Crate
                && moveAfter.GameObject.GetChar() == (char)Characters.OpenTrap)
            {
                move.GameObject.ChestOnTrap = false;
                if (IsOnSpecialSquare)
                    move.GameObject.SetChar(_tempChar);
                else
                    move.GameObject.SetChar((char)Characters.Tile);
                SwapTwo(_playerObject, move);
                return true;
            }
            return false;
        }

        public LinkedList MoveUp(LinkedList currentLevel)
        {
            if (NormalMove(_playerObject.ObjectAbove))
                return currentLevel;
            if (MoveOntoDestinationOrTrap(_playerObject.ObjectAbove))
                return currentLevel;
            if (CheckCrateMove(_playerObject.ObjectAbove, _playerObject.ObjectAbove.ObjectAbove))
                return currentLevel;
            return currentLevel;
        }

        public LinkedList MoveLeft(LinkedList currentLevel)
        {
            if (NormalMove(_playerObject.ObjectPrevious))
                return currentLevel;
            if (MoveOntoDestinationOrTrap(_playerObject.ObjectPrevious))
                return currentLevel;
            if (CheckCrateMove(_playerObject.ObjectPrevious, _playerObject.ObjectPrevious.ObjectPrevious))
                return currentLevel;
            return currentLevel;
        }

        public LinkedList MoveDown(LinkedList currentLevel)
        {
            if (NormalMove(_playerObject.ObjectBelow))
                return currentLevel;
            if (MoveOntoDestinationOrTrap(_playerObject.ObjectBelow))
                return currentLevel;
            if (CheckCrateMove(_playerObject.ObjectBelow, _playerObject.ObjectBelow.ObjectBelow))
                return currentLevel;
            return currentLevel;
        }

        public LinkedList MoveRight(LinkedList currentLevel)
        {
            if (NormalMove(_playerObject.ObjectNext))
                return currentLevel;
            if (MoveOntoDestinationOrTrap(_playerObject.ObjectNext))
                return currentLevel;
            if (CheckCrateMove(_playerObject.ObjectNext, _playerObject.ObjectNext.ObjectNext))
                return currentLevel;
            return currentLevel;
        }

        public bool GameFinished(LinkedList level)
        {
            var rows = level.First;
            var columns = level.First;

            while (rows != null)
            {
                while (columns != null)
                {
                    if (columns.GameObject.GetChar() == (char)Characters.Destination ||
                        columns.GameObject.GetChar() == (char)Characters.Crate)
                    {
                        GameWon = false;
                        return false;
                    }
                    columns = columns.ObjectNext;
                }
                rows = rows.ObjectBelow;
                columns = rows;
            }
            GameWon = true;
            return true;
        }
    }

}