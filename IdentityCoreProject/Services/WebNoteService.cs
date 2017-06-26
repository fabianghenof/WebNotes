using AutoMapper;
using CsvHelper;
using IdentityCoreProject.Data;
using IdentityCoreProject.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using RazorLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;


namespace IdentityCoreProject.Services
{
    public class WebNoteService : IWebNoteService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IHostingEnvironment _hostingEnvironment;

        public WebNoteService
            (ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IHostingEnvironment hostingEnvironment)

        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
        }

        public List<WebNote> GetUsersNotes(string userId, string sortingOption)
        {
            if (sortingOption == "byPriority")
            {
                var notesByPriority = userId == null ? new List<WebNote>() : _context.WebNotes
                .Where(n => n.UserId == userId)
                .OrderBy(x => x.Color)
                .ToList();
                return notesByPriority;
            }
            else
            {
                var notesByDate = userId == null ? new List<WebNote>() : _context.WebNotes
                .Where(n => n.UserId == userId)
                .OrderBy(x => x.OrderIndex)
                .ToList();
                return notesByDate;
            }
        }

        public void UpdateTitle(int id, string title)
        {
            var toUpdate = _context.WebNotes.FirstOrDefault(x => x.Id == id);
            toUpdate.Title = title;
            _context.Update(toUpdate);
            _context.SaveChanges();
        }

        public void UpdateContent(int id, string content)
        {
            var toUpdate = _context.WebNotes.FirstOrDefault(x => x.Id == id);
            toUpdate.Content = content;
            _context.Update(toUpdate);
            _context.SaveChanges();
        }

        public void DeleteNote(int id)
        {
            var noteToDelete = _context.WebNotes.SingleOrDefault(x => x.Id == id);
            var fileToDelete = _context.FileAttachments.SingleOrDefault(x => x.Id == noteToDelete.FileId);
            _context.WebNotes.Remove(noteToDelete);
            if (fileToDelete != null) _context.FileAttachments.Remove(fileToDelete);
            _context.SaveChanges();
        }

        public void CreateNote(WebNote webNote, ApplicationUser user)
        {
            int nbrOfNotes = _context.WebNotes.Count();
            //query list here .ToList apoi foloseste asta in forloop
            for (int i = 0; i < nbrOfNotes; i++)
            {
                var noteToModify = _context.WebNotes.FirstOrDefault(x => x.OrderIndex == i);
                if (noteToModify != null)
                {
                    noteToModify.OrderIndex++;
                    _context.WebNotes.Update(noteToModify);
                }
            }

            var file = new FileAttachment();

            webNote.User = user;
            webNote.FileAttachment = file;
            webNote.hasFile = false;

            var toUpdate = _context.Users.FirstOrDefault(x => x.Id == user.Id);
            toUpdate.WebNotes.Add(webNote);
            toUpdate.FileAttachments.Add(file);
            _context.Update(toUpdate);
            _context.Add(webNote);
            _context.SaveChanges();

        }

        public void MoveNoteUp(int idOfClickedNote, string userId)
        {
            var userSortingOption = GetUsersSortingOption(userId);
            if (userSortingOption == "byDate" || userSortingOption == null)
            {
                var noteClickedOn = _context.WebNotes
                .Where(x => x.UserId == userId)
                .FirstOrDefault(x => x.Id == idOfClickedNote);

                var noteAbove = _context.WebNotes.FirstOrDefault(x => x.OrderIndex == noteClickedOn.OrderIndex - 1);
                if (noteAbove != null)
                {
                    int orderIndexToMoveTo = noteAbove.OrderIndex;
                    int temp = noteClickedOn.OrderIndex;

                    noteClickedOn.OrderIndex = orderIndexToMoveTo;
                    noteAbove.OrderIndex = temp;
                    _context.Update(noteClickedOn);
                    _context.Update(noteAbove);
                    _context.SaveChanges();
                }
                else { return; }
            }
            else { return; }
        }

        public void MoveNoteDown(int idOfClickedNote, string userId)
        {
            var userSortingOption = GetUsersSortingOption(userId);
            if (userSortingOption == "byDate" || userSortingOption == null)
            {
                var noteClickedOn = _context.WebNotes
                    .Where(x => x.UserId == userId)
                    .FirstOrDefault(x => x.Id == idOfClickedNote);

                var noteBelow = _context.WebNotes.FirstOrDefault(x => x.OrderIndex == noteClickedOn.OrderIndex + 1);
                if (noteBelow != null)
                {
                    int orderIndexToMoveTo = noteBelow.OrderIndex;
                    int temp = noteClickedOn.OrderIndex;

                    noteClickedOn.OrderIndex = noteBelow.OrderIndex;
                    noteBelow.OrderIndex = temp;
                    _context.Update(noteClickedOn);
                    _context.Update(noteBelow);
                    _context.SaveChanges();
                }
                else { return; }
            }

            else { return;  }
        }

        public MemoryStream DownloadNotes(string userId, List<WebNote> myWebNotes)
        {
            var trimmedWebNotes = new List<CsvNote>();

            for (int i = 0; i < myWebNotes.Count(); i++)
            {
                var noteToAdd = _mapper.Map<CsvNote>(myWebNotes[i]);
                trimmedWebNotes.Add(noteToAdd);
            }

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            var csv = new CsvWriter(writer);
            csv.WriteRecords(trimmedWebNotes);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }

        void IWebNoteService.SendEmail(WebNote note, string loggedInEmail, string email)
        {
            var engine = EngineFactory.CreatePhysical(Path.Combine(_hostingEnvironment.ContentRootPath, "Templates", "Email"));
            var model = new
            {
                Sender = loggedInEmail,
                Title = note.Title,
                Color = note.Color,
                Content = note.Content,
            };
            string result = engine.Parse("basic.cshtml", model);


            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("WebNotes", "fabian.ghenof@xconta.ro"));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = "WebNote from " + loggedInEmail;
            message.Body = new TextPart("html")
            {
                Text = result
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.mailgun.org", 587, false);
                // Note: since we don't have an OAuth2 token, disable // the XOAUTH2 authentication mechanism
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(
                    Environment.GetEnvironmentVariable("emailClientUsername"),
                    Environment.GetEnvironmentVariable("emailClientPassword"));
                client.Send(message);
                client.Disconnect(true);
            }
            //
        }

        public string GetUsersSortingOption(string userId)
        {
            var sortingOption = _context.Users
                .Where(x => x.Id == userId)
                .Select(x => x.WebnoteSortingOption)
                .FirstOrDefault();

                return sortingOption;
            
        }

        public void AddFileToNote(WebNote noteToAttachTo, byte[] fileData, string fileType, ApplicationUser user)
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(
             new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                "<storage-account-name>",
                "<access-key>"), true);

            //Create a file / Add it to DB
            var newFileAttachment = new FileAttachment();
            newFileAttachment.User = user;
            newFileAttachment.Name = "File Attachment";
            _context.FileAttachments.Add(newFileAttachment);
            //Add it to the user
            user.FileAttachments.Add(newFileAttachment);
            _context.Update(user);
            //Asign it to a webnote
            //var toUpdate = _context.WebNotes.FirstOrDefault(x => x.Id == noteToAttachTo.Id);
            noteToAttachTo.FileAttachment = newFileAttachment;
            noteToAttachTo.hasFile = true;
            _context.Update(noteToAttachTo);
            _context.SaveChanges();
        }
    }
}
