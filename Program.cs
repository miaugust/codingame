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
        var PathSearcher = new BestPathSearcher(GameStage);

        bool start = true;
        // game loop
        while (true)
        {
            var inputs = Console.ReadLine().Split(' ');
            int cloneFloor = int.Parse(inputs[0]); // floor of the leading clone
            int clonePos = int.Parse(inputs[1]); // position of the leading clone on its floor
            string direction = inputs[2]; // direction of the leading clone: LEFT or RIGHT
            Console.Error.WriteLine(String.Join(" ", inputs));

            if (start)
            {
                GameStage.Map[cloneFloor][clonePos] = 'S';
                GameStage.PrintMap();
                Node staringPoint = new Node(GameStage, clonePos, cloneFloor, direction);
                PathSearcher.AddStartingPoint(staringPoint);
                PathSearcher.CalculateRoute();
                start = false;

            }

            (int y, int x) = PathSearcher.Path.Peek();
            string action = "WAIT";
            // Jeśli lider osiągną dotychczasowy cel
            if (cloneFloor == y && clonePos == x)
            {
                PathSearcher.Path.Pop();
                if (!PathSearcher.Path.Any())
                    action = "WAIT";
                else
                    (y, x) = PathSearcher.Path.Peek();
            }
            if (cloneFloor == -1 || clonePos == -1)
                action = "WAIT";
            else if (y > cloneFloor && GameStage.Map[cloneFloor][clonePos] == 'E')
                action = "WAIT";
            else if (y > cloneFloor && GameStage.Map[cloneFloor][clonePos] == 'O')
            {
                action = "ELEVATOR";
                GameStage.Map[cloneFloor][clonePos] = 'E';
            }

            else if (y == cloneFloor && clonePos > x && direction == "RIGHT")
                action = "BLOCK";
            else if (y == cloneFloor && clonePos < x && direction == "LEFT")
                action = "BLOCK";

            if (y == cloneFloor && GameStage.Map[cloneFloor][clonePos] == 'O')
                GameStage.Map[cloneFloor][clonePos] = '_';

            Console.WriteLine(action);
            GameStage.NbRounds--;
        }
    }
}
internal class BestPathSearcher
{
    private Stage gameStage;
    private List<Node> SearchList;
    private List<Node> ClosedList;
    public Stack<ValueTuple<int, int>> Path;
    private Node lastNode;

    public BestPathSearcher(Stage gameStage)
    {
        this.gameStage = gameStage;

        SearchList = new List<Node>();
        ClosedList = new List<Node>();
        Path = new Stack<(int, int)>();
        lastNode = new Node();
    }

    public void AddStartingPoint(Node node)
    {
        SearchList.Add(node);
    }

    public void CalculateRoute()
    {
        while (SearchList.Any())
        {
            var minCost = SearchList.Min(n => n.NodeCost);
            Node BestSoFar = SearchList.Find(n => n.NodeCost == minCost);
            SearchList.Remove(BestSoFar);
            List<Node> Neighbors = FindNeighbors(BestSoFar);
            foreach (var neighbor in Neighbors)
            {
                if (neighbor.X == gameStage.ExitPos && neighbor.Y == gameStage.ExitFloor)
                {
                    this.lastNode = neighbor;
                    SearchList.Clear();
                    break;
                }
                var sameOnOpen = SearchList.FirstOrDefault(n => n.X == neighbor.X &&
                    n.Y == neighbor.Y && n.ExtraElevatorsUsed == neighbor.ExtraElevatorsUsed && n.BotDirection == neighbor.BotDirection);
                var sameOnClosed = ClosedList.FirstOrDefault(n => n.X == neighbor.X &&
                    n.Y == neighbor.Y && n.ExtraElevatorsUsed == neighbor.ExtraElevatorsUsed && n.BotDirection == neighbor.BotDirection);
                if (sameOnOpen == null && sameOnClosed == null)
                    SearchList.Add(neighbor);
                else
                {
                    if (sameOnOpen != null && sameOnOpen.NodeCost > neighbor.NodeCost)
                        SearchList.Add(neighbor);
                    else if (sameOnClosed != null && sameOnClosed.NodeCost > neighbor.NodeCost)
                        SearchList.Add(neighbor);
                }
            }
            ClosedList.Add(BestSoFar);
        }

        while (lastNode.Parent != null)
        {
            Path.Push((lastNode.Y, lastNode.X));
            lastNode = lastNode.Parent;
        }


    }

    private List<Node> FindNeighbors(Node bestSoFar)
    {
        var Neighbors = new List<Node>();
        if (gameStage.Map[bestSoFar.Y][bestSoFar.X] == 'E')
        {
            var n = new Node(gameStage, bestSoFar, bestSoFar.X, bestSoFar.Y + 1);
            Neighbors.Add(n);
        }
        else
        {
            for (int x = bestSoFar.X - 1; x >= 0; x--)
            {
                if (gameStage.Map[bestSoFar.Y][x] != '_')
                {
                    var n = new Node(gameStage, bestSoFar, x, bestSoFar.Y);
                    Neighbors.Add(n);
                    break;
                }
            }
            for (int x = bestSoFar.X + 1; x < gameStage.Width; x++)
            {
                if (gameStage.Map[bestSoFar.Y][x] != '_')
                {
                    var n = new Node(gameStage, bestSoFar, x, bestSoFar.Y);
                    Neighbors.Add(n);
                    break;
                }
            }
        }

        if (gameStage.Map[bestSoFar.Y][bestSoFar.X] == 'O' && bestSoFar.ExtraElevatorsUsed < gameStage.NbExtraElevators)
        {
            var n = new Node(gameStage, bestSoFar, bestSoFar.X, bestSoFar.Y + 1);
            //n.ExtraElevatorsUsed = bestSoFar.ExtraElevatorsUsed + 1;
            Neighbors.Add(n);
        }
        return Neighbors;
    }
}
internal class Node
{
    public string BotDirection { get; set; }
    public Node Parent { get; private set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int ExtraElevatorsUsed { get; set; }
    public float NodeCost { get; set; }
    public float CostToGetHere { get; set; }
    public float OptimisticCostToFinish { get; set; }

    public Node(Stage GameStage, int x, int y, string botDirection)
    {
        Parent = null;
        Y = y;
        X = x;
        BotDirection = botDirection;
        var distanceToFinish = GameStage.ExitFloor + Math.Abs(GameStage.ExitPos - X);
        if ((botDirection == "RIGHT" && GameStage.ExitPos < x) || (botDirection == "LEFT" && GameStage.ExitPos > x))
            distanceToFinish += 3;

        NodeCost = OptimisticCostToFinish = distanceToFinish;
    }
    public Node(Stage GameStage, Node ParentNode, int x, int y)
    {
        Parent = ParentNode;
        X = x;
        Y = y;
        ExtraElevatorsUsed = Parent.ExtraElevatorsUsed;

        if (Y == ParentNode.Y + 1 && GameStage.Map[ParentNode.Y][ParentNode.X] == 'O')
            ExtraElevatorsUsed++;

        CostToGetHere = ParentNode.CostToGetHere + Math.Abs(x - ParentNode.X) + y - ParentNode.Y;
        if (X - ParentNode.X > 0)
            BotDirection = "RIGHT";
        else if (X - ParentNode.X < 0)
            BotDirection = "LEFT";
        else
            BotDirection = Parent.BotDirection;

        if (BotDirection != ParentNode.BotDirection)
            CostToGetHere += 3;

        OptimisticCostToFinish = Math.Abs(x - GameStage.ExitPos) + Math.Abs(y - GameStage.ExitFloor);
        if ((X < GameStage.ExitPos && BotDirection == "LEFT") || (X > GameStage.ExitPos && BotDirection == "RIGHT"))
            OptimisticCostToFinish += 3;

        NodeCost = CostToGetHere + OptimisticCostToFinish;

    }

    public Node()
    {
    }
    public override string ToString()
    {
        return $"X: {X} Y: {Y} E: {ExtraElevatorsUsed} D: {BotDirection} Cost: {NodeCost}";
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
    public Char[][] Map { get; set; }
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

        // Everywere underneath
        for (int i = ExitFloor; i > 0; i--)
        {
            for (int j = 0; j < Width; j++)
            {
                if (Map[i][j] == 'X' || (Map[i][j] == 'E' && i < ExitFloor))
                {
                    for (int k = 1; k <= NbExtraElevators; k++)
                    {
                        if (i - k >= 0 && Map[i - k][j] == '_')
                            Map[i - k][j] = 'O';
                    }

                }
            }
        }
    }

    public void PrintMap()
    {
        for (int i = floors - 1; i >= 0; i--)
        {
            Console.Error.WriteLine(Map[i]);
        }
    }
}