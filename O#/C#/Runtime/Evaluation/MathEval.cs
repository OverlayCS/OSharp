using System;

namespace OSharp.Runtime.Evaluation
{
    public class MathEvalResults
    {
        public string oper;
        public int value;

        public MathEvalResults(string oper, int value)
        {
            this.oper = oper;
            this.value = value;
        }
    }

    public class MathEval
    {
        public MathEvalResults EvalMath(int value1, string op, int value2)
        {
            switch (op)
            {
                case "+":
                    return new MathEvalResults(op, value1 + value2);

                case "-":
                    return new MathEvalResults(op, value1 - value2);

                case "*":
                    return new MathEvalResults(op, value1 * value2);

                case "/":
                    if (value2 == 0)
                        throw new DivideByZeroException("Cannot divide by zero");
                    return new MathEvalResults(op, value1 / value2);

                case "%":
                    if (value2 == 0)
                        throw new DivideByZeroException("Cannot modulo by zero");
                    return new MathEvalResults(op, value1 % value2);

                case "==":
                    return new MathEvalResults(op, value1 == value2 ? 1 : 0);

                case "!=":
                    return new MathEvalResults(op, value1 != value2 ? 1 : 0);

                case ">":
                    return new MathEvalResults(op, value1 > value2 ? 1 : 0);

                case "<":
                    return new MathEvalResults(op, value1 < value2 ? 1 : 0);

                case ">=":
                    return new MathEvalResults(op, value1 >= value2 ? 1 : 0);

                case "<=":
                    return new MathEvalResults(op, value1 <= value2 ? 1 : 0);

                default:
                    throw new ArgumentException($"Unsupported operator '{op}'");
            }
        }
    }
}
