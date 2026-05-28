using System.Security.Cryptography;
using System.Text;

namespace EnglishQuizApp.Helpers
{
    public static class QuestionHashHelper
    {
        public static string GenerateHash(QuestionSeed q)
        {
            var sb = new StringBuilder();

            sb.Append(q.Text);
            sb.Append("|");
            sb.Append(q.Category);
            sb.Append("|");
            sb.Append(q.Difficulty);

            foreach (var a in q.Answers)
            {
                sb.Append("|");
                sb.Append(a.Text);
                sb.Append("|");
                sb.Append(a.IsCorrect);
            }

            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));

            return Convert.ToHexString(bytes);
        }
    }
}