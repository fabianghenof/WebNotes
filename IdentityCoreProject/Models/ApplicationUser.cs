using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IdentityCoreProject.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            WebNotes = new List<WebNote>();
            FileAttachments = new List<FileAttachment>();
        }

        public string WebnoteSortingOption { get; set; }
        public virtual List<WebNote> WebNotes { get; set; }
        public virtual List<FileAttachment> FileAttachments { get; set; }
    }
}
