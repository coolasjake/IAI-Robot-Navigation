using System;

namespace IAI_Robot_Nav
{
    class Program
    {
        static void Main(string[] args)
        {
            Environment env;


            if (args.Length > 0)
            {
                if (args[0].ToLower() == "custom" || args[0].ToLower() == "new" || args[0].ToLower() == "environment")
                    env = new Environment(EnvironmentBuilder.BuildEnvironment());
                else if (args[0].ToLower() == "search" || args[0].ToLower() == "nicesearch")
                {
                    if (args.Length > 1)
                    {
                        if (args[1].ToLower() == "custom" || args[1].ToLower() == "new")
                            env = new Environment(EnvironmentBuilder.BuildEnvironment());
                        else
                            env = new Environment(args[1]);
                    }
                    else
                        env = new Environment();

                    Robot bot = new BOT_BreadthFirst(env);
                    if (args.Length > 2)
                    {
                        if (args[2].ToUpper() == "BFS")
                            bot = new BOT_BreadthFirst(env);
                        else if (args[2].ToUpper() == "AS")
                            bot = new BOT_AStar(env);
                        else if (args[2].ToUpper() == "GBFS")
                            bot = new BOT_GBFS(env);
                        else if (args[2].ToUpper() == "DFS")
                            bot = new BOT_DepthFirst(env);
                        else if (args[2].ToUpper() == "CUS1")
                            bot = new BOT_LHW(env);
                        else if (args[2].ToUpper() == "CUS2")
                            bot = new BOT_Handshake(env);
                        else
                            Console.WriteLine("Argument doesn't match any Bots. Using BFS.");
                    }
                    if (args[0].ToLower() == "search")
                        bot.FindSolution();
                    else
                        bot.PFindSolution();
                }
            }
            else
            {
                Console.WriteLine("Unknown Command");
                Console.WriteLine("Use 'search <environment> <method>' to search");
                Console.WriteLine("Use 'custom' to create a custom environment");
                Console.WriteLine("Use 'search custom <method>' to create a custom environment and search");
            }
        }
    }
}
