using System.Collections.Generic;
using UnityEngine;

public sealed class ProfileManager : SingletonInstance<ProfileManager>
{
    [SerializeField]
    private PlayerProfile selectedProfile =
        new PlayerProfile();

    private List<PlayerProfile> loadedProfiles = 
        new List<PlayerProfile>();

    public PlayerProfile SelectedProfile => selectedProfile;
}
