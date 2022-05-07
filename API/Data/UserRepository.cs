using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using API.Data;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using API.DTOs;
using API.Helpers;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MemberDTO> GetMemberAsync(string username)
        {
            //return await _context.Users.Where(x => x.UserName == username)
            //    .Select(user => new MemberDTO
            //    {

            //    }).SingleOrDefaultAsync();

            //Use below ProjectTo from Automapper to do as above

            return await _context.Users.Where(x => x.UserName == username).ProjectTo<MemberDTO>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();

            query = query.Where(u => u.UserName != userParams.CurrentUsername);
            query = query.Where(u => u.Gender == userParams.Gender);

            var mindob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxdob = DateTime.Today.AddYears(-userParams.MinAge);
            query = query.Where(u => u.DateOfBirth >= mindob && u.DateOfBirth <= maxdob);

            return await PagedList<MemberDTO>
                .CreateAsync(query.ProjectTo<MemberDTO>(_mapper.ConfigurationProvider).AsNoTracking()
                , userParams.PageNumber, userParams.PageSize);
        }
        
        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            return await _context.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users.Include(p => p.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}