using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IdentityCoreProject.Models;
using IdentityCoreProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using IdentityCoreProject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using OfficeOpenXml;
using System.IO;
using System;
using OfficeOpenXml.Style;
using System.Drawing;

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

            //Assuring order indexes are correct
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

        [HttpPost("uploadFileAttachment")]
        public async Task<IActionResult> SaveFile(IFormFile file, string fileId, int noteId)
        {
            //Set The connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("storageConnectionString"));
            //Create a blob client
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            //Get a reference to a container
            CloudBlobContainer container = blobClient.GetContainerReference("files");
            //Get a reference to a blob
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(file.FileName);
            //Create or overwrite the blob with the contents of a local files
            using (var fileStream = file.OpenReadStream())
            {
            await blockBlob.UploadFromStreamAsync(fileStream);
            }

            //Sending file to JS
            var newFileAttachment = new FileAttachment();
            newFileAttachment.Name = file.FileName;
            newFileAttachment.Size = blockBlob.Properties.Length;
            newFileAttachment.URI  = blockBlob.Uri.ToString();
            var noteToAttachTo = _context.WebNotes.FirstOrDefault(x => x.Id == noteId);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            _webNoteService.AddFileToNote(newFileAttachment, noteToAttachTo, user);

            return RedirectToAction("Index");
        }

        [HttpGet("getSingleFile")]
        public IActionResult GetSingleFile(int id)
        {
            var theFile = _context.FileAttachments.FirstOrDefault(x => x.Id == id);
            return Json(theFile);
        }

        [HttpPost("deleteFile")]
        public IActionResult DeleteFile(int fileId)
        {
            _webNoteService.DeleteFile(fileId);
            return Ok();
        }

        [HttpGet]
        public FileResult DownloadNotesCsv() {
              var userId = _userManager.GetUserId(HttpContext.User);
              var sortingOption = _webNoteService.GetUsersSortingOption(userId);
              var myWebNotes = _webNoteService.GetUsersNotes(userId, sortingOption);
              var stream = _webNoteService.DownloadNotes(userId, myWebNotes);
  
              return File(stream, "text/csv", "WebNotes("+ User.Identity.Name +").csv");
        }

        public FileResult DownloadNotesExcel()
        {
            ExcelPackage ExcelPkg = new ExcelPackage();
            ExcelWorksheet workSheet = ExcelPkg.Workbook.Worksheets.Add("Worksheet");
            int defaultFontSize = 12;
            Color headersColor = ColorTranslator.FromHtml("#9c9c9c");

            //INTRODUCTION WITH USER'S NAME
            using (ExcelRange Range = workSheet.Cells[1, 1, 1, 6])
            {
                Range.Value = "The WebNotes of '"+ User.Identity.Name +"':";
                Range.Merge = true;
                Range.Style.Font.Size = defaultFontSize;
            }

            //TABLE HEADERS
                ///Index Header
            using (ExcelRange Range = workSheet.Cells[3, 1, 3, 1])
            {
                Range.Value = "#";
                Range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                Range.Merge = true;
                Range.Style.Font.Size = defaultFontSize;
                Range.Style.Font.Color.SetColor(headersColor);
                Range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                Range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                Range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                Range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            }
                ///Priority Header
            using (ExcelRange Range = workSheet.Cells[3, 2, 3, 2])
            {
                Range.Value = "Priority";
                Range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                Range.Merge = true;
                Range.Style.Font.Size = defaultFontSize;
                Range.Style.Font.Color.SetColor(headersColor);
                Range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                Range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                Range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }
                ///Title Header
            using (ExcelRange Range = workSheet.Cells[3, 3, 3, 4])
            {
                Range.Value = "Title";
                Range.Merge = true;
                Range.Style.Font.Size = defaultFontSize;
                Range.Style.Font.Color.SetColor(headersColor);
                Range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                Range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                Range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }
                ///Content Header
            using (ExcelRange Range = workSheet.Cells[3, 5, 3, 12])
            {
                Range.Value = "Content";
                Range.Merge = true;
                Range.Style.Font.Size = defaultFontSize;
                Range.Style.Font.Color.SetColor(headersColor);
                Range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                Range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                Range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }

            //Getting the user's notes
            var userId = _userManager.GetUserId(HttpContext.User);
            var sortingOption = _webNoteService.GetUsersSortingOption(userId);
            var theNotes = _webNoteService.GetUsersNotes(userId, sortingOption);

            int currentRow = 4;
            //Writing notes to Excel
            for (int i = 0; i < theNotes.Count(); i++)
            {
                int rowToWriteAt = i + currentRow;
                int noteNumber = i + 1;
                ///Note Index
                using (ExcelRange Range = workSheet.Cells[rowToWriteAt, 1, rowToWriteAt, 1])
                {
                    Range.Value = noteNumber;
                    Range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    Range.Merge = true;
                    Range.Style.Font.Size = defaultFontSize;
                    Range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    Range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    Range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                }
                ///Note Priority
                using (ExcelRange Range = workSheet.Cells[rowToWriteAt, 2, rowToWriteAt, 2])
                {
                    Color priorityColor = ColorTranslator.FromHtml(theNotes[i].Color);
                    Range.Value = "O";
                    Range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    Range.Style.Font.Color.SetColor(priorityColor);
                    Range.Merge = true;
                    Range.Style.Font.Size = defaultFontSize;
                    Range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    Range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }
                ///Note Title
                using (ExcelRange Range = workSheet.Cells[rowToWriteAt, 3, rowToWriteAt, 4])
                {
                    Range.Value = theNotes[i].Title;
                    Range.Merge = true;
                    Range.Style.Font.Size = defaultFontSize;
                    Range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    Range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }
                ///Note Content
                using (ExcelRange Range = workSheet.Cells[rowToWriteAt, 5, rowToWriteAt, 12])
                {
                    Range.Value = theNotes[i].Content;
                    Range.Merge = true;
                    Range.Style.Font.Size = defaultFontSize;
                    Range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    Range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }
            }

            //Finishing and creating the Excel file
            workSheet.Protection.IsProtected = false;
            workSheet.Protection.AllowSelectLockedCells = false;

            var fileStream = new MemoryStream();
            ExcelPkg.SaveAs(fileStream);
            fileStream.Position = 0;
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var theExcelFile = new FileStreamResult(fileStream, contentType);
            theExcelFile.FileDownloadName = "WebNotes" + "(" + User.Identity.Name + ")" + ".xlsx";

            return theExcelFile;
        }
}
}
