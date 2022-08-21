namespace AltAntlr
{
    using System.Collections.Generic;

    public class Util
    {
        static Dictionary<string, List<int>> _cache = new Dictionary<string, List<int>>();
        private static List<int> ComputeIndexes(string buffer)
        {
            List<int> indices = new List<int>();
            int cur_index = 0;
            int cur_line = 0; // zero based LSP.
            int cur_col = 0; // zero based LSP.
            indices.Add(cur_index);
            int length = buffer.Length;
            // Go through file and record index of start of each line.
            for (int i = 0; i < length; ++i)
            {
                if (cur_index >= length)
                {
                    break;
                }

                char ch = buffer[cur_index];
                if (ch == '\r')
                {
                    if (cur_index + 1 >= length)
                    {
                        break;
                    }
                    else if (buffer[cur_index + 1] == '\n')
                    {
                        cur_line++;
                        cur_col = 0;
                        cur_index += 2;
                        indices.Add(cur_index);
                    }
                    else
                    {
                        // Error in code.
                        cur_line++;
                        cur_col = 0;
                        cur_index += 1;
                        indices.Add(cur_index);
                    }
                }
                else if (ch == '\n')
                {
                    cur_line++;
                    cur_col = 0;
                    cur_index += 1;
                    indices.Add(cur_index);
                }
                else
                {
                    cur_col += 1;
                    cur_index += 1;
                }
                if (cur_index >= length)
                {
                    break;
                }
            }
            return indices;
        }

        public static int GetIndex(int line, int column, string buffer)
        {
            if (buffer == null)
            {
                return 0;
            }

            _cache.TryGetValue(buffer, out List<int> indices);
            if (indices == null)
            {
                indices = ComputeIndexes(buffer);
                _cache.Add(buffer, indices);
            }
            int low = indices[line - 1];
            var index = low + column;
            return index;
        }

        public static (int, int) GetLineColumn(int index, string buffer)
        {
            if (buffer == null)
            {
                return (0, 0);
            }

            _cache.TryGetValue(buffer, out List<int> indices);
            if (indices == null)
            {
                indices = ComputeIndexes(buffer);
                _cache.Add(buffer, indices);
            }
            // Binary search.
            int low = 0;
            int high = indices.Count - 1;
            int i = 0;
            while (low <= high)
            {
                i = (low + high) / 2;
                var v = indices[i];
                if (v < index) low = i + 1;
                else if (v > index) high = i - 1;
                else break;
            }
            var min = low <= high ? i : high;
            var myindex = (min + 1, index - indices[min]);
            return myindex;
        }
    }
}
