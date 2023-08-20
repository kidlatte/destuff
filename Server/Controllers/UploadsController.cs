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
    private IIdentityHasher<Stuff> StuffId { get; }
    private IIdentityHasher<Location> LocationId { get; }
    private IFileService Files { get; }

    public UploadsController(ApplicationDbContext context, IMapper mapper, IIdentityHasher<Upload> hasher,
        IIdentityHasher<Stuff> stuffId, IIdentityHasher<Location> locationId, IFileService files) : base(context, mapper, hasher)
    {
        StuffId = stuffId;
        LocationId = locationId;
        Files = files;
    }

    [AllowAnonymous]
    [HttpGet(ApiRoutes.UploadFiles + "/{hash}/{name}")]
    public async Task<IActionResult> Get(string hash, string name)
    {
        var id = Hasher.Decode(hash);
        var query = Query.Where(x => x.Id == id);

        var entity = await query.FirstOrDefaultAsync();
        if (entity == null || entity.Path == null || entity.FileName != name || !System.IO.File.Exists(entity.Path))
            return NotFound();

        var file = System.IO.File.OpenRead(entity.Path);
        string contentType = Files.GetContentType(name);
        return File(file, contentType);
    }

    [HttpPost]
    public async Task<ActionResult<UploadModel>> Post([FromForm] UploadCreateModel model)
    {
        if (model.File == null || model.File.Length == 0)
            return BadRequest();

        try
        {
            var filePath = await Files.Save(model.File);

            var locationId = LocationId.Decode(model.LocationId);
            var stuffId = StuffId.Decode(model.StuffId);

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

    [HttpPost(ApiRoutes.UploadImage)]
    public async Task<ActionResult<UploadModel>> PostImage([FromForm] UploadCreateModel model)
    {
        if (model.File == null || model.File.Length == 0)
            return BadRequest();

        try
        {
            var filePath = await Files.SaveImage(model.File);

            var locationId = LocationId.Decode(model.LocationId);
            var stuffId = StuffId.Decode(model.StuffId);

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

    internal override Task BeforeDeleteAsync(Upload entity)
    {
        if (entity.Path != null)
            Files.Delete(entity.Path);

        return Task.CompletedTask;
    }
}