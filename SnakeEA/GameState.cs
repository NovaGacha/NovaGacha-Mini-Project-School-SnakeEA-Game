using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeEA
{
    public class GameState
    { 
        public int Rows { get; }
        public int Cols { get; }
        public GridValue[,] Grid { get; }
        public Derection Dir { get; private set; }
        public int Score { get; private set; }
        public bool GameOver { get; private set; }

        private readonly LinkedList<Derection> dirChanges = new LinkedList<Derection>();
        private readonly LinkedList<Positions> snakePosition = new LinkedList<Positions>();
        private readonly Random random = new Random();

        public GameState(int rows, int cols) 
        {
            Rows = rows; 
            Cols = cols;
            Grid = new GridValue[rows, cols];
            Dir = Derection.right;

            AddSnake();
            AddFood();
        }

        private void AddSnake()
        {
            int r = Rows / 2;
            for (int c = 1; c <= 3; c++)
            {
                Grid[r, c] = GridValue.Snake;
                snakePosition.AddFirst(new Positions(r, c));

            }
        }

        private IEnumerable<Positions> EmptyPositions()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Grid[r, c] == GridValue.Empty)
                    {
                        yield return new Positions(r, c);  
                    }
                }
            }
        }

        private void AddFood() 
        { 
            List<Positions> empty = new List<Positions>(EmptyPositions());

            if (empty.Count == 0)
            {
                return;
            }

            Positions pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Col] = GridValue.Food;
        }

        public Positions HeadPositions()
        {
            return snakePosition.First.Value;
        }

        public Positions TailPositions()
        {
            return snakePosition.Last.Value;
        }

        public IEnumerable<Positions> SnakePositions()
        {
            return snakePosition;
        }

        private void AddHead(Positions pos)
        { 
            snakePosition.AddFirst(pos);
            Grid[pos.Row, pos.Col] = GridValue.Snake;
        }

        public void RemoveTail()
        { 
            Positions tail = snakePosition.Last.Value;
            Grid[tail.Row, tail.Col] = GridValue.Empty;
            snakePosition.RemoveLast();
        }

        private Derection GetLastDerection()
        { 
            if (dirChanges.Count == 0)
            {
                return Dir;
            }
            return dirChanges.Last.Value;
        }

        private bool CanChangesDerection(Derection newDir)
        {
            if (dirChanges.Count == 2)
            { 
                return false;
            }
            Derection lastDir = GetLastDerection();
            return newDir != lastDir && newDir != lastDir.Opposite();
        }

        public void ChangeDerection(Derection dir)
        {
            if (CanChangesDerection(dir))
            {
                dirChanges.AddLast(dir);
            }
        }

        private bool OutsideGrid(Positions pos)
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Col < 0 || pos.Col >= Cols;
        }

        private GridValue WillHit(Positions newHeadPos)
        {
            if (OutsideGrid(newHeadPos))
            {
                return GridValue.Outside;
            }
            if (newHeadPos == TailPositions())
            {
                return GridValue.Empty;
            }
            return Grid[newHeadPos.Row, newHeadPos.Col];
        }

        public void Move()
        {
            if (dirChanges.Count > 0)
            {
                Dir = dirChanges.First.Value;
                dirChanges.RemoveFirst();
            }

            Positions newHeadPos = HeadPositions().Translate(Dir);
            GridValue hit = WillHit(newHeadPos);
            if (hit == GridValue.Outside || hit == GridValue.Snake)
            {
                GameOver = true;
            }
            else if (hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }
            else if (hit == GridValue.Food)
            { 
                AddHead(newHeadPos);
                Score++;
                AddFood();
            }

        }
    }

    
}
