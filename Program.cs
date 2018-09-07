﻿using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Player
{
    static void Main(string[] args)
    {
        var GameStage = new Stage();
        

        // game loop
        while (true)
        {
            var inputs = Console.ReadLine().Split(' ');
            int cloneFloor = int.Parse(inputs[0]); // floor of the leading clone
            int clonePos = int.Parse(inputs[1]); // position of the leading clone on its floor
            string direction = inputs[2]; // direction of the leading clone: LEFT or RIGHT

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            Console.WriteLine("WAIT"); // action: WAIT or BLOCK
        }
    }
}

internal class Stage
{
    public int Floors { get; private set; }
    public int Width { get; private set; }
    public int NbRounds { get; private set; }
    public int ExitFloor { get; private set; }
    public int ExitPos { get; private set; }
    public int NbTotalClones { get; private set; }
    public int NbExtraElevators { get; private set; }
    public Dictionary<int, List<int>> Elevators { get; set; }

    public Stage()
    {
        string[] inputs = Console.ReadLine().Split(' ');
        Floors = int.Parse(inputs[0]); // number of floors
        Width = int.Parse(inputs[1]); // width of the area
        NbRounds = int.Parse(inputs[2]); // maximum number of rounds
        ExitFloor = int.Parse(inputs[3]); // floor on which the exit is found
        ExitPos = int.Parse(inputs[4]); // position of the exit on its floor
        NbTotalClones = int.Parse(inputs[5]); // number of generated clones
        NbExtraElevators = int.Parse(inputs[6]); // number of additional elevators that you can build
        Elevators = new Dictionary<int, List<int>>();
        Elevators.Add(ExitFloor, new List<int>{ExitPos}); //exit as an elevator
        int nbElevators = int.Parse(inputs[7]); // number of elevators
        for (int i = 0; i < nbElevators; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int elevatorFloor = int.Parse(inputs[0]); // floor on which this elevator is found
            int elevatorPos = int.Parse(inputs[1]); // position of the elevator on its floor
            if (Elevators.ContainsKey(elevatorFloor))
                Elevators[elevatorFloor].Add(elevatorPos);
            else
                Elevators.Add(elevatorFloor, new List<int>{elevatorPos});
        }
    }
}