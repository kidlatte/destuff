using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
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
    private IStuffIdentifier StuffId { get; }
    private ILocationIdentifier LocationId { get; }
    private IUploadIdentifier UploadId { get; }

    public UploadsController(ApplicationDbContext context, IMapper mapper,
        IStuffIdentifier stuffId, ILocationIdentifier locationId, IUploadIdentifier uploadId) : base(context, mapper)
    {
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
    public async Task<ActionResult<UploadModel>> Create([FromForm] UploadCreateModel model, [FromServices] IFileService file)
    {
        if (model.File == null || model.File.Length == 0)
            return BadRequest();

        try
        {
            var filePath = await file.Save(model.File);

            var locationId = model.LocationId != null ? LocationId.Decode(model.LocationId) : default(int?);
            var stuffId = model.StuffId != null ? StuffId.Decode(model.StuffId) : default(int?);

            var entity = new Upload 
            { 
                FileName = model.File.FileName, 
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