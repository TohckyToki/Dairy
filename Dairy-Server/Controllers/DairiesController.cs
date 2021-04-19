using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Dairy_Server.Entities;
using Dairy_Server.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dairy_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DairiesController : ControllerBase
    {
        private readonly MyDairyContext DairyContext;

        public DairiesController(MyDairyContext context)
        {
            this.DairyContext = context;
        }

        [HttpGet("getDairyList")]
        public async Task<List<DairyModel>> GetDairyListAsync([FromQuery] int page, [FromQuery] int pageSize)
        {
            return await this.DairyContext.Dairies
                            .Where(e => e.Enabled)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .Select(e => new DairyModel
                            {
                                Uid = e.Uid,
                                WroteDate = e.WroteDate,
                                Thema = e.Thema,
                                Wheather = e.Wheather,
                                Emotions = e.Emotions,
                                Content = e.Content,
                            })
                            .ToListAsync();
        }

        [HttpGet("getDairyInfo/{id}")]
        public async Task<DairyModel> GetDairyInfoAsync(int id)
        {
            var row = await this.DairyContext.Dairies.FindAsync(id);

            var query = this.DairyContext.Dairies;
            var previous = (await query.Where(e => e.WroteDate < row.WroteDate).CountAsync()) > 0;
            var next = (await query.Where(e => e.WroteDate > row.WroteDate).CountAsync()) > 0;

            return new DairyModel
            {
                Uid = row.Uid,
                WroteDate = row.WroteDate,
                Thema = row.Thema,
                Wheather = row.Wheather,
                Emotions = row.Emotions,
                Content = row.Content,
                HasPrevious = previous,
                HasNext = next,
            };
        }

        [HttpGet("getPreviousDairy")]
        public async Task<DairyModel> GetPreviousDairyAsync([FromQuery] DateTime date)
        {
            var row = await this.DairyContext.Dairies.Where(e => e.WroteDate < date).OrderByDescending(e => e.WroteDate).FirstOrDefaultAsync();

            var query = this.DairyContext.Dairies;
            var previous = (await query.Where(e => e.WroteDate < row.WroteDate).CountAsync()) > 0;
            var next = (await query.Where(e => e.WroteDate > row.WroteDate).CountAsync()) > 0;

            return new DairyModel
            {
                Uid = row.Uid,
                WroteDate = row.WroteDate,
                Thema = row.Thema,
                Wheather = row.Wheather,
                Emotions = row.Emotions,
                Content = row.Content,
                HasPrevious = previous,
                HasNext = next,
            };
        }

        [HttpGet("getNextDairy")]
        public async Task<DairyModel> GetNextDairyAsync([FromQuery] DateTime date)
        {
            var row = await this.DairyContext.Dairies.Where(e => e.WroteDate > date).OrderBy(e => e.WroteDate).FirstOrDefaultAsync();

            var query = this.DairyContext.Dairies;
            var previous = (await query.Where(e => e.WroteDate < row.WroteDate).CountAsync()) > 0;
            var next = (await query.Where(e => e.WroteDate > row.WroteDate).CountAsync()) > 0;

            return new DairyModel
            {
                Uid = row.Uid,
                WroteDate = row.WroteDate,
                Thema = row.Thema,
                Wheather = row.Wheather,
                Emotions = row.Emotions,
                Content = row.Content,
                HasPrevious = previous,
                HasNext = next,
            };
        }

        [HttpGet("getDairiesCount")]
        public async Task<int> GetDairiesCountAsync()
        {
            return await this.DairyContext.Dairies.Where(e => e.Enabled).CountAsync();
        }

        [HttpPost("addDairy")]
        public async Task<string> AddDairyAsync([FromBody] DairyModel dairy)
        {
            using var transaction = await this.DairyContext.Database.BeginTransactionAsync();
            try
            {
                await this.DairyContext.Dairies.AddAsync(dairy);
                await this.DairyContext.SaveChangesAsync();
                await this.DairyContext.Database.CommitTransactionAsync();
                return JsonSerializer.Serialize(true);
            }
            catch (System.Exception)
            {
                await transaction.RollbackAsync();
                return JsonSerializer.Serialize(false);
            }
        }

        [HttpPut("updateDairy")]
        public async Task<string> UpdateDairyAsync([FromBody] DairyModel dairy)
        {
            using var transaction = await this.DairyContext.Database.BeginTransactionAsync();
            try
            {
                var d = await this.DairyContext.Dairies.FindAsync(dairy.Uid);

                d.Thema = dairy.Thema;
                d.Wheather = dairy.Wheather;
                d.Emotions = dairy.Emotions;
                d.Content = dairy.Content;

                await this.DairyContext.SaveChangesAsync();
                await this.DairyContext.Database.CommitTransactionAsync();
                return JsonSerializer.Serialize(true);
            }
            catch (System.Exception)
            {
                await transaction.RollbackAsync();
                return JsonSerializer.Serialize(false);
            }
        }

        [HttpDelete("deleteDairy/{id}")]
        public async Task<string> DeleteDairyAsync(int id)
        {
            using var transaction = await this.DairyContext.Database.BeginTransactionAsync();
            try
            {
                var d = await this.DairyContext.Dairies.FindAsync(id);
                d.Enabled = false;
                await this.DairyContext.SaveChangesAsync();
                await this.DairyContext.Database.CommitTransactionAsync();
                return JsonSerializer.Serialize(true);
            }
            catch (System.Exception)
            {
                await transaction.RollbackAsync();
                return JsonSerializer.Serialize(false);
            }
        }
    }
}
