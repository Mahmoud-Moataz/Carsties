using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/auctions")]
    public class AuctionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly AuctionDbContext _context;
        public AuctionsController(AuctionDbContext Context, IMapper mapper)
        {
            _context = Context;
            _mapper = mapper;
                
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
        {
            var auctions =  await _context.Auctions
                .Include(a=>a.Item)
                .OrderBy(a=>a.Item.Make)
                .ToListAsync();

            return _mapper.Map<List<AuctionDto>>(auctions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
        {
            var auction = await _context.Auctions
                .Include(a => a.Item)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (auction == null)
            {
                return NotFound();
            }

            return _mapper.Map<AuctionDto>(auction);
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
        {
            var auction = _mapper.Map<Auction>(createAuctionDto);
            //TODO: Add cuurent user as seller
            auction.Seller = "test";
             _context.Auctions.Add(auction);
            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Couldn't save changes to the DB");

            return 
            CreatedAtAction(nameof(GetAuctionById), new {id = auction.Id}
            ,_mapper.Map<AuctionDto>(auction));
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
        {
            var auction = await _context.Auctions.Include(a => a.Item)
                            .FirstOrDefaultAsync(a => a.Id == id);
            if (auction == null) return NotFound();

            //TODO: check seller == username

            auction.Item.Make = updateAuctionDto.Make?? auction.Item.Make;
            auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
            auction.Item.Color = updateAuctionDto.Color??  auction.Item.Color;
            auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

            bool result = await _context.SaveChangesAsync() > 0;
            if (!result) return BadRequest("Problem, Data not be updated");

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null) return NotFound();

            //TODO: check seller == username
            _context.Auctions.Remove(auction);
            bool result = await _context.SaveChangesAsync() > 0;
            if (!result) return BadRequest("Problem, Data not be removed");

            return Ok();
        }
    }
}
