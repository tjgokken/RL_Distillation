namespace RL_Distillation;

public class Program
{
    public static void Main()
    {
        var env = new RLEnvironment();
        var agent = new RLAgent();
        var summary = new LearningSummary();
        var totalEpisodes = 50;

        Console.WriteLine("Training the teacher agent...");

        for (var episode = 0; episode < totalEpisodes; episode++)
        {
            env.Reset();
            var state = 0;
            var done = false;
            var steps = 0;
            var totalReward = 0.0;

            // Show simple progress indicator
            Console.Write($"\rEpisode {episode + 1}/{totalEpisodes}");

            while (!done && steps < 50)
            {
                var action = agent.GetAction(state);
                var (nextState, reward, isDone) = env.Step(action);
                agent.Learn(state, action, reward, nextState);

                totalReward += reward;
                state = nextState;
                done = isDone;
                steps++;
            }

            if (done) agent.IncrementSuccess();

            var successRate = (double)agent.GetSuccessCount() / (episode + 1);
            summary.AddResult(episode + 1, successRate, totalReward, steps);
        }

        Console.Clear();
        

        Console.WriteLine("\nStarting knowledge distillation...");
        var distillation = new Distillation(agent);
        distillation.Distill(25);

        summary.DisplaySummaryChart();

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}