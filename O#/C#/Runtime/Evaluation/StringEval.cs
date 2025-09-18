using System;

namespace OSharp.Runtime.Evaluation
{
    public class StringEvalResults
    {
        public string value;

        public StringEvalResults(string value)
        {
            this.value = value;
        }
    }

    public class StringEval
    {
        public StringEvalResults EvalString(string s1, string op, string s2)
        {
            switch (op)
            {
                case "+":
                    // Concatenate strings
                    return new StringEvalResults(s1 + s2);

                case "==":
                    return new StringEvalResults((s1 == s2).ToString());

                case "!=":
                    return new StringEvalResults((s1 != s2).ToString());

                case ">":
                    return new StringEvalResults((string.Compare(s1, s2) > 0).ToString());

                case "<":
                    return new StringEvalResults((string.Compare(s1, s2) < 0).ToString());

                case ">=":
                    return new StringEvalResults((string.Compare(s1, s2) >= 0).ToString());

                case "<=":
                    return new StringEvalResults((string.Compare(s1, s2) <= 0).ToString());

                default:
                    throw new ArgumentException($"Unsupported operator '{op}'");
            }
        }
    }
}
