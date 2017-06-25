using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace IdentityCoreProject.Models
{
    public class WebNote
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Color { get; set; }
        public int OrderIndex { get; set; }
        public bool hasFile { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public int FileId { get; set; }
        [ForeignKey("FileId")]
        public FileAttachment FileAttachment { get; set; }
    }
}
