namespace RL_Distillation;

public class Program
{
    public static void Main()
    {
        var env = new RLEnvironment();
        var agent = new RLAgent();
        var totalEpisodes = 50;

        Console.WriteLine("Training the teacher agent...\n");
        Console.WriteLine("Legend: A = Agent, G = Goal, _ = Empty space\n");

        // Teacher training phase
        for (var episode = 0; episode < totalEpisodes; episode++)
        {
            env.Reset();
            var state = 0;
            var done = false;
            var steps = 0;
            var totalReward = 0.0;

            Console.WriteLine($"\nEpisode {episode + 1}/{totalEpisodes}");

            while (!done && steps < 50)
            {
                var action = agent.GetAction(state);
                var (nextState, reward, isDone) = env.Step(action);
                agent.Learn(state, action, reward, nextState);

                totalReward += reward;
                state = nextState;
                done = isDone;
                steps++;

                Console.WriteLine(
                    $"\r{env.Visualize()} | Step {steps} | Q-values: {agent.GetQTableVisualization(state)}");
                Thread.Sleep(100);
            }

            if (done)
            {
                agent.IncrementSuccess();
                Console.WriteLine($"\nSuccess! Reached goal in {steps} steps. Total reward: {totalReward:F2}");
            }
            else
            {
                Console.WriteLine($"\nFailed to reach goal. Total reward: {totalReward:F2}");
            }

            Console.WriteLine($"Success rate: {(double)agent.GetSuccessCount() / (episode + 1):P2}");
        }

        Console.WriteLine("\nTeacher training completed!");
        Console.WriteLine($"Teacher final success rate: {(double)agent.GetSuccessCount() / totalEpisodes:P2}");

        // Distillation phase
        Console.WriteLine("\nStarting knowledge distillation...");
        Console.WriteLine("Training students using both pure imitation and RL-enhanced learning...\n");

        var distillation = new Distillation(agent);
        distillation.Distill(25);  // Train students for 25 episodes

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}