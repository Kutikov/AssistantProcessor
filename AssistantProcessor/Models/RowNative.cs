namespace AssistantProcessor.Models
{
    public class RowNative
    {
        public string content;
        public bool included;
        public int rowNumber;

        public RowNative(string content, int rowNumber)
        {
            this.rowNumber = rowNumber;
            this.content = content;
            this.included = true;
        }
    }
}
