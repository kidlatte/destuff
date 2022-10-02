using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Destuff.Server.Data;
using Destuff.Server.Data.Entities;
using Destuff.Server.Models;
using Destuff.Server.Services;
using Destuff.Shared;
using Destuff.Shared.Models;


namespace Destuff.Server.Controllers;

[Route(ApiRoutes.Uploads)]
[ApiController, Authorize]
public class UploadsController : BaseController<Upload>
{
    private IConfiguration Configuration { get; }
    private IStuffIdentifier StuffId { get; }
    private ILocationIdentifier LocationId { get; }
    private IUploadIdentifier UploadId { get; }

    public UploadsController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration,
        IStuffIdentifier stuffId, ILocationIdentifier locationId, IUploadIdentifier uploadId) : base(context, mapper)
    {
        Configuration = configuration;
        StuffId = stuffId;
        LocationId = locationId;
        UploadId = uploadId;
    }

    [AllowAnonymous]
    [HttpGet(ApiRoutes.UploadFiles + "/{id}/{name}")]
    public async Task<IActionResult> Get(string id, string name, [FromServices] FileExtensionContentTypeProvider provider)
    {
        int actualId = UploadId.Decode(id);
        var query = Query.Where(x => x.Id == actualId);

        var entity = await query.FirstOrDefaultAsync();
        if (entity == null || entity.Path == null || entity.FileName != name)
            return NotFound();

        string contentType = provider.TryGetContentType(name, out string? value) ? value : "application/octet-stream";

        var file = System.IO.File.OpenRead(entity.Path);
        return File(file, contentType);
    }

    [HttpPost]
    public async Task<ActionResult<UploadModel>> Create([FromForm] UploadCreateModel model)
    {
        var file = model.File;
        if (file == null || file.Length == 0)
            return BadRequest();

        try
        {
            var path = Path.Combine(Configuration.GetDataPath(), "uploads", DateTime.UtcNow.ToString("yyyyMMdd"));
            Directory.CreateDirectory(path);

            var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}-{Guid.NewGuid().ToString().Substring(0, 5)}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(path, fileName);


            await using FileStream fs = new(filePath, FileMode.Create);
            await file.CopyToAsync(fs);

            var locationId = model.LocationId != null ? LocationId.Decode(model.LocationId) : default(int?);
            var stuffId = model.StuffId != null ? StuffId.Decode(model.StuffId) : default(int?);

            var entity = new Upload 
            { 
                FileName = file.FileName, 
                Path = filePath,
                LocationId = locationId,
                StuffId = stuffId,
            };
            Audit(entity);
            
            Context.Add(entity);
            await Context.SaveChangesAsync();

            return Mapper.Map<UploadModel>(entity);
        }
        catch (IOException)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

}