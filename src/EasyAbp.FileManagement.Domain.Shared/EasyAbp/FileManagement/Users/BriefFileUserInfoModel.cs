using System;

namespace EasyAbp.FileManagement.Users;

[Serializable]
public class BriefFileUserInfoModel
{
    public Guid Id { get; set; }

    public string UserName { get; set; }

    public BriefFileUserInfoModel(Guid id, string userName)
    {
        Id = id;
        UserName = userName;
    }
}