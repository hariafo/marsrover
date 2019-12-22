using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarsRover
{
    public class Rover
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Directions Direction { get; set; }

        public Rover()
        {
            X = 0;
            Y = 0;
            Direction = Directions.N;
        }

        private void TurnLeft()
        {
            var newDirection = ((int)this.Direction - 1) % 4;

            if (newDirection == 0)
                newDirection = 4;


            this.Direction = (Directions)newDirection;
        }

        private void TurnRight()
        {
            var newDirection = ((int)this.Direction + 1) % 4;

            if (newDirection == 0)
                newDirection = 4;


            this.Direction = (Directions)newDirection;
        }

        private void MoveForward()
        {
            switch (this.Direction)
            {
                case Directions.N:
                    this.Y += 1;
                    break;
                case Directions.S:
                    this.Y -= 1;
                    break;
                case Directions.E:
                    this.X += 1;
                    break;
                case Directions.W:
                    this.X -= 1;
                    break;
                default:
                    break;
            }
        }

        public void StartMoving(List<int> terrainMaxPoints, char[] moves, List<Rover> roverList)
        {
            foreach (var move in moves)
            {
                switch (move)
                {
                    case 'M':
                        this.MoveForward();
                        break;
                    case 'L':
                        this.TurnLeft();
                        break;
                    case 'R':
                        this.TurnRight();
                        break;
                    default:
                        throw new Exception($"Invalid Move Command. (Invalid Data: {move}) Valid move values are (L,R,M)");
                }

                //Checks if final location is inside the boundaries.
                if (this.X < 0 || this.X > terrainMaxPoints[0] || this.Y < 0 || this.Y > terrainMaxPoints[1])
                {
                    throw new Exception(
                        $"Attempted move ({new string(moves)}) is out of boundaries. Valid boundaries are between (0,0) and ({terrainMaxPoints[0]},{terrainMaxPoints[1]})");
                }

                //Checks if final location is occupied.
                if (roverList.FirstOrDefault(r => r.X == this.X && r.Y == this.Y) != null)
                {
                    throw new Exception($"Rover move ({new string(moves)}) terminatad. Final location ({this.X},{this.Y}) is occupied");
                }
            }
        }
    }
}
