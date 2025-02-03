namespace RL_Distillation;

public class LearningSummary
{
    private readonly List<EpisodeResult> results = new();

    public record EpisodeResult(
        int Episode,
        double SuccessRate,
        double TotalReward,
        int Steps
    );

    public void AddResult(int episode, double successRate, double totalReward, int steps)
    {
        results.Add(new EpisodeResult(episode, successRate, totalReward, steps));
    }

    public void DisplaySummaryChart()
    {
        const int width = 80;  // Fixed width for better formatting
        const int height = 20; // Height of the chart

        Console.WriteLine("\nLearning Progress Summary");
        Console.WriteLine("========================");

        // Calculate moving averages for smoother curves
        var windowSize = 5;
        var smoothedSuccessRates = CalculateMovingAverage(results.Select(r => r.SuccessRate), windowSize);
        var smoothedRewards = CalculateMovingAverage(results.Select(r => r.TotalReward), windowSize);

        // Normalize rewards to 0-1 range for plotting
        var maxReward = smoothedRewards.Max();
        var minReward = smoothedRewards.Min();
        var normalizedRewards = smoothedRewards
            .Select(r => (r - minReward) / (maxReward - minReward))
            .ToList();

        // Create the chart
        var chart = new char[height, width];
        for (var y = 0; y < height; y++)
        for (var x = 0; x < width; x++)
            chart[y, x] = ' ';

        // Plot success rates and rewards
        for (var x = 0; x < Math.Min(width, smoothedSuccessRates.Count); x++)
        {
            var successY = (int)((1 - smoothedSuccessRates[x]) * (height - 1));
            var rewardY = (int)((1 - normalizedRewards[x]) * (height - 1));

            successY = Math.Max(0, Math.Min(height - 1, successY));
            rewardY = Math.Max(0, Math.Min(height - 1, rewardY));

            chart[successY, x] = '•';  // Success rate point
            chart[rewardY, x] = '×';   // Reward point
        }

        // Print the chart with labels
        Console.WriteLine("\nSuccess Rate (•) and Normalized Reward (×) over Episodes");
        Console.WriteLine(new string('-', width + 6));

        for (var y = 0; y < height; y++)
        {
            // Y-axis labels
            if (y == 0)
                Console.Write("100% |");
            else if (y == height - 1)
                Console.Write("  0% |");
            else if (y == height / 2)
                Console.Write(" 50% |");
            else
                Console.Write("     |");

            // Plot points
            for (var x = 0; x < width; x++)
                Console.Write(chart[y, x]);
            Console.WriteLine();
        }
        Console.WriteLine(new string('-', width + 6));
        Console.WriteLine($"0{new string(' ', width - 8)}Episodes{results.Count}");

        // Print statistics
        Console.WriteLine("\nFinal Statistics:");
        Console.WriteLine($"Final Success Rate: {results.Last().SuccessRate:P2}");
        Console.WriteLine($"Best Reward: {maxReward:F2}");
        Console.WriteLine($"Average Steps per Success: {results.Where(r => r.TotalReward > 0).Average(r => r.Steps):F1}");
        Console.WriteLine($"Total Episodes: {results.Count}");
    }

    private List<double> CalculateMovingAverage(IEnumerable<double> values, int windowSize)
    {
        var list = values.ToList();
        var result = new List<double>();

        for (var i = 0; i < list.Count; i++)
        {
            var windowStart = Math.Max(0, i - windowSize + 1);
            var windowEnd = i + 1;
            var windowValues = list.GetRange(windowStart, windowEnd - windowStart);
            result.Add(windowValues.Average());
        }

        return result;
    }
}