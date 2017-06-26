using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IdentityCoreProject.Models;
using IdentityCoreProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using IdentityCoreProject.Services;
using System.IO;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace IdentityCoreProject.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebNoteService _webNoteService;

        public HomeController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebNoteService webNoteService)
        {
            _context = context;
            _userManager = userManager;
            _webNoteService = webNoteService;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var sortingOption = _webNoteService.GetUsersSortingOption(userId);
            var notes = _webNoteService.GetUsersNotes(userId, sortingOption);
                return View(notes);
        }

        [HttpGet("getWebNotes")]
        public IActionResult GetWebNotes()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var sortingOption = _webNoteService.GetUsersSortingOption(userId);
            var webnotes = _webNoteService.GetUsersNotes(userId, sortingOption);

            if(sortingOption == "byDate" || sortingOption == null)
            {
                for (int i = 0; i < webnotes.Count(); i++)
                {
                    var toUpdate = _context.WebNotes
                        .Where(x => x.UserId == userId)
                        .FirstOrDefault(x => x.Id == webnotes[i].Id);
                    toUpdate.OrderIndex = i;
                    _context.Update(toUpdate);
                    webnotes[i].OrderIndex = i;
                    //webnotes[i].OrderIndex = i;
                }
                _context.SaveChanges();
            }
            

            return Json(new { notes = webnotes });
        }

        [HttpPost("updateNoteTitle")]
        public IActionResult UpdateNoteTitle(int id, string title)
        {
            _webNoteService.UpdateTitle(id, title);
            return Ok();
        }

        [HttpPost("updateNoteContent")]
        public IActionResult UpdateNoteContent(int id, string content)
        {
            _webNoteService.UpdateContent(id, content);
            return Ok();
        }

        [HttpPost("deleteNote")]
        public IActionResult DeleteNote(int id)
        {
            _webNoteService.DeleteNote(id);
            return Ok();
        }

        [HttpPost("saveNote")]
        public async Task<IActionResult> Save(WebNote webNote)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            _webNoteService.CreateNote(webNote, user);
            return Ok();
        }

        [HttpPost("moveNoteUp")]
        public IActionResult MoveNoteUp(int idOfClickedNote, string userId)
        {

            _webNoteService.MoveNoteUp(idOfClickedNote, userId);
            return Ok();
        }

        [HttpPost("moveNoteDown")]
        public IActionResult MoveNoteDown(int idOfClickedNote, string userId)
        {
            _webNoteService.MoveNoteDown(idOfClickedNote, userId);
            return Ok();
        }

        [HttpPost("sendEmail")]
        public IActionResult SendEmail(string email, WebNote note)
        {
            string loggedInEmail = User.Identity.Name;
            _webNoteService.SendEmail(note, loggedInEmail, email); 
            return Ok();
        }

        [HttpGet("getSingleWebNote")]
        public IActionResult getSingleWebNote(int id)
        {
            WebNote webnote = _context.WebNotes.FirstOrDefault(x => x.Id == id);
            return Json(webnote);
        }

        [HttpPost("groupByPriority")]
        public IActionResult groupByPriority()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var toUpdate = _context.Users.FirstOrDefault(x => x.Id == userId);
            var currentSortingOption = _webNoteService.GetUsersSortingOption(userId);

            if (currentSortingOption == "byPriority")
            {
                toUpdate.WebnoteSortingOption = "byDate";
                _context.Update(toUpdate);
                _context.SaveChanges();
                return Ok();
            }
            else
            {
                toUpdate.WebnoteSortingOption = "byPriority";
                _context.Update(toUpdate);
                _context.SaveChanges();
                return Ok();
            }
        }

        [HttpGet("getSortingOption")]
        public string GetSortingOption()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var sortingOption = _webNoteService.GetUsersSortingOption(userId);
            return sortingOption;
        }

        [HttpPost("getNoteToAttachFileTo")]
        public WebNote GetNoteToAttachFileTo(int id)
        {
            var webNote = _context.WebNotes.FirstOrDefault(x => x.Id == id);
            return webNote;
        }

        [HttpGet]
        public FileResult DownloadNotes()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var sortingOption = _webNoteService.GetUsersSortingOption(userId);
            var myWebNotes = _webNoteService.GetUsersNotes(userId, sortingOption);
            var stream = _webNoteService.DownloadNotes(userId, myWebNotes);

            return File(stream, "text/csv", "WebNotes(" + User.Identity.Name + ").csv");
        }

        [HttpPost("uploadFileAttachment")]
        public async Task<IActionResult> SaveFile(IFormFile file)
        {
            //Set The connection string
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("storageConnectionString"));
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=webnotestorage;AccountKey=I8nWrPfr9NYyUQPGNVm0WROiUHuOkE88bZMrwywfXTJNls1SXXitmqwVEIwQGkuAkc9kL7kOdgn6XHz76Xl9qA==;EndpointSuffix=core.windows.net");

            //Create a blob client
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            //Get a reference to a container
            CloudBlobContainer container = blobClient.GetContainerReference("files");

            //Get a reference to a blob
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(file.Name);

            //Create or overwrite the blob with the contents of a local files
            using (var fileStream = file.OpenReadStream())
            {
            await blockBlob.UploadFromStreamAsync(fileStream);
            }

            var newFileAttachment = new FileAttachment();
            newFileAttachment.Name = blockBlob.Name;
            newFileAttachment.Size = blockBlob.Properties.Length;

            var note = GetNoteToAttachFileTo();
            _webNoteService.AddFileToNote();

            return RedirectToAction("Index");
            }
    }
}
