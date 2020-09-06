using System;
using System.Collections.Generic;
using System.Text;

namespace AssistantProcessor.Models
{
    public class AnalyseBlock
    {
        public List<RowAnalized> task;
        public List<RowAnalized> correctAnswers;
        public List<RowAnalized> wrongAnswers;
        public List<RowAnalized> comments;
        public List<RowAnalized> project;
        public int number;

    }
}
