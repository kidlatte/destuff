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
    private IIdentityHasher<Stuff> StuffHasher { get; }
    private IIdentityHasher<Location> LocationHasher { get; }
    public IIdentityHasher<Purchase> PurchaseHasher { get; }
    private IFileService Files { get; }

    public UploadsController(ApplicationDbContext context, IMapper mapper, IIdentityHasher<Upload> hasher,
        IIdentityHasher<Stuff> stuffHasher, IIdentityHasher<Location> locationHasher, 
        IIdentityHasher<Purchase> purchaseHasher, IFileService files) : base(context, mapper, hasher)
    {
        StuffHasher = stuffHasher;
        LocationHasher = locationHasher;
        PurchaseHasher = purchaseHasher;
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

            var stuffId = StuffHasher.Decode(model.StuffId);
            var locationId = LocationHasher.Decode(model.LocationId);
            var purchaseId = PurchaseHasher.Decode(model.PurchaseId);

            var entity = new Upload 
            { 
                FileName = model.File.FileName, 
                Path = filePath,
                LocationId = locationId,
                StuffId = stuffId,
                PurchaseId = purchaseId
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

            var stuffId = StuffHasher.Decode(model.StuffId);
            var locationId = LocationHasher.Decode(model.LocationId);
            var purchaseId = PurchaseHasher.Decode(model.PurchaseId);

            var entity = new Upload 
            { 
                FileName = model.File.FileName, 
                Path = filePath,
                StuffId = stuffId,
                LocationId = locationId,
                PurchaseId = purchaseId
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