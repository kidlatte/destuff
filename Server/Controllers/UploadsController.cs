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
    [HttpGet(ApiRoutes.UploadFiles + "/{id}/{name}")]
    public async Task<IActionResult> Get(string id, string name)
    {
        int actualId = Hasher.Decode(id);
        var query = Query.Where(x => x.Id == actualId);

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

    [HttpPost(ApiRoutes.UploadImage)]
    public async Task<ActionResult<UploadModel>> PostImage([FromForm] UploadCreateModel model)
    {
        if (model.File == null || model.File.Length == 0)
            return BadRequest();

        try
        {
            var filePath = await Files.SaveImage(model.File);

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

    [HttpDelete("{hash}")]
    public override async Task<IActionResult> Delete(string hash, [FromServices] IIdentityHasher<Upload> hasher)
    {
        int id = hasher.Decode(hash);
        var entity = await Query.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (entity == null)
            return NotFound();

        if (entity.Path != null)
            Files.Delete(entity.Path);

        Context.Remove(entity);
        await Context.SaveChangesAsync();

        return NoContent();
    }    
}