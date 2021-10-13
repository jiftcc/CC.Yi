﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yi.Framework.Interface;
using Yi.Framework.Model;
using Yi.Framework.Model.Models;

namespace Yi.Framework.Service
{
   public class UserService: BaseService<user>,IUserService
    {
        public UserService(DbContext Db) :base(Db)
        {
        }
        public async Task<bool> DelListByUpdateAsync(List<int> _ids)
        {
            var userList = await GetEntitiesAsync(u => _ids.Contains(u.id));
            userList.ToList().ForEach(u => u.is_delete = (short)Common.Enum.DelFlagEnum.Deleted);
            return await UpdateListAsync(userList);
        }

        public async Task<IEnumerable<user>> GetAllEntitiesTrueAsync()
        {
            return await GetEntitiesAsync(u => u.is_delete == (short)Common.Enum.DelFlagEnum.Normal);
        }

        public async Task<List<menu>> GetMenusByUser(user _user)
        {
           var user_data= await _Db.Set<user>().Include(u => u.roles).ThenInclude(u => u.menus)
                .Where(u=>u.id==_user.id&& u.is_delete == (short)Common.Enum.DelFlagEnum.Normal).FirstOrDefaultAsync();
           var menuList= user_data.roles.Select(u => u.menus);
            return (List<menu>)menuList;
        }

        public async Task<List<mould>> GetMouldByUser(user _user)
        {         
            var user_data = await GetEntity(u => u.id == _user.id && u.is_delete == (short)Common.Enum.DelFlagEnum.Normal);
            var menu = await GetMenusByUser(user_data);
            var mouldList = menu.Select(u=>u.mould);
            return (List<mould>)mouldList;
        }

        public async Task<List<role>> GetRolesByUser(user _user)
        {
            var user_data = await GetEntity(u => u.id == _user.id && u.is_delete == (short)Common.Enum.DelFlagEnum.Normal);
            return (List<role>)user_data.roles;
        }

        public async Task<bool> Login(user _user)
        {
            var user_data =await GetEntity(u => u.username == _user.username&&u.password==_user.password&& 
            u.is_delete == (short)Common.Enum.DelFlagEnum.Normal);
            if (user_data == null)
            {
                return false;
            }
            return true;

        }

        public async Task<bool> Register(user _user)
        {
            var user_data =await GetEntity(u => u.username == _user.username);
            if (user_data != null)
            {
                return false;
            }
           return await AddAsync(_user);
        }

        public async Task<bool> SetRolesByUserId(List<int> roleIds, int userId)
        {
            var user_data =await  GetEntity(u => u.id ==userId &&u.is_delete == (short)Common.Enum.DelFlagEnum.Normal);
            if (user_data == null)
            {
                return false;
            }           
            var roleList = _Db.Set<role>().Where(u => roleIds.Contains(u.id)).ToList();
            
            user_data.roles = roleList;
            return await AddAsync(user_data);
        }
    }
}