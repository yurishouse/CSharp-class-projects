using System;
using static System.Console;
using System.Threading;
using NLog;

namespace GameOfLife
{
    class Program
    {
        const int CellState_Dead = 0;             // Using a grid of 0's and 1's will help us count
        const int CellState_Alive = 1;            //   count neighbors efficiently in the Life program.

        const int RunMode_Interactive = 0;
        const int RunMode_Slient = 1;

        
        const int GridInitType_RPentamino = 0;
        const int GridInitType_Random = 1;
        

        static int GridSizeX = 25;
        static int GridSizeY = 25;

        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            logger.Info("=== Starting Program ===");
            // Parsing Parameter
            int[] parsedParam = ParseCLIParameters(args);
            int RunMode = parsedParam[0];                   // RunMode from arg0 Interactive / Silent
            int GridInitType = parsedParam[1];              // Grid initialization type, by random or by RPentamino
            int GridFillDensity = parsedParam[2];           // Grid Fill Density, only meaningful when Grid is filled by random, 0-100
            int SimulationCycleCount = parsedParam[3];      // Simulation Cycle Counts, -1 means infinite cycle.

            int gridCount = 0;
            int[,] grid = new int[GridSizeX, GridSizeY];

            bool done = false;
            int eproh = 0;
            InitGrid(GridInitType,GridFillDensity,grid);
            while (!done)
            {
                Grow(grid);
                eproh++;
                if ((eproh>=SimulationCycleCount)&&(SimulationCycleCount>0)){
                    done = true;
                }
                logger.Debug($"Grid #{gridCount}  aliveCount: {CountLiveCells(grid)}");

                switch (RunMode){
                    case RunMode_Interactive:
                        // Display the grid (and log its statistics)
                        WriteLine($"Grid #{gridCount}");
                        PrintGrid(grid);
                        break;
                    case RunMode_Slient:
                        if (!done) break; // Only Print on last eproh
                        WriteLine($"Grid #{gridCount}");
                        PrintGrid(grid);
                        break;
                    default:
                        logger.Error("Unreachable Code (Enum): RunMode, Program exiting");
                        return;
                }

                Thread.Sleep(500);

                // Check to see if the user pressed a key
                if (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;
                    logger.Debug($"{key} pressed...");
                    switch (key){
                        case ConsoleKey.F:
                            GridInitType = GridInitType_Random;
                            if (GridFillDensity<0) GridFillDensity = 20;
                            InitGrid(GridInitType,GridFillDensity,grid);
                            break;
                        case ConsoleKey.R:
                            GridInitType = GridInitType_RPentamino;
                            InitGrid(GridInitType,GridFillDensity,grid);
                            break;
                        case ConsoleKey.Q: // exit
                            done = true;
                            break;
                        default: // Unknown Key, ignored
                            break;
                    }
                }

                gridCount++;            // Increment at bottom of loop so that first grid displayed is Grid #0
            }
            logger.Info("=== Ending Program ===");
        }

        static void InitGrid(int GridInitType, int GridFillDensity, int[,] grid){
            // Randomly Fill the Grid
            switch (GridInitType){
                case GridInitType_Random:
                    FillGridRandomly(grid, GridFillDensity);
                    break;
                case GridInitType_RPentamino:
                    FillGridRPentamino(grid);
                    break;
                default:
                    logger.Error("Unreachable Code (Enum): GridInitType, Program exiting");
                    return;
            }
        }

        static int[] ParseCLIParameters(string[] args){
            int RunMode;
            int GridInitType;
            int GridFillDensity = -1;
            int SimulationCycleCount;
            if (args.Length<1)
             { // 1st argument DNE
                RunMode = RunMode_Interactive;
                GridInitType = GridInitType_RPentamino;
                SimulationCycleCount = -1;
                logger.Info("0 Param Received, Default");
            } else {
                logger.Info("Param 1: "+args[0]);
                if (args[0].ToLower().ToCharArray()[0]=='i') {
                    RunMode = RunMode_Interactive;
                } else {
                    RunMode = RunMode_Slient;
                }
                if (args.Length<2)
                {
                    GridInitType = GridInitType_RPentamino;
                    SimulationCycleCount = (RunMode == RunMode_Interactive)?-1:50;
                } else {
                    logger.Info("Param 2: "+args[1]);
                    if (args[1].ToLower().ToCharArray()[0]=='r')
                    {
                        GridInitType = GridInitType_RPentamino;
                    } else {
                        GridInitType = GridInitType_Random;
                        try{
                            GridFillDensity = Int32.Parse(args[1]);
                            if (GridFillDensity>100)
                            {
                                logger.Warn("GridFillDensity Over 100%, trim to 100.");
                                GridFillDensity = 100;
                            } else if (GridFillDensity<0){
                                logger.Warn("GridFillDensity Lower than 0%, pad to 0.");
                                GridFillDensity = 0;
                            }
                        } catch (Exception e){
                            logger.Error("Int Conversion Failed when parsing argument 2, defaults to Rpentamino");
                            GridInitType = GridInitType_RPentamino;
                        }
                    }
                    if (args.Length<3)
                    {
                        SimulationCycleCount = (RunMode == RunMode_Interactive)?-1:50;
                    } else {
                        logger.Info("Param 3: "+args[2]);
                        try{
                            SimulationCycleCount = Int32.Parse(args[2]);
                            if (SimulationCycleCount<1)
                            {
                                SimulationCycleCount = -1;
                            }
                        } catch (Exception e){
                            logger.Error("Int Conversion Failed when parsing argument 3, failover to defaults");
                            SimulationCycleCount = (RunMode == RunMode_Interactive)?-1:50;
                        }
                    }
                }
            }
            logger.Info("RunMode: "+((RunMode == RunMode_Interactive)?"Interactive": "Silent"));
            logger.Info("GridInitType: "+ ((GridInitType==GridInitType_Random)?"Random":"RPentamino"));
            logger.Info("GridFillDensity: "+GridFillDensity);
            if (SimulationCycleCount<0)
            {
                logger.Info("SimulationCycleCount: Infinite");
            } else {
                logger.Info("SimulationCycleCount: "+SimulationCycleCount);
            }
            int[] resArr = new int[4];
            resArr[0] = RunMode;
            resArr[1] = GridInitType;
            resArr[2] = GridFillDensity;
            resArr[3] = SimulationCycleCount;
            return resArr;
        }

        private static void Grow(int[,] grid) {
            // https://gist.github.com/lennartb-/7482783
            for (int x=0; x < GridSizeX; x++)
            {
                for (int y=0; y < GridSizeY; y++)
                {
                    int numOfAliveNeighbors = GetNeighbors(grid, x,y);

                    if (grid[x, y]==CellState_Alive) {
                        if (numOfAliveNeighbors < 2) {
                            grid[x, y] = CellState_Dead;
                        }

                        if (numOfAliveNeighbors > 3) {
                            grid[x, y] = CellState_Dead;
                        }
                    }
                    else {
                        if (numOfAliveNeighbors == 3) {
                            grid[x, y] = CellState_Alive;
                        }
                    }
                }
            }
        }


        private static int GetNeighbors(int[,] grid, int x, int y) {
            int NumOfAliveNeighbors = 0;

            for (int i = x - 1; i < x + 2; i++) {
                for (int j = y - 1; j < y + 2; j++) {
                    if (!((i < 0 || j < 0) || (i >= GridSizeX || j >= GridSizeY))) {
                        if (grid[i, j] == CellState_Alive) NumOfAliveNeighbors++;
                    }
                }
            }
            return NumOfAliveNeighbors;
        }

        static void FillGridRandomly(int[,] grid, int fillPercentage)
        {
            for (int x=0; x < GridSizeX; x++)
            {
                for (int y=0; y < GridSizeY; y++)
                {
                    if (RandomBool(fillPercentage) == true)
                        grid[x,y] = CellState_Alive;
                    else
                        grid[x,y] = CellState_Dead;
                }
            }
        }

        static void FillGridRPentamino(int[,] grid)
        {
            for (int x=0; x < GridSizeX; x++)
            {
                for (int y=0; y < GridSizeY; y++)
                {
                    grid[x,y] = CellState_Dead;
                }
            }
            //Random random = new Random();
            //int x0=random.Next() % (GridSizeX-3);
            //int y0=random.Next() % (GridSizeY-3);
            //int orientation = random.Next() % 4;
            int x0=GridSizeX/2;// Put Pattern at center
            int y0=GridSizeY/2;
            int orientation = 0;
            grid[x0+1,y0+1] = CellState_Alive;
            switch(orientation){
                case 0:
                    grid[x0+1,y0+0] = CellState_Alive;
                    grid[x0+2,y0+0] = CellState_Alive;
                    grid[x0+0,y0+1] = CellState_Alive;
                    grid[x0+1,y0+2] = CellState_Alive;
                    break;
                case 1:
                    grid[x0+1,y0+0] = CellState_Alive;
                    grid[x0+0,y0+1] = CellState_Alive;
                    grid[x0+2,y0+1] = CellState_Alive;
                    grid[x0+2,y0+2] = CellState_Alive;
                    break;
                case 2:
                    grid[x0+1,y0+0] = CellState_Alive;
                    grid[x0+0,y0+2] = CellState_Alive;
                    grid[x0+2,y0+1] = CellState_Alive;
                    grid[x0+1,y0+2] = CellState_Alive;
                    break;
                case 3:
                    grid[x0+0,y0+0] = CellState_Alive;
                    grid[x0+0,y0+1] = CellState_Alive;
                    grid[x0+1,y0+2] = CellState_Alive;
                    grid[x0+2,y0+1] = CellState_Alive;
                    break;
                default:
                    logger.Error("Unreachable (logic), UID: 5836C324-24D6-4EBE-9653-8ED411CA9D0C");
                    return;
            }
        }

        static bool RandomBool(int percent)
        {
            Random rng = new Random();
            return (rng.Next() % 100 < percent);
        }

        static void PrintGrid(int[,] grid)
        {
            WriteLine($"+{Dashes(GridSizeX*3)}+");
            for (int y=0; y < GridSizeY; y++)
            {
                string s = "|";
                for (int x=0; x < GridSizeX; x++)
                {
                    string cell = (grid[x,y] == CellState_Alive) ? " * " : "   ";
                    s += cell;
                }
                s += "|";
                WriteLine(s);
            }
            WriteLine($"+{Dashes(GridSizeX*3)}+");
        }

        static string Dashes(int number)
        {
            return new string('-', number);
        }

        static int CountLiveCells(int[,] grid)
        {
            int count = 0;
            for (int x=0; x < GridSizeX; x++)
                for (int y=0; y < GridSizeY; y++)
                    if (grid[x,y] == CellState_Alive)
                        count++;
            return count;
        }
    }
}