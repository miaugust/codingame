using System;
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
        GameStage.AddOptionalElevators();
        GameStage.PrintMap();
        Node EstimatedStartingPoint = new Node(GameStage);
        var PathSearcher = new BestPathSearcher(GameStage, EstimatedStartingPoint); 
        PathSearcher.CalculateRoute(); 

        // game loop
        while (true)
        {
            var inputs = Console.ReadLine().Split(' ');
            int cloneFloor = int.Parse(inputs[0]); // floor of the leading clone
            int clonePos = int.Parse(inputs[1]); // position of the leading clone on its floor
            string direction = inputs[2]; // direction of the leading clone: LEFT or RIGHT

            Console.WriteLine("WAIT"); // action: WAIT or BLOCK
        }
    }
}
internal class BestPathSearcher
{
    private Stage gameStage;
    private Node estimatedStartingPoint;
    public List<Node> SearchList { get; set; }

    public BestPathSearcher(Stage gameStage, Node estimatedStartingPoint)
    {
        this.gameStage = gameStage;
        this.estimatedStartingPoint = estimatedStartingPoint;
        SearchList = new List<Node> { estimatedStartingPoint};
    }

    public void CalculateRoute()
    {
        while(SearchList.Any())
        {
            var minCost = SearchList.Min(n => n.NodeCost);
            Node BestSoFar = SearchList.Find(n=>n.NodeCost == minCost);
            SearchList.Remove(BestSoFar);
        }

    }
}
internal class Node
{
    Node Parent;
    public int X { get; set; }
    public int Y { get; set; }
    public int ExtraElevatorsUsed { get; set; }
    public float NodeCost { get; set; }
    public float CostToGetHere {get; set;}
    public float OptimisticCostToFinish {get; set;}

    public Node(Stage GameStage)
    {
        Parent = null;
        Y = 0;
        string baseFloor = GameStage.Map[0].ToString();
        
        if(baseFloor.Count(c=> c == 'E') == 2)
            X = (baseFloor.IndexOf('E') + baseFloor.LastIndexOf('E')) / 2;
        else
            X = baseFloor.Length / 2;

        NodeCost = GameStage.ExitFloor + Math.Abs(GameStage.ExitPos - X);
    }
}

internal class Stage
{
    private readonly int floors;
    public int Width { get; private set; }
    public int ExitFloor { get; private set; }
    public int ExitPos { get; private set; }
    public int NbTotalClones { get; private set; }
    public int NbRounds { get; set; }
    public int NbExtraElevators { get; set; }
    public Char[][] Map {get; set;}
    public Stage()
    {
        string[] inputs = Console.ReadLine().Split(' ');
        Console.Error.WriteLine(String.Join(" ", inputs));
        floors = int.Parse(inputs[0]); // number of floors
        Width = int.Parse(inputs[1]); // width of the area
        Map = new Char[floors][];
        for (int i = 0; i < floors; i++)
        {
            Map[i] = new String('_', Width).ToCharArray();
        }
        NbRounds = int.Parse(inputs[2]); // maximum number of rounds
        ExitFloor = int.Parse(inputs[3]); // floor on which the exit is found
        ExitPos = int.Parse(inputs[4]); // position of the exit on its floor
        Map[ExitFloor][ExitPos] = 'X';
        NbTotalClones = int.Parse(inputs[5]); // number of generated clones
        NbExtraElevators = int.Parse(inputs[6]); // number of additional elevators that you can build
        int nbElevators = int.Parse(inputs[7]); // number of elevators
        for (int i = 0; i < nbElevators; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            Console.Error.WriteLine(String.Join(" ", inputs));
            int elevatorFloor = int.Parse(inputs[0]); // floor on which this elevator is found
            int elevatorPos = int.Parse(inputs[1]); // position of the elevator on its floor
            Map[elevatorFloor][elevatorPos] = 'E';

        }
    }
    public void AddOptionalElevators()
    {
        //Under the exit
        if( Map[ExitFloor-1][ExitPos] == '_')
            Map[ExitFloor-1][ExitPos] = 'O';
        // Everywere underneath
        for (int i = ExitFloor-1; i>0; i--)
        {
            for (int j = 0; j < Width; j++)
            {
                if(Map[i][j] != '_')
                {
                    if(Map[i-1][j] == '_')
                        Map[i-1][j] = 'O';
                }
            }
        }
    }

    public void PrintMap()
    {
        for (int i = floors-1; i >= 0; i--)
        {
            Console.Error.WriteLine(Map[i]);
        }
    }
}