using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Interfaces;
using API.Data;
using API.Entities;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _Context;

        public LikesRepository(DataContext Context)
        {
            _Context = Context;
        }
        public Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LikeDTO>> GetUserLikes(string predicate, int userid)
        {
            throw new NotImplementedException();
        }

        public Task<AppUser> GetUserWithLike(int userId)
        {
            throw new NotImplementedException();
        }
    }
}