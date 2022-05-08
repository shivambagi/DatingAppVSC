using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Interfaces;
using API.Data;
using API.Entities;
using API.Extensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _Context;

        public LikesRepository(DataContext Context)
        {
            _Context = Context;
        }
        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _Context.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<IEnumerable<LikeDTO>> GetUserLikes(string predicate, int userid)
        {
            var users = _Context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _Context.Likes.AsQueryable();

            if(predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == userid);
                users = likes.Select(like => like.LikedUser);
            }

            if (predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == userid);
                users = likes.Select(like => like.SourceUser);
            }

            return await users.Select(user => new LikeDTO
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            }).ToListAsync();
        }

        public async Task<AppUser> GetUserWithLike(int userId)
        {
            return await _Context.Users
                    .Include(x => x.LikedUsers)
                    .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}