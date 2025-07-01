using System.Collections.Generic;

/// <summary>
/// Configuration class that defines all game configs to be loaded
/// </summary>
public class GameListConfig : IListConfigDeclaration
{
    public List<IBaseConfig> listConfigs { get; private set; }

    /// <summary>
    /// Constructor that accepts the list of configs to be loaded
    /// </summary>
    /// <param name="configs">List of config instances to be loaded</param>
    public GameListConfig(List<IBaseConfig> configs)
    {
        listConfigs = configs ?? new List<IBaseConfig>();
    }

    /// <summary>
    /// Default constructor with standard game configs
    /// </summary>
    public GameListConfig()
    {
        listConfigs = GetDefaultConfigs();
    }

    /// <summary>
    /// Gets the default list of game configurations
    /// </summary>
    /// <returns>List of default game configs</returns>
    private static List<IBaseConfig> GetDefaultConfigs()
    {
        return new List<IBaseConfig>
        {
            new MapConfig(),
            new EnemyConfig(),
            new TurretConfig(),
            new MapWaveConfig(),
            new PlayerConfig(),
        };
    }

    /// <summary>
    /// Factory method to create GameListConfig with specific configs
    /// </summary>
    /// <param name="configs">Configs to include</param>
    /// <returns>New GameListConfig instance</returns>
    public static GameListConfig Create(params IBaseConfig[] configs)
    {
        return new GameListConfig(new List<IBaseConfig>(configs));
    }

    /// <summary>
    /// Factory method to create GameListConfig with default configs plus additional ones
    /// </summary>
    /// <param name="additionalConfigs">Additional configs to add to defaults</param>
    /// <returns>New GameListConfig instance</returns>
    public static GameListConfig CreateWithDefaults(params IBaseConfig[] additionalConfigs)
    {
        var configs = GetDefaultConfigs();
        if (additionalConfigs != null)
        {
            configs.AddRange(additionalConfigs);
        }
        return new GameListConfig(configs);
    }
}