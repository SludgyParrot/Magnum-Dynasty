using System;

[Serializable]
public sealed class PlayerProfile : Profile
{
    public GameData Data;

    public PlayerProfile() { }

    public PlayerProfile(string profileName, 
        GameData data)
    {
        ProfileName = profileName;
        Data = data;
    }
}
