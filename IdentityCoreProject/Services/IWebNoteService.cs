using System.Collections.Generic;
using IdentityCoreProject.Models;
using System.IO;
using System.Threading.Tasks;

namespace IdentityCoreProject.Services
{
    public interface IWebNoteService
    {
        void CreateNote(WebNote webNote, ApplicationUser user);
        Task DeleteNote(int id);
        Task DeleteFile(int fileId);
        List<WebNote> GetUsersNotes(string userId, string sortingOption);
        void MoveNoteDown(int idOfClickedNote, string userId);
        void MoveNoteUp(int idOfClickedNote, string userId);
        void UpdateContent(int id, string content);
        void UpdateTitle(int id, string title);
        MemoryStream DownloadNotes(string userId, List<WebNote> notes);
        void SendEmail(WebNote note, string loggedInEmail, string email);
        string GetUsersSortingOption(string userId);
        void AddFileToNote(FileAttachment file, WebNote noteToAttachTo, ApplicationUser user);
    }
}