using System.ComponentModel.DataAnnotations.Schema;


namespace IdentityCoreProject.Models
{
    public class FileAttachment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public string URI { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

    }
}
