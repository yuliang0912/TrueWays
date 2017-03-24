using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueWays.Core.Models;

namespace TrueWays.Core.Repository
{
    public class UserInfoRepository : RepositoryBase<UserInfo>
    {
        const string TableName = "userInfo";

        public UserInfoRepository() : base(TableName)
        {
        }
    }
}
