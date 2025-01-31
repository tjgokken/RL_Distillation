namespace RL_Distillation;

public class Distillation(RLAgent teacher)
{
    private readonly Student learningStudent = new(true);
    private readonly Student cluelessStudent = new(false);
    private readonly List<(double learning, double clueless)> successRates = new();
    private readonly Random random = new();

    public void Distill(int episodes)
    {
        var env = new RLEnvironment();
        Console.WriteLine("\n=== Starting Learning Comparison ===");

        for (var episode = 0; episode < episodes; episode++)
        {
            TrainEpisode(env, episode);
            var (learningSuccess, cluelessSuccess) = TestStudents(env);
            successRates.Add((learningSuccess, cluelessSuccess));
            ShowPerformanceMetrics(episode);
        }

        ShowFinalResults();
    }

    private void TrainEpisode(RLEnvironment env, int episode)
    {
        var startPos = random.Next(0, 11);
        Console.WriteLine($"\nEpisode {episode + 1} (Starting from position {startPos})");

        // Teacher demonstrates
        env.Reset();
        env.SetState(startPos);
        var state = startPos;
        var done = false;
        var steps = 0;

        Console.WriteLine("\nTeacher demonstrating:");
        while (!done && steps < 50)
        {
            var teacherAction = teacher.GetAction(state);
            learningStudent.Learn(state, teacherAction);

            var (nextState, _, isDone) = env.Step(teacherAction);
            Console.WriteLine($"{env.Visualize()} | Action: {(teacherAction == 0 ? "Left" : "Right")}");

            state = nextState;
            done = isDone;
            steps++;
            Thread.Sleep(25);
        }
    }

    private (double learningRate, double cluelessRate) TestStudents(RLEnvironment env)
    {
        int testEpisodes = 5;
        int learningSuccesses = 0, cluelessSuccesses = 0;

        for (int i = 0; i < testEpisodes; i++)
        {
            int startPos = random.Next(0, 11);

            if (RunTest(env, learningStudent, startPos)) learningSuccesses++;
            if (RunTest(env, cluelessStudent, startPos)) cluelessSuccesses++;
        }

        return ((double)learningSuccesses / testEpisodes, (double)cluelessSuccesses / testEpisodes);
    }

    private bool RunTest(RLEnvironment env, Student student, int startPos)
    {
        env.Reset();
        env.SetState(startPos);
        var state = startPos;
        var done = false;
        var steps = 0;
        var maxSteps = 20;

        while (!done && steps < maxSteps)
        {
            var action = student.GetAction(state);
            var (nextState, _, isDone) = env.Step(action);
            state = nextState;
            done = isDone;
            steps++;
        }

        return done;
    }

    private void ShowPerformanceMetrics(int episode)
    {
        var windowSize = Math.Min(5, successRates.Count);
        var recentRates = successRates.TakeLast(windowSize).ToList();
        var avgLearning = recentRates.Average(r => r.learning);
        var avgClueless = recentRates.Average(r => r.clueless);

        Console.WriteLine("\n=== Recent Performance (Last 5 Episodes) ===");
        Console.WriteLine($"Learning Student: {avgLearning:P2}");
        Console.WriteLine($"Clueless Student: {avgClueless:P2}");
    }

    private void ShowFinalResults()
    {
        var finalLearningRate = successRates.Average(r => r.learning);
        var finalCluelessRate = successRates.Average(r => r.clueless);

        Console.WriteLine("\n=== Final Results ===");
        Console.WriteLine($"Learning Student: {finalLearningRate:P2}");
        Console.WriteLine($"Clueless Student: {finalCluelessRate:P2}");
    }
}