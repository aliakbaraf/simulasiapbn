/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
namespace SimulasiAPBN.Web.Models
{
    public class Alert
    {
        public Alert() {}
        
        public Alert(string type, string title, string text)
        {
            Type = type;
            Title = title;
            Text = text;
        }
        
        public string Text { get; set; }
        
        public string Title { get; set; }
        
        public string Type { get; set; }
    }
}