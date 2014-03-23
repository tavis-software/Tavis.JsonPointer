using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Tavis
{
    public class JsonPointer
    {
        private readonly string[] _Tokens;

        public JsonPointer(string pointer)
        {
            _Tokens = pointer.Split('/').Skip(1).Select(Decode).ToArray();
        }

        private string Decode(string token)
        {
            return Uri.UnescapeDataString(token).Replace("~1", "/").Replace("~0", "~");
        }

        public JToken Find(JToken sample)
        {
            if (_Tokens.Length == 0)
            {
                return sample;
            }

            var pointer = sample;
            foreach (var token in _Tokens)
            {
                if (pointer is JArray)
                {
                    pointer = pointer[Convert.ToInt32(token)];
                }
                else
                {
                    pointer = pointer[token];
                }
            }
            return pointer;
        }
    }
}
