namespace BenchRando.IC
{
    // Note: currently unused, BenchContainer support has not been tested
    public class BenchStyleTag : Tag
    {
        public string NearStyle { get; set; }
        public string FarStyle { get; set; }
        public void SetStyle(string style)
        {
            NearStyle = FarStyle = style;
        }
    }
}
