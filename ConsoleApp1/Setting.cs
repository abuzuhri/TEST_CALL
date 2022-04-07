namespace ConsoleApp1
{
    public static class Setting
    {
        public const string NOTFOUND = "N/A";
        public const int sleepTime = 0;

        public static string GetValue(string content, int startIndex, string startText, string secondStartText, string endText, ref int indexer, string remove = null)
        {
            int pos1 = 0, pos2 = 0;
            pos1 = content.IndexOf(startText, startIndex);
            if (pos1 != -1)
            {
                pos1 = pos1 + startText.Length;
                if (!string.IsNullOrWhiteSpace(secondStartText))
                {
                    pos1 = content.IndexOf(secondStartText, pos1) + secondStartText.Length;
                }
                pos2 = content.IndexOf(endText, pos1);
                if (pos2 != -1)
                {
                    string text = content.Substring(pos1, pos2 - pos1);

                    if (!string.IsNullOrWhiteSpace(remove))
                    {
                        var removes = remove.Split(';');
                        foreach (var r in removes)
                        {
                            text = text.Replace(r, "");
                        }
                    }
                    indexer = pos2;
                    return text.Trim();
                }
                else return NOTFOUND;

            }
            else
            {
                return NOTFOUND;
            }

        }


    }
}
