using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace AssistantProcessor.Models
{
    public class FilterPatterns
    {
        public List<string> taskPattern;
        public List<string> correctAnswerPattern;
        public List<string> wrongAnswerPattern;
        public bool tasksDividedByNewRow;

        [JsonIgnore] public List<Regex> taskRegexes = new List<Regex>();
        [JsonIgnore] public List<Regex> correctAnswerRegexes = new List<Regex>();
        [JsonIgnore] public List<Regex> wrongAnswerRegexes = new List<Regex>();

        public FilterPatterns(IReadOnlyList<bool> includedTask, IReadOnlyList<bool> includedCorrect, IReadOnlyList<bool> includedWrong, bool tasksDividedByNewRow)
        {
            this.tasksDividedByNewRow = tasksDividedByNewRow;
            for (int i = 0; i < includedTask.Count; i++)
            {
                if (includedTask[i])
                {
                    taskRegexes.Add(taskPatterns[i]);
                }
            }
            for (int i = 0; i < includedCorrect.Count; i++)
            {
                if (includedCorrect[i])
                {
                    correctAnswerRegexes.Add(trueAnswerPatterns[i]);
                }
            }
            for (int i = 0; i < includedWrong.Count; i++)
            {
                if (includedWrong[i])
                {
                    wrongAnswerRegexes.Add(falseAnswerPatterns[i]);
                }
            }
        }

        public void Encode()
        {
            taskPattern = new List<string>();
            foreach (Regex regex in taskRegexes)
            {
                taskPattern.Add(regex.ToString());
            }
            correctAnswerPattern = new List<string>();
            foreach (Regex regex in correctAnswerRegexes)
            {
                correctAnswerPattern.Add(regex.ToString());
            }
            wrongAnswerPattern = new List<string>();
            foreach (Regex regex in wrongAnswerRegexes)
            {
                wrongAnswerPattern.Add(regex.ToString());
            }
        }

        public void Decode()
        {
            taskRegexes = new List<Regex>();
            foreach (var regex in taskPattern)
            {
                taskRegexes.Add(new Regex(regex));
            }
            correctAnswerRegexes = new List<Regex>();
            foreach (var regex in correctAnswerPattern)
            {
                taskRegexes.Add(new Regex(regex));
            }
            wrongAnswerRegexes = new List<Regex>();
            foreach (var regex in wrongAnswerPattern)
            {
                taskRegexes.Add(new Regex(regex));
            }
        }

        public static Regex[] taskPatterns =
        {
            new Regex(@"^\d+\."),       //"13."
            new Regex(@"^\d+\.d+\."),   //"21.23. "
            new Regex(@"^\d+\s"),       //"13 "
            new Regex(@"^\d+\.\d+\s")   //"13.13 "
        };

        public static Regex[] trueAnswerPatterns =
        {
            new Regex(@"^[A|А]\."),     //"A."
            new Regex(@"^1\."),         //"1."
            new Regex(@"^[a|а]\."),     //"a."
            new Regex(@"^[A|А]\s"),     //"A "
            new Regex(@"^1\s"),         //"1 "
            new Regex(@"^[a|а]\s"),     //"a "
            new Regex(@"^\d\*"),        //"2*"
            new Regex(@"^\d\.\*"),      //"2.*"
            new Regex(@"^\d\s\*"),      //"2 *"
            new Regex(@"^\d\.\s\*"),    //"2. *"
            new Regex(@"^\w\*"),        //"c*"
            new Regex(@"^\w\.\*"),      //"c.*"
            new Regex(@"^\w\s\*"),      //"c *"
            new Regex(@"^\w\.\s\*"),    //"c. *"
            new Regex(@"^\d\+"),        //"2+"
            new Regex(@"^\d\.\+"),      //"2.+"
            new Regex(@"^\d\s\+"),      //"2 +"
            new Regex(@"^\d\.\s\+"),    //"2. +"
            new Regex(@"^\w\+"),        //"c+"
            new Regex(@"^\w\.\+"),      //"c.+"
            new Regex(@"^\w\s\+"),      //"c +"
            new Regex(@"^\w\.\s\+"),    //"c. +"
            new Regex(@"^\*"),          //"*text" & "* text"
            new Regex(@"^\+")           //"+text" & "+ text"
        };

        public static Regex[] falseAnswerPatterns =
        {
            new Regex(@"^\d\."),        //"2."
            new Regex(@"^\d\s"),        //"2 "
            new Regex(@"^\w\s"),        //"c "
            new Regex(@"^\w\.\s"),      //"c. "
            new Regex(@"^-"),           //"-"
            new Regex(@"^~")            //"~"
        };
    }
}
