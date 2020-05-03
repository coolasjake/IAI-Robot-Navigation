using System;

namespace IAI_Robot_Nav
{
    class Program
    {
        static void Main(string[] args)
        {
            Environment env;

            //If there is at least one argument, begin execution, otherwise print an error.
            if (args.Length > 0)
            {
                //If the first argument (the 'command') is custom etc, build a custom environment instead of searching.
                if (args[0].ToLower() == "custom" || args[0].ToLower() == "new" || args[0].ToLower() == "environment")
                    env = new Environment(EnvironmentBuilder.BuildEnvironment());
                else if (args[0].ToLower() == "print")
                {
                    if (args.Length > 1)
                    {
                        Console.WriteLine("Printing Environment.");
                        env = new Environment(args[1]);
                        env.PrintEnvironment();
                    }
                    else
                        Console.WriteLine("Please specify an environment to print.");
                }
                else if (args[0].ToLower() == "search" || args[0].ToLower() == "nicesearch")
                {
                    if (args.Length > 1)
                    {
                        //If the environment name is 'custom' build and use a custom environment, otherwise use the specified one.
                        if (args[1].ToLower() == "custom" || args[1].ToLower() == "new")
                            env = new Environment(EnvironmentBuilder.BuildEnvironment());
                        else
                            env = new Environment(args[1]);
                    }
                    else
                        env = new Environment();

                    //Get the bot specified by arg 3, using BFS as a default.
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

                    //Do either a normal or nice search (nice gives much more detailed information like number of iterations).
                    if (args[0].ToLower() == "search")
                        bot.FindSolution();
                    else
                        bot.PFindSolution();
                }
                else
                    Console.WriteLine("Unknown Command.");
            }
            else
            {
                Console.WriteLine("Unknown Command");
                Console.WriteLine("Make sure you run this with arguments from a console.");
                Console.WriteLine("Use 'search <environment> <method>' to search");
                Console.WriteLine("Use 'custom' to create a custom environment");
                Console.WriteLine("Use 'search custom <method>' to create a custom environment and search");
                Console.ReadLine();
            }
        }
    }
}
