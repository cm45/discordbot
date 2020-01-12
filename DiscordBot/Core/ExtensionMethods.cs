using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordBot.Core
{
    public static class ExtensionMethods
    {
        public static string RemoveWhitespacesAndSeperators(this string input)
        {
            // TODO: simplify
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c) && c != '/' && c != '-' && c != '.' && c != '"' && c != '(' && c != ')')
                .ToArray());
        }
    }
}
