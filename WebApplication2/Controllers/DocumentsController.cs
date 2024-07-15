using Microsoft.AspNetCore.Mvc;
using System.IO;
using Furion.DynamicApiController;
using Furion.FriendlyException;
using Microsoft.AspNetCore.Http;
using WebApplication2.Service;

namespace WebApplication2.Controllers;

[ApiDescriptionSettings(Name = "MyFur")]
public class DocumentsController:IDynamicApiController
{
    private readonly GitService _gitService;

    public DocumentsController(GitService gitService)
    {
        _gitService = gitService;
    }

    [HttpPost("upload")]
    public Task Upload( IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
           throw Oops.Oh("File is empty");
        }

        var filePath = Path.Combine(_gitService.RepositoryPath, file.FileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            file.CopyTo(stream);
        }

        _gitService.AddAndCommit(filePath, "Uploaded " + file.FileName);
        return Task.CompletedTask;
    }
    
    
        [HttpPost("update")]
        public async Task Update( IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw Oops.Oh("File is empty");
            }

            var filePath = Path.Combine(_gitService.RepositoryPath, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _gitService.UpdateAndCommit(filePath, "Updated " + file.FileName);

      
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <example>新建文本文档 (4).txt</example>
        [HttpGet("history")]
        public List<object> GetHistory([FromQuery] string fileName)
        {
            var commits = _gitService.GetCommitHistory(fileName);
            var history = new List<object>();

            foreach (var commit in commits)
            {
                history.Add(new
                {
                    commit.Message,
                    commit.Author.Name,
                    commit.Author,
                    commit.Sha
                });
            }

            return history;
        }
        
        [HttpGet("download")]
        public IActionResult Download([FromQuery] string fileName, [FromQuery] string commitSha)
        {
            var filePath = Path.Combine(_gitService.RepositoryPath, fileName);
            var fileContent = _gitService.GetFileContentByVersion(filePath, commitSha);

            if (fileContent == null)
            {
               throw Oops.Oh("File or version not found");
            }

            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
            return new FileStreamResult(stream, "application/octet-stream")
            {
                FileDownloadName = fileName
            };
        }
}