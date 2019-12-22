using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace MarsRover
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Rover> roverList = new List<Rover>();
            List<int> terrainMaxPoints = new List<int>();

            //Gets the instructions from  .txt file located under application base directory.
            var commandlines = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\\roverCommands.txt").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

            if (commandlines.Count < 3)
            {
                throw new Exception(
                    $"Insufficient information provided. Information must include terrain max cooridnates. At least one rover's initial positions and movement information");
            }

            var terrainPointsInput = commandlines[0].Trim().Split(' ').ToList();

            //Parses and validates the gathered information for terrain data.
            ParseAndValidateTerrainData(terrainPointsInput, terrainMaxPoints);

            for (int i = 1; i < commandlines.Count; i += 2)
            {
                Rover rover = new Rover();
                var initialRoverPosition = commandlines[i].Trim().Split(' ').ToList();

                //Parses and validates the gathered information for mars rover.
                ParseAndValidateInitialRoverData(initialRoverPosition, rover);

                //Checks if initial rover placement location is occupied by another rover.
                CheckLocationAvailability(roverList, rover, terrainMaxPoints);

                //Checks if any move information exists.
                if (commandlines.Count == i + 1)
                    throw new Exception($"Rover#{roverList.Count + 1} movement terminated. Missing move information");

                //Gets the rover's move command(s).
                var moves = commandlines[i + 1].ToUpper().Trim().Replace(" ", "").ToCharArray();

                try
                {
                    //Starts rover's movement.
                    rover.StartMoving(terrainMaxPoints, moves, roverList);
                    roverList.Add(rover);
                    Console.WriteLine($"Rover#{roverList.Count} moved successfully to target location ({rover.X},{rover.Y} {rover.Direction.ToString()})");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Rover#{roverList.Count + 1} movement terminated.");
                    throw e;
                }
            }

            //Successful Output.
            foreach (var rover in roverList)
            {
                Console.WriteLine($"{rover.X} {rover.Y} {rover.Direction.ToString()}");
            }

            Console.ReadLine();
        }

        private static void CheckLocationAvailability(List<Rover> roverList, Rover rover, List<int> terrainPoints)
        {
            //Checks if initial rover position is occupied by another rover.
            if (roverList.Any(r => r.X == rover.X && r.Y == rover.Y))
            {
                throw new Exception($"Initial rover position is not available. Target location is occupied by another rover.");
            }

            //Checks if initial rover position is inside the boundaries.
            if (rover.X > terrainPoints[0] || rover.Y > terrainPoints[1])
            {
                throw new Exception($"Initial rover position({rover.X},{rover.Y}) is not available. Target location is out of boundaries.");
            }

        }

        private static void ParseAndValidateInitialRoverData(List<string> initialRoverPosition, Rover rover)
        {
            if (initialRoverPosition.Count == 3)
            {
                //Parses and validates initial rover's X coordinate input
                if (!int.TryParse(initialRoverPosition[0], out int xPosition))
                    throw new Exception(
                        $"Invalid initial X position for rover (Invalid data: {initialRoverPosition[0]}). Position value must be an integer.");

                //Parses and validates initial rover's Y coordinate input
                if (!int.TryParse(initialRoverPosition[1], out int yPosition))
                    throw new Exception(
                        $"Invalid initial Y position for rover (Invalid data: {initialRoverPosition[1]}). Position value must be an integer.");

                //Parses and validates initial rover's direction input.
                Directions direction;
                if (!Regex.IsMatch(initialRoverPosition[2], @"^[a-zA-Z]+$"))
                    throw new Exception(
                        $"Invalid initial direction for rover (Invalid data: {initialRoverPosition[2]}). Valid direction values are (N,E,S,W)");

                //Parses and validates initial rover's direction input is valid direction type.
                if (!Enum.TryParse(initialRoverPosition[2], true, out direction))
                    throw new Exception(
                        $"Invalid initial direction for rover (Invalid data: {initialRoverPosition[2]}). Valid direction values are (N,E,S,W)");

                rover.X = xPosition;
                rover.Y = yPosition;
                rover.Direction = direction;
            }
            else
            {
                throw new Exception(
                    $"Invalid initial directions for rover. Valid direction values must include X,Y coordinates and initial direction(N,E,S,W). Eg.(1 2 N)");
            }
        }

        private static void ParseAndValidateTerrainData(List<string> terrainPointsInput, List<int> terrainMaxPoints)
        {
            foreach (var input in terrainPointsInput)
            {
                if (!int.TryParse(input, out int parsedResult))
                {
                    throw new Exception($"Invalid terrain position data (Invalid data: {input}). Position value must be an integer.");
                }
                else
                {
                    terrainMaxPoints.Add(parsedResult);
                }
            }

            if (terrainMaxPoints.Count < 2)
                throw new Exception(
                    $"Terrain size data is invalid. Terrain size data must include maximum X and maximum Y positions. Eg.(6 6)");
            else if (terrainMaxPoints.Count > 2)
                throw new Exception(
                    $"Terrain size data is invalid. Terrain size data must include maximum X and maximum Y positions. Eg.(6 6)");
        }
    }
}
