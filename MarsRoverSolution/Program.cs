using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// This is the grid or terrain. To keep things clean and easy to read
// We could extract this into a separate file.
namespace MarsRoverSolution
{
    class Grid
    {
        /// <summary>
        /// Grid props. Only the grid can set these properties 
        /// </summary>
        public int maxX { get; private set; }
        public int minX { get; private set; }
        public int maxY { get; private set; }
        public int minY { get; private set; }
        
        //This is not a requirement but it makes things a bit exciting :)
        public HashSet<Point> obstacles { get; private set; }

        /// <summary>
        /// Build grid with provided parameters of default to ones below
        /// </summary>
        /// <param name="maxX"></param>
        /// <param name="minX"></param>
        /// <param name="maxY"></param>
        /// <param name="minY"></param>
        public Grid(int maxX = 10, int minX = 0, int maxY = 10, int minY = 0)
        {
            this.maxX = maxX;
            this.minX = minX;
            this.maxY = maxY;
            this.minY = minY;

            //generate obstacles for the quater of the map
            int numObstacles = ((maxX - minX) * (maxY - minY)) / 4;
            obstacles = new HashSet<Point>();

            Random rand = new Random();
            int index = 1;
            while (index <= numObstacles)
            {
                //generate and keep random point if it is not a duplicate
                Point p = new Point(rand.Next(minX, maxX - 1), rand.Next(minY, maxY + 1));
                if (!obstacles.Contains(p))
                {
                    obstacles.Add(p);
                    index++;
                }
            }
        }

        /// <summary>
        /// Gives details regarding obstacles' position
        /// </summary>
        public void showObstacles()
        {
            foreach (Point p in obstacles)
                Console.WriteLine("(" + p.X + ", " + p.Y + ")");
        }
    }
}


namespace MarsRoverSolution
{
    /// <summary>
    /// Enum presenting the four possible directions
    /// </summary>
    enum Direction { N, S, E, W };

    /// <summary>
    /// The Rover
    /// </summary>
    class Rover
    {
        /// <summary>
        /// Properties of the Rover. These should be set within this class.
        /// </summary>
        public Point currentPos { get; private set; }
        public Direction currentDir { get; private set; }

        private Grid grid;     //current grid rover is in

        /// <summary>
        /// Build rover with provided parameters of default to ones below
        /// </summary>
        /// <param name="startingX"></param>
        /// <param name="startingY"></param>
        /// <param name="startingDir"></param>
        public Rover(int startingX = 0, int startingY = 0, Direction startingDir = Direction.N)
        {
            currentPos = new Point(startingX, startingY);
            currentDir = startingDir;
        }

        //method to assign grid to rover
        /// <summary>
        /// Instantiates the rover in the terrain
        /// </summary>
        /// <param name="grid"></param>
        public void assignGrid(Grid grid)
        {
            this.grid = grid;

            if (grid.obstacles.Contains(currentPos))
            {
                Console.Write("Grid has obstacle at rover starting position. Landed rover at ");
                //verify that current position of rover is not an obstacle
                while (grid.obstacles.Contains(currentPos))
                    moveForward();

                Console.Write("(" + currentPos.X + ", " + currentPos.Y + ") instead\n");
            }
        }

        /// <summary>
        /// Moves the rover forward or errors if obstacle encountered
        /// </summary>
        /// <returns></returns>
        public bool moveForward()
        {
            switch (currentDir)
            {
                case Direction.N: incrementY();
                    break;

                case Direction.S: decrementY();
                    break;

                case Direction.E: incrementX();
                    break;

                case Direction.W: decrementX();
                    break;
            }

            //if new current position has obstacle, move back and report it
            if (grid.obstacles.Contains(currentPos))
            {
                Console.WriteLine("Cannot move forward. Obstacle present ahead at (" + currentPos.X + ", " + currentPos.Y + ")");
                moveBackward();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Moves the rover backwards or errors if obstacle encountered
        /// </summary>
        /// <returns></returns>
        public bool moveBackward()
        {
            switch (currentDir)
            {
                case Direction.N: decrementY();
                    break;

                case Direction.E: decrementX();
                    break;
                
                case Direction.S: incrementY();
                    break;

                case Direction.W: incrementX();
                    break;
            }

            //if new current position has obstacle, move back and report it
            if (grid.obstacles.Contains(currentPos))
            {
                Console.WriteLine("Cannot move backward. Obstacle present behind at (" + currentPos.X + ", " + currentPos.Y + ")");
                moveForward();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Switches the direction the rover is facing
        /// </summary>
        public void turnLeft()
        {
            switch (currentDir)
            {
                case Direction.N: currentDir = Direction.W;
                    break;

                case Direction.S: currentDir = Direction.E;
                    break;

                case Direction.E: currentDir = Direction.N;
                    break;

                case Direction.W: currentDir = Direction.S;
                    break;
            }
        }

        /// <summary>
        /// Switches the direction the rover is facing
        /// </summary>
        public void turnRight()
        {
            switch (currentDir)
            {
                case Direction.N: currentDir = Direction.E;
                    break;

                case Direction.S: currentDir = Direction.W;
                    break;

                case Direction.E: currentDir = Direction.S;
                    break;

                case Direction.W: currentDir = Direction.N;
                    break;
            }
        }

        /// <summary>
        /// Method to increment rover's x position
        /// </summary>
        private void incrementX()
        {
            int newX = currentPos.X + 1;

            if (newX > grid.maxX)
                newX = grid.minX;

            currentPos = new Point(newX, currentPos.Y);
        }

        /// <summary>
        /// Method to decrement rover's x position
        /// </summary>
        private void decrementX()
        {
            int newX = currentPos.X - 1;

            if (newX < grid.minX)
                newX = grid.maxX;

            currentPos = new Point(newX, currentPos.Y);
        }

        /// <summary>
        /// Method to decrement rover's y position
        /// </summary>
        private void incrementY()
        {
            int newY = currentPos.Y + 1;

            if (newY > grid.maxY)
                newY = grid.minY;

            currentPos = new Point(currentPos.X, newY);
        }

        /// <summary>
        /// Method to decrement rover's y position
        /// </summary>
        private void decrementY()
        {
            int newY = currentPos.Y - 1;

            if (newY < grid.minY)
                newY = grid.maxY;

            currentPos = new Point(currentPos.X, newY);
        }

        /// <summary>
        /// Displays the rover's new position and facing
        /// </summary>
        public void displayNewPosition()
        {
            Console.Write("New rover position is (" + currentPos.X + ", " + currentPos.Y + ") facing ");

            switch (currentDir)
            {
                case Direction.N: Console.Write("North.\n");
                    break;

                case Direction.S: Console.Write("South.\n");
                    break;

                case Direction.E: Console.Write("East.\n");
                    break;

                case Direction.W: Console.Write("West.\n");
                    break;
            }
        }
    }
}

namespace MarsRoverSolution
{
    class Program
    {
        static void Main(string[] args)
        {
            Grid grid = new Grid();
            Rover rover = new Rover();

            Console.WriteLine(string.Format("Landing rover at ({0}, {1})", rover.currentPos.X, rover.currentPos.Y));
            rover.assignGrid(grid);

            string input;
            Console.WriteLine("To move in the facing direction, input M.\nTo move forward, input F\nTo move backward, input B\nTo turn left, input L.\nTo turn right, input R OR exit:");
            input = Console.ReadLine().ToLower();

            while (!input.Equals("exit"))
            {
                bool lastMoveSuccess = true;

                foreach (char ch in input)
                {
                    //break out of loop if encountered obstacle
                    if (lastMoveSuccess == false)
                        break;

                    //switch ignore other non valid commands
                    switch (ch)
                    {
                        case 'm': lastMoveSuccess = rover.moveForward();
                            break;

                        // In case one wants to be specific about the movement
                        case 'f': lastMoveSuccess = rover.moveBackward();
                            break;

                        // In case one wants to be specific about the movement
                        case 'b': lastMoveSuccess = rover.moveBackward();
                            break;

                        case 'l': rover.turnLeft();
                            break;

                        case 'r': rover.turnRight();
                            break;
                    }
                }

                rover.displayNewPosition();

                Console.WriteLine("To move in the facing direction, input M.\nTo move forward, input F\nTo move backward, input B\nTo turn left, input L.\nTo turn right, input R OR exit:");
                input = Console.ReadLine().ToLower();
            }
        }
    }
}
