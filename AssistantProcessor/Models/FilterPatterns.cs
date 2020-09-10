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

        [JsonIgnore] public List<Regex> taskRegexes = new List<Regex>();
        [JsonIgnore] public List<Regex> correctAnswerRegexes = new List<Regex>();
        [JsonIgnore] public List<Regex> wrongAnswerRegexes = new List<Regex>();

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

        public static Regex taskAnswerPattern1 = new Regex(@"^\d+\.");       //"13."
        public static Regex taskAnswerPattern2 = new Regex(@"^\d+\.d+\.");   //"21.23. "
        public static Regex taskAnswerPattern4 = new Regex(@"^\d+\s");       //"13 "
        public static Regex taskAnswerPattern5 = new Regex(@"^\d+\.\d+\s");  //"13.13 "

        public static Regex trueAnswerPattern1 = new Regex(@"^[A|А]\.");    //"A."
        public static Regex trueAnswerPattern2 = new Regex(@"^1\.");        //"1."
        public static Regex trueAnswerPattern3 = new Regex(@"^[a|а]\.");    //"a."
        public static Regex trueAnswerPattern4 = new Regex(@"^[A|А]\s");    //"A "
        public static Regex trueAnswerPattern5 = new Regex(@"^1\s");        //"1 "
        public static Regex trueAnswerPattern6 = new Regex(@"^[a|а]\s");    //"a "
        public static Regex trueAnswerPattern70 = new Regex(@"^\d\*");      //"2*"
        public static Regex trueAnswerPattern71 = new Regex(@"^\d\.\*");    //"2.*"
        public static Regex trueAnswerPattern72 = new Regex(@"^\d\s\*");    //"2 *"
        public static Regex trueAnswerPattern73 = new Regex(@"^\d\.\s\*");  //"2. *"
        public static Regex trueAnswerPattern80 = new Regex(@"^\w\*");      //"c*"
        public static Regex trueAnswerPattern81 = new Regex(@"^\w\.\*");    //"c.*"
        public static Regex trueAnswerPattern82 = new Regex(@"^\w\s\*");    //"c *"
        public static Regex trueAnswerPattern83 = new Regex(@"^\w\.\s\*");  //"c. *"
        public static Regex trueAnswerPattern76 = new Regex(@"^\d\+");      //"2+"
        public static Regex trueAnswerPattern77 = new Regex(@"^\d\.\+");    //"2.+"
        public static Regex trueAnswerPattern78 = new Regex(@"^\d\s\+");    //"2 +"
        public static Regex trueAnswerPattern79 = new Regex(@"^\d\.\s\+");  //"2. +"
        public static Regex trueAnswerPattern86 = new Regex(@"^\w\+");      //"c+"
        public static Regex trueAnswerPattern87 = new Regex(@"^\w\.\+");    //"c.+"
        public static Regex trueAnswerPattern88 = new Regex(@"^\w\s\+");    //"c +"
        public static Regex trueAnswerPattern89 = new Regex(@"^\w\.\s\+");  //"c. +"
        public static Regex trueAnswerPattern9 = new Regex(@"^\*");         //"*text" & "* text"
        public static Regex trueAnswerPattern91 = new Regex(@"^\+");        //"+text" & "+ text"

        public static Regex falseAnswerPattern70 = new Regex(@"^\d");      //"2"
        public static Regex falseAnswerPattern71 = new Regex(@"^\d\.");    //"2."
        public static Regex falseAnswerPattern72 = new Regex(@"^\d\s");    //"2 "
        public static Regex falseAnswerPattern73 = new Regex(@"^\d\.\s");  //"2. "
        public static Regex falseAnswerPattern80 = new Regex(@"^\w\s");    //"c "
        public static Regex falseAnswerPattern81 = new Regex(@"^\w\.\s");  //"c. "
        public static Regex falseAnswerPattern82 = new Regex(@"^-");       //"-"
        public static Regex falseAnswerPattern83 = new Regex(@"^~");       //"~"
    }
}
