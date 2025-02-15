﻿namespace RL_Distillation;

/// <summary>
/// Represents a student that can learn from a teacher or goes at it by self
/// </summary>
public class Student(bool isLearning)
{
    private readonly Dictionary<int, int> learnedMoves = new();
    private readonly Random random = new();

    public void Learn(int state, int teacherAction)
    {
        if (isLearning)
        {
            learnedMoves[state] = teacherAction;
        }
    }

    public int GetAction(int state)
    {
        if (isLearning && learnedMoves.ContainsKey(state))
        {
            return learnedMoves[state];
        }
        return random.Next(2);
    }

    public string GetType() => isLearning ? "Learning" : "Clueless";
}