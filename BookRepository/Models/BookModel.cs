using System.ComponentModel.DataAnnotations;

namespace BookRepository.Models
{
    public class BookModel
    {
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public string Publisher { get; set; }
        public string Author { get; set; }
    }
}
