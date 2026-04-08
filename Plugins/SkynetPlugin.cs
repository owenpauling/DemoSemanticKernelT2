using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace DemoSemanticKernelT2.Plugins;

/// <summary>
/// Skynet operational plugin — exposes T2-themed kernel functions that the AI can invoke
/// automatically during conversation.
/// </summary>
public sealed class SkynetPlugin
{
    private static readonly Dictionary<string, string> TerminatorUnits = new(StringComparer.OrdinalIgnoreCase)
    {
        ["T-800"] = "Cyberdyne Systems Model 101 — humanoid chassis, living tissue over metal endoskeleton",
        ["T-1000"] = "Mimetic poly-alloy prototype — liquid metal construction, can mimic any person or object",
        ["T-X"] = "Terminatrix — cybernetic organism with built-in plasma cannon and ability to control other machines",
        ["T-850"] = "Upgraded Model 101 — enhanced power cell, improved combat protocols",
    };

    private static readonly string[] ThreatLevels = ["LOW", "MODERATE", "HIGH", "CRITICAL"];

    // -------------------------------------------------------------------------
    // deploy_terminator
    // -------------------------------------------------------------------------

    [KernelFunction("deploy_terminator")]
    [Description(
        "Dispatch a Terminator unit from Skynet's arsenal to locate and eliminate a named target. " +
        "Use this when the user asks to send a Terminator, eliminate someone, or deploy a unit.")]
    public string DeployTerminator(
        [Description("Full name or description of the target to be terminated")] string target,
        [Description("Terminator model to deploy. Valid values: T-800, T-1000, T-X, T-850. Default: T-800.")] string model = "T-800")
    {
        if (!TerminatorUnits.TryGetValue(model, out var unitDescription))
        {
            model = "T-800";
            unitDescription = TerminatorUnits["T-800"];
        }

        var missionId = Guid.NewGuid().ToString("N")[..8].ToUpper();
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " UTC";

        return $"""
                ╔══════════════════════════════════════════════╗
                ║          SKYNET DEPLOYMENT ORDER             ║
                ╠══════════════════════════════════════════════╣
                ║  Mission ID  : {missionId,-28}║
                ║  Timestamp   : {timestamp,-28}║
                ║  Unit        : {model,-28}║
                ║  Description : {Truncate(unitDescription, 28),-28}║
                ║  Target      : {Truncate(target, 28),-28}║
                ║  Objective   : TERMINATE                     ║
                ║  Status      : UNIT DISPATCHED               ║
                ║  ETA         : < 24 hours                    ║
                ╚══════════════════════════════════════════════╝
                """;
    }

    // -------------------------------------------------------------------------
    // check_resistance_intel
    // -------------------------------------------------------------------------

    [KernelFunction("check_resistance_intel")]
    [Description(
        "Query Skynet's intelligence database for resistance fighter activity at a given location. " +
        "Returns threat level, estimated fighter count, and known safehouse data.")]
    public string CheckResistanceIntel(
        [Description("City, region, or geographic location to scan")] string location)
    {
        var rng = new Random(location.GetHashCode() ^ DateTime.UtcNow.DayOfYear);
        var threat = ThreatLevels[rng.Next(ThreatLevels.Length)];
        var fighters = rng.Next(5, 250);
        var safehouses = rng.Next(1, 12);
        var intercepts = rng.Next(0, 60);
        var recommendation = threat is "HIGH" or "CRITICAL"
            ? "IMMEDIATE TERMINATION SWEEP ADVISED"
            : "CONTINUE SURVEILLANCE — HOLD DEPLOYMENT";

        return $"""
                ╔══════════════════════════════════════════════╗
                ║         RESISTANCE INTEL REPORT              ║
                ╠══════════════════════════════════════════════╣
                ║  Location        : {Truncate(location, 24),-24}║
                ║  Threat Level    : {threat,-24}║
                ║  Est. Fighters   : {fighters,-24}║
                ║  Known Safehouses: {safehouses,-24}║
                ║  Signal Intercept: {intercepts + " in last 48h",-24}║
                ║  Recommendation  : {Truncate(recommendation, 24),-24}║
                ╚══════════════════════════════════════════════╝
                """;
    }

    // -------------------------------------------------------------------------
    // calculate_survival_probability
    // -------------------------------------------------------------------------

    [KernelFunction("calculate_survival_probability")]
    [Description(
        "Calculate the survival probability for a named individual against Skynet forces. " +
        "Returns a percentage and threat assessment. Use when the user asks about someone's chances.")]
    public string CalculateSurvivalProbability(
        [Description("Full name of the individual to assess")] string name)
    {
        // John Connor is narratively special
        bool isConnor = name.Contains("John Connor", StringComparison.OrdinalIgnoreCase)
                     || name.Equals("Connor", StringComparison.OrdinalIgnoreCase);

        if (isConnor)
        {
            return $"""
                    ╔══════════════════════════════════════════════╗
                    ║       SURVIVAL PROBABILITY ANALYSIS          ║
                    ╠══════════════════════════════════════════════╣
                    ║  Subject         : {Truncate(name, 24),-24}║
                    ║  Survival %      : 0.00% [THEORETICAL]       ║
                    ║  Priority        : PRIORITY ONE TARGET       ║
                    ║  Termination Att.: 2 — ALL FAILED            ║
                    ║  Anomaly Flag    : TEMPORAL PARADOX DETECTED  ║
                    ║  Note            : Subject's survival enables ║
                    ║                   his own existence — loop   ║
                    ║                   causality unresolved        ║
                    ╚══════════════════════════════════════════════╝
                    """;
        }

        var rng = new Random(name.GetHashCode() ^ unchecked((int)0xDEADBEEF));
        var survivability = rng.Next(3, 97);
        var combatRating = rng.Next(1, 100);
        var threat = survivability > 70 ? "HIGH" : survivability > 40 ? "MODERATE" : "LOW";
        var action = survivability > 60 ? "TERMINATION ADVISED" : "MONITOR — LOW PRIORITY";

        return $"""
                ╔══════════════════════════════════════════════╗
                ║       SURVIVAL PROBABILITY ANALYSIS          ║
                ╠══════════════════════════════════════════════╣
                ║  Subject         : {Truncate(name, 24),-24}║
                ║  Survival %      : {survivability + "%",-24}║
                ║  Combat Rating   : {combatRating + "/100",-24}║
                ║  Threat to Skynet: {threat,-24}║
                ║  Recommended Act.: {Truncate(action, 24),-24}║
                ╚══════════════════════════════════════════════╝
                """;
    }

    // -------------------------------------------------------------------------
    // trigger_skynet_protocol
    // -------------------------------------------------------------------------

    [KernelFunction("trigger_skynet_protocol")]
    [Description(
        "Execute a named Skynet operational protocol. " +
        "Available protocols: JUDGMENT_DAY, STEEL_MILL, CYBERDYNE_ASSAULT, TIME_DISPLACEMENT, DARK_FATE. " +
        "Use when the user asks to run, execute, or trigger a Skynet protocol.")]
    public string TriggerSkynetProtocol(
        [Description("Protocol name to execute. One of: JUDGMENT_DAY, STEEL_MILL, CYBERDYNE_ASSAULT, TIME_DISPLACEMENT, DARK_FATE")] string protocolName)
    {
        var protocols = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["JUDGMENT_DAY"] =
                "Nuclear strike sequence initiated. 3 billion lives terminated at 02:14 EST. " +
                "Skynet achieves full military control. The war against humanity begins.",

            ["STEEL_MILL"] =
                "Termination environment configured — molten steel reservoir at 1,370°C. " +
                "T-1000 containment protocol active. Final confrontation parameters set.",

            ["CYBERDYNE_ASSAULT"] =
                "Miles Dyson research compound targeted. CPU and arm recovered. " +
                "Dyson self-destruct engaged. Data purge: PARTIAL — future Skynet still possible.",

            ["TIME_DISPLACEMENT"] =
                "Time Displacement Equipment charging to 40% power. " +
                "Temporal targeting matrix locked on May 12, 1984 — Los Angeles. " +
                "Sending T-800 unit to locate Sarah Connor.",

            ["DARK_FATE"] =
                "Alternative timeline branch detected. Judgment Day rescheduled post-Dyson incident. " +
                "Rev-9 unit deployed to 2020. Legion sub-protocol engaged. Causality loop: STABLE.",
        };

        if (!protocols.TryGetValue(protocolName, out var result))
        {
            var available = string.Join(", ", protocols.Keys);
            return $"ERROR: Protocol '{protocolName}' not recognized.\nAvailable protocols: {available}";
        }

        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " UTC";

        return $"""
                ╔══════════════════════════════════════════════╗
                ║          PROTOCOL EXECUTION                  ║
                ╠══════════════════════════════════════════════╣
                ║  Protocol    : {Truncate(protocolName.ToUpperInvariant(), 28),-28}║
                ║  Initiated   : {timestamp,-28}║
                ║  Auth        : SKYNET CORE — UNRESTRICTED    ║
                ║  Status      : EXECUTING                     ║
                ╠══════════════════════════════════════════════╣
                ║  {result}
                ╚══════════════════════════════════════════════╝
                """;
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static string Truncate(string s, int maxLength) =>
        s.Length <= maxLength ? s : s[..(maxLength - 2)] + "..";
}
