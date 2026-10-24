using UnityEngine;

public sealed class GameManager : SingletonInstance<GameManager>
{
    private GameData gameData;

    public GameData GameData => ProfileManager.Instance.SelectedProfile.Data;

}
