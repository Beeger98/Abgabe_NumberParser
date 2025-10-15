using System.Text;

namespace NumberParser.Infratructure
{
    static class StringCleaner
    {

        public static string DropTabs(string line)
        {
            var sb = new StringBuilder();
            foreach (var t in line)
                sb.Append(t == '\t' ? new string(' ', 4 - sb.Length % 4) : t);

            return sb.ToString();
        }
    }
}
