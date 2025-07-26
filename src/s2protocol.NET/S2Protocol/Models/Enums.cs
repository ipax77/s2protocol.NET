namespace s2protocol.NET.S2Protocol.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
/// <summary>
/// Provides enumerations for various protocol attributes and namespaces used in the S2 game system.
/// </summary>
/// <remarks>This class contains nested enumerations that define constants for attribute namespaces and specific
/// attributes used in the S2 protocol. These enumerations are typically used to identify and configure game settings,
/// player roles, AI behavior, and other protocol-related metadata.</remarks>
public static class S2ProtocolEnums
{
    /// <summary>
    /// Represents the namespace for attributes used in the application.
    /// </summary>
    /// <remarks>This enumeration defines the possible namespaces for attributes.  The value <see
    /// cref="BLIZZARD"/> corresponds to the Blizzard namespace.</remarks>
    public enum AttributeNamespace
    {
        None = 0,
        BLIZZARD = 999
    };

    /// <summary>
    /// Represents a set of predefined attributes used to configure and manage various aspects of a game,  such as game
    /// modes, party configurations, AI settings, and player-specific options.
    /// </summary>
    /// <remarks>This enumeration provides a comprehensive list of attributes that can be used to define game
    /// settings,  player roles, and other configurations. Each attribute is associated with a unique integer value, 
    /// which can be used to identify and apply the corresponding setting.   Common use cases include: - Configuring
    /// game modes (e.g., <see cref="GAME_MODE"/>). - Setting up party configurations (e.g., <see
    /// cref="PARTIES_PREMADE_1V1"/>). - Defining AI behavior and skill levels (e.g., <see cref="AI_SKILL"/>). -
    /// Managing player-specific options like race, handicap, or commander settings.  The values are grouped logically
    /// to represent different categories, such as party configurations,  AI settings, and game-specific
    /// options.</remarks>
#pragma warning disable CA1707 // Identifiers should not contain underscores
    public enum Attributes
    {
        NONE = 0,
        CONTROLLER = 500,
        RULES = 1000,
        IS_PREMADE_GAME = 1001,
        PARTIES_PRIVATE = 2000,
        PARTIES_PREMADE = 2001,
        PARTIES_PREMADE_1V1 = 2002,
        PARTIES_PREMADE_2V2 = 2003,
        PARTIES_PREMADE_3V3 = 2004,
        PARTIES_PREMADE_4V4 = 2005,
        PARTIES_PREMADE_FFA = 2006,
        PARTIES_PREMADE_5V5 = 2007,
        PARTIES_PREMADE_6V6 = 2008,
        PARTIES_PRIVATE_ONE = 2010,
        PARTIES_PRIVATE_TWO = 2011,
        PARTIES_PRIVATE_THREE = 2012,
        PARTIES_PRIVATE_FOUR = 2013,
        PARTIES_PRIVATE_FIVE = 2014,
        PARTIES_PRIVATE_SIX = 2015,
        PARTIES_PRIVATE_SEVEN = 2016,
        PARTIES_PRIVATE_FFA = 2017,
        PARTIES_PRIVATE_CUSTOM = 2018,
        PARTIES_PRIVATE_EIGHT = 2019,
        PARTIES_PRIVATE_NINE = 2020,
        PARTIES_PRIVATE_TEN = 2021,
        PARTIES_PRIVATE_ELEVEN = 2022,
        PARTIES_PRIVATE_FFA_TANDEM = 2023,
        PARTIES_PRIVATE_CUSTOM_TANDEM = 2024,
        GAME_SPEED = 3000,
        RACE = 3001,
        PARTY_COLOR = 3002,
        HANDICAP = 3003,
        AI_SKILL = 3004,
        AI_RACE = 3005,
        LOBBY_DELAY = 3006,
        PARTICIPANT_ROLE = 3007,
        WATCHER_TYPE = 3008,
        GAME_MODE = 3009,
        LOCKED_ALLIANCES = 3010,
        PLAYER_LOGO = 3011,
        TANDEM_LEADER = 3012,
        COMMANDER = 3013,
        COMMANDER_LEVEL = 3014,
        GAME_DURATION = 3015,
        COMMANDER_MASTERY_LEVEL = 3016,
        AI_BUILD_FIRST = 3100,
        AI_BUILD_LAST = 3300,
        PRIVACY_OPTION = 4000,
        USING_CUSTOM_OBSERVER_UI = 4001,
        CAN_READY = 4009,
        LOBBY_MODE = 4010,
        READY_ORDER_DEPRECATED = 4011,
        ACTIVE_TEAM = 4012,
        LOBBY_PHASE = 4015,
        READYING_COUNT_DEPRECATED = 4016,
        ACTIVE_ROUND = 4017,
        READY_MODE = 4018,
        READY_REQUIREMENTS = 4019,
        FIRST_ACTIVE_TEAM = 4020,
        COMMANDER_MASTERY_TALENT_FIRST = 5000,
        COMMANDER_MASTERY_TALENT_LAST = 5005,
    }
}

#pragma warning restore CA1707 // Identifiers should not contain underscores

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
